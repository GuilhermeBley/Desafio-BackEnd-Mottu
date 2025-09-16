using Bl.Mottu.Maintenance.Api.Model;
using Bl.Mottu.Maintenance.Core.Model;
using Bl.Mottu.Maintenance.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;

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

    var result = response.Results
        .Select(x => new
        {
            identificador = x.Code,
            ano = x.Year,
            modelo = x.Model,
            placa = x.Placa,
        })
        .FirstOrDefault();

    if (result is null) return Results.NotFound();

    return Results.Ok();
});
#endregion

#region Delivery drivers endpoints
app.MapPost("/entregadores", async (
    [FromBody] CreateDeliveryDriverViewModel model,
    [FromServices] IMediator mediator,
    CancellationToken cancellationToken) =>
{
    var response = await mediator.Send(new Bl.Mottu.Maintenance.Core.Commands.CreateDeliveryDriver.CreateDeliveryDriverRequest(
        Code: model.Identificador,
        Name: model.Nome,
        Cnpj: model.Cnpj,
        BirthDate: DateOnly.FromDateTime(model.DataNascimento),
        CnhNumber: model.NumeroCnh,
        CnhCategory: model.TipoCnh,
        CnhImage: model.GetCnhImage()), // TODO: dispose this
        cancellationToken);

    if (response.Result.IsSuccess) return Results.Created();

    return Results.BadRequest(new ResultViewModel("Dados inválidos"));
});

app.MapPost("/entregadores/{id}/cnh", async (
    string id,
    [FromBody] CreateCnhDeliveryDriverViewModel model,
    [FromServices] IMediator mediator,
    CancellationToken cancellationToken) =>
{
    var response = await mediator.Send(new Bl.Mottu.Maintenance.Core.Commands.PatchDeliveryDriverPhoto.PatchDeliveryDriverPhotoRequest(
        Code: id,
        CnhImage: model.GetCnhImage()), // TODO: dispose this
        cancellationToken);

    if (response.Result.IsSuccess) return Results.Created();

    return Results.BadRequest(new ResultViewModel("Dados inválidos"));
});
#endregion

#region Vehicle Rent endpoints

app.MapPost("/locacao", async (
    [FromBody] CreateVehicleRentViewModel model,
    [FromServices] IMediator mediator,
    CancellationToken cancellationToken) =>
{
    var response = await mediator.Send(new Bl.Mottu.Maintenance.Core.Commands.CreateVehicleRent.CreateVehicleRentRequest(
        DeliveryDriverCode: model.EntregadorId ?? string.Empty,
        VehicleCode: model.MotoId ?? string.Empty,
        StartAt: model.DataInicio.UtcDateTime,
        EndedAt: model.DataTermino.UtcDateTime,
        ExpectedEndingDate: model.DataPrevisaoTermino.UtcDateTime,
        Plan: model.Plano),
        cancellationToken);

    if (response.Result.IsSuccess) return Results.Created();

    return Results.BadRequest(new ResultViewModel("Dados inválidos"));
});

app.MapGet("/locacao/{id}", async (
    Guid id,
    [FromServices] IMediator mediator,
    CancellationToken cancellationToken) =>
{
    var response = await mediator.Send(new Bl.Mottu.Maintenance.Core.Commands.GetVehicleRentById.GetVehicleRentByIdRequest(id),
        cancellationToken);

    if (response.Result is null) return Results.NotFound();

    return Results.Ok(new
    {
        identificador = response.Result.Id,
        valor_diaria = response.Result.DailyValue,
        entregador_id = response.Result.DeliveryDriverId,
        moto_id = response.Result.VehicleId,
        data_inicio = response.Result.StartAt,
        data_termino = response.Result.EndedAt,
        data_previsao_termino = response.Result.ExpectedEndingDate,
        data_devolucao = response.Result.EndedAt,
    });
});

app.MapPut("/locacao/{id}/devolucao", async (
    Guid id,
    [FromBody] PatchVehicleRentEndingDateViewModel model,
    [FromServices] IMediator mediator,
    CancellationToken cancellationToken) =>
{
    var response = await mediator.Send(new Bl.Mottu.Maintenance.Core.Commands.PatchVehicleRentEndingDate.PatchVehicleRentEndingDateRequest(
        Id: id,
        DevolutionDate: model.DataTermino.UtcDateTime),
        cancellationToken);

    if (response.Result.IsSuccess) return Results.Ok();

    return Results.BadRequest(new ResultViewModel("Dados inválidos"));
});

#endregion

app.Run();
