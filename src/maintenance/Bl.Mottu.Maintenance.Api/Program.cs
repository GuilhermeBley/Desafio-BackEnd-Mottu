using Bl.Mottu.Maintenance.Api.Model;
using Bl.Mottu.Maintenance.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure();

builder.Services.Configure<Bl.Mottu.Maintenance.Infrastructure.Config.PostgreConfig>(
     builder.Configuration.GetSection("PostgreConfig"));

builder.Services.Configure<Bl.Mottu.Maintenance.Infrastructure.Config.StorageAccountConfig>(
     builder.Configuration.GetSection("StorageAccountConfig"));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

#region Motorcycle endpoints
app.MapPost("/motos", async (
    [FromBody] CreateMotorcycleViewModel model,
    [FromServices]IMediator mediator,
    CancellationToken cancellationToken) =>
{
    var response = await mediator.Send(new Bl.Mottu.Maintenance.Core.Commands.CreateMotorcycle.CreateMotorcycleRequest(
        Code: model.Identificador ?? string.Empty,
        Placa: model.Placa ?? string.Empty,
        Model: model.Modelo ?? string.Empty,
        Year: model.Ano),
        cancellationToken);

    if (response.Result.IsSuccess) return Results.Created();

    return Results.BadRequest(new ResultViewModel("Dados inválidos"));
});

app.MapGet("/motos", async (
    [FromServices] IMediator mediator,
    CancellationToken cancellationToken,
    [FromQuery] string? placa = null) =>
{
    var response = await mediator.Send(new Bl.Mottu.Maintenance.Core.Commands.GetMotorcycles.GetMotorcyclesRequest(PlacaFilter: placa, CodeFilter: null),
        cancellationToken);

    return Results.Ok(response.Results
        .Select(x => new
        {
            identificador = x.Code,
            ano = x.Year,
            modelo = x.Model,
            placa = x.Placa,
        }));
});

app.MapPut("/motos/{id}/placa", async (
    string id,
    [FromBody] PutMotorcycleViewModel model,
    [FromServices] IMediator mediator,
    CancellationToken cancellationToken) =>
{
    var response = await mediator.Send(new Bl.Mottu.Maintenance.Core.Commands.PatchMotorcyclePlacaByCode.PatchMotorcyclePlacaByCodeRequest(
        Code: id ?? string.Empty,
        Placa: model.Placa ?? string.Empty),
        cancellationToken);

    if (response.Result.IsSuccess)
        return Results.Ok(new ResultViewModel("Placa modificada com sucesso"));

    return Results.BadRequest(new ResultViewModel("Dados inválidos"));
});

app.MapDelete("/motos/{id}", async (
    string id,
    [FromServices] IMediator mediator,
    CancellationToken cancellationToken) =>
{
    var response = await mediator.Send(new Bl.Mottu.Maintenance.Core.Commands.DeleteMotorcycleById.DeleteMotorcycleByIdRequest(id),
        cancellationToken);

    if (response.Result.IsSuccess) return Results.Ok();

    return Results.BadRequest(new ResultViewModel("Dados inválidos"));
});

app.MapGet("/motos/{id}", async (
    string id,
    [FromServices] IMediator mediator,
    CancellationToken cancellationToken) =>
{
    var response = await mediator.Send(new Bl.Mottu.Maintenance.Core.Commands.GetMotorcycles.GetMotorcyclesRequest(PlacaFilter: null, CodeFilter: id),
        cancellationToken);

    return Results.Ok(response.Results
        .Select(x => new
        {
            identificador = x.Code,
            ano = x.Year,
            modelo = x.Model,
            placa = x.Placa,
        })
        .FirstOrDefault());
});

#endregion

app.Run();
