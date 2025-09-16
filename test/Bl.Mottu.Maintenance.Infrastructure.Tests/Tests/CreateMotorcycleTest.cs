using Bl.Mottu.Maintenance.Core.Commands.CreateMotorcycle;
using Bl.Mottu.Maintenance.Core.Primitive;
using Bl.Mottu.Maintenance.Core.Repository;
using Smartec.Web.Infrastructure.Tests.TestBase;

namespace Bl.Mottu.Maintenance.Infrastructure.Tests.Tests;

[Collection(InfrastructureTestFixtureCollection.CollectionName)]
public class CreateMotorcycleTest
{
    private readonly InfrastructureTestFixture _fixture;

    public CreateMotorcycleTest(InfrastructureTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Send_ShouldCreateNewMotorcycle()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var request = CreateRandom();
        var response = await mediator.Send(request);

        Assert.NotNull(response);
        Assert.True(response.Result.IsSuccess);
    }

    [Fact]
    public async Task Send_ShouldMatchModel()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var request = CreateRandom();
        var response = await mediator.Send(request with
        {
            Model = "Honda CB 500F"
        });

        Assert.Equal("HONDA CB 500F", response.Result.RequiredResult.Model);
    }

    [Fact]
    public async Task Send_ShouldMatchCode()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var request = CreateRandom();
        var response = await mediator.Send(request with
        {
            Code = " MOTO-001 "
        });

        Assert.Equal("MOTO-001", response.Result.RequiredResult.Code);
    }

    [Fact]
    public async Task Send_ShouldMatchPlaca()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var request = CreateRandom();
        var response = await mediator.Send(request with
        {
            Placa = "ABC1D23"
        });

        Assert.Equal("ABC1D23", response.Result.RequiredResult.Placa);
    }

    [Fact]
    public async Task Send_ShouldMatchYear()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var request = CreateRandom();
        var response = await mediator.Send(request with
        {
            Year = 2023
        });

        Assert.Equal(2023, response.Result.RequiredResult.Year);
    }

    [Fact]
    public async Task Send_ShouldReturnConflict_WhenPlacaAlreadyExists()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        // First create a motorcycle
        var firstRequest = CreateRandom();
        var firstResponse = await mediator.Send(firstRequest);
        Assert.True(firstResponse.Result.IsSuccess);

        // Try to create another with same placa
        var secondRequest = CreateRandom() with { Placa = firstRequest.Placa };
        var secondResponse = await mediator.Send(secondRequest);

        Assert.False(secondResponse.Result.IsSuccess);
        Assert.Contains(CoreExceptionCode.Conflict, secondResponse.Result.Errors.Select(x => x.StatusCode));
    }

    [Fact]
    public async Task Send_ShouldReturnConflict_WhenCodeAlreadyExists()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        // First create a motorcycle
        var firstRequest = CreateRandom();
        var firstResponse = await mediator.Send(firstRequest);
        Assert.True(firstResponse.Result.IsSuccess);

        // Try to create another with same code
        var secondRequest = CreateRandom() with { Code = firstRequest.Code };
        var secondResponse = await mediator.Send(secondRequest);

        Assert.False(secondResponse.Result.IsSuccess);
        Assert.Contains(CoreExceptionCode.Conflict, secondResponse.Result.Errors.Select(x => x.StatusCode));
    }

    private static CreateMotorcycleRequest CreateRandom()
    {
        return new CreateMotorcycleRequest(
            Code: $"MOTO-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            Placa: GenerateRandomPlaca(),
            Model: "HONDA CB 500F",
            Year: Random.Shared.Next(2010, 2024));
    }

    private static string GenerateRandomPlaca()
    {
        var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var numbers = "0123456789";

        var random = new Random();
        var placa = $"{letters[random.Next(letters.Length)]}" +
                   $"{letters[random.Next(letters.Length)]}" +
                   $"{letters[random.Next(letters.Length)]}" +
                   $"{numbers[random.Next(numbers.Length)]}" +
                   $"{letters[random.Next(letters.Length)]}" +
                   $"{numbers[random.Next(numbers.Length)]}" +
                   $"{numbers[random.Next(numbers.Length)]}";

        return placa;
    }
}