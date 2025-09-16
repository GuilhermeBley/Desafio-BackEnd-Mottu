using Bl.Mottu.Maintenance.Core.Commands.CreateDeliveryDriver;
using Bl.Mottu.Maintenance.Core.Commands.CreateMotorcycle;
using Bl.Mottu.Maintenance.Core.Commands.CreateVehicleRent;
using Bl.Mottu.Maintenance.Core.Model;
using Bl.Mottu.Maintenance.Core.Primitive;
using Bl.Mottu.Maintenance.Infrastructure.Tests.Mock;
using Smartec.Web.Infrastructure.Tests.TestBase;

namespace Bl.Mottu.Maintenance.Infrastructure.Tests.Tests;

[Collection(InfrastructureTestFixtureCollection.CollectionName)]
public class CreateVehicleRentTest
{
    private readonly InfrastructureTestFixture _fixture;

    public CreateVehicleRentTest(InfrastructureTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Send_ShouldCreateNewVehicleRent()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // Create prerequisite entities
        var (deliveryDriver, motorcycle) = await CreatePrerequisiteEntities(mediator);

        var request = CreateRandom(deliveryDriver.Code, motorcycle.Code);
        var response = await mediator.Send(request);

        Assert.NotNull(response);
        Assert.True(response.Result.IsSuccess);
    }

    [Fact]
    public async Task Send_ShouldReturnNotFound_WhenDeliveryDriverNotFound()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // Create only motorcycle
        var motorcycleRequest = CreateRandomMotorcycleRequest();
        var motorcycleResponse = await mediator.Send(motorcycleRequest);
        Assert.True(motorcycleResponse.Result.IsSuccess);

        var request = CreateRandom("NONEXISTENT-DRIVER", motorcycleResponse.Result.RequiredResult.Code);
        var response = await mediator.Send(request);

        Assert.False(response.Result.IsSuccess);
        Assert.Contains(CoreExceptionCode.NotFound, response.Result.Errors.Select(x => x.StatusCode));
    }

    [Fact]
    public async Task Send_ShouldReturnNotFound_WhenVehicleNotFound()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // Create only delivery driver
        var driverRequest = CreateRandomDeliveryDriverRequest();
        var driverResponse = await mediator.Send(driverRequest);
        Assert.True(driverResponse.Result.IsSuccess);

        var request = CreateRandom(driverResponse.Result.RequiredResult.Code, "NONEXISTENT-VEHICLE");
        var response = await mediator.Send(request);

        Assert.False(response.Result.IsSuccess);
        Assert.Contains(CoreExceptionCode.NotFound, response.Result.Errors.Select(x => x.StatusCode));
    }

    [Fact]
    public async Task Send_ShouldReturnInsufficientCategory_WhenDriverHasNoACategory()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // Create delivery driver without A category
        var driverRequest = CreateRandomDeliveryDriverRequest() with { CnhCategory = "B" };
        var driverResponse = await mediator.Send(driverRequest);
        Assert.True(driverResponse.Result.IsSuccess);

        // Create motorcycle
        var motorcycleRequest = CreateRandomMotorcycleRequest();
        var motorcycleResponse = await mediator.Send(motorcycleRequest);
        Assert.True(motorcycleResponse.Result.IsSuccess);

        var request = CreateRandom(driverResponse.Result.RequiredResult.Code, motorcycleResponse.Result.RequiredResult.Code);
        var response = await mediator.Send(request);

        Assert.False(response.Result.IsSuccess);
        Assert.Contains(CoreExceptionCode.DriverHasInsufficientCategory, response.Result.Errors.Select(x => x.StatusCode));
    }

    [Fact]
    public async Task Send_ShouldAllowRent_WhenDriverHasACategory()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // Create delivery driver with A category
        var driverRequest = CreateRandomDeliveryDriverRequest() with { CnhCategory = "A" };
        var driverResponse = await mediator.Send(driverRequest);
        Assert.True(driverResponse.Result.IsSuccess);

        // Create motorcycle
        var motorcycleRequest = CreateRandomMotorcycleRequest();
        var motorcycleResponse = await mediator.Send(motorcycleRequest);
        Assert.True(motorcycleResponse.Result.IsSuccess);

        var request = CreateRandom(driverResponse.Result.RequiredResult.Code, motorcycleResponse.Result.RequiredResult.Code);
        var response = await mediator.Send(request);

        Assert.True(response.Result.IsSuccess);
    }

    [Fact]
    public async Task Send_ShouldAllowRent_WhenDriverHasABCategory()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // Create delivery driver with AB category
        var driverRequest = CreateRandomDeliveryDriverRequest() with { CnhCategory = "AB" };
        var driverResponse = await mediator.Send(driverRequest);
        Assert.True(driverResponse.Result.IsSuccess);

        // Create motorcycle
        var motorcycleRequest = CreateRandomMotorcycleRequest();
        var motorcycleResponse = await mediator.Send(motorcycleRequest);
        Assert.True(motorcycleResponse.Result.IsSuccess);

        var request = CreateRandom(driverResponse.Result.RequiredResult.Code, motorcycleResponse.Result.RequiredResult.Code);
        var response = await mediator.Send(request);

        Assert.True(response.Result.IsSuccess);
    }

    [Fact]
    public async Task Send_ShouldReturnConflict_WhenVehicleIsAlreadyRented()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // Create prerequisite entities
        var (deliveryDriver, motorcycle) = await CreatePrerequisiteEntities(mediator);

        // Create first rent
        var firstRequest = CreateRandom(deliveryDriver.Code, motorcycle.Code);
        var firstResponse = await mediator.Send(firstRequest);
        firstResponse.Result.EnsureSuccess();

        // Try to create second rent for same vehicle with overlapping dates
        var secondRequest = firstRequest with
        {
            StartAt = firstRequest.StartAt.AddDays(1),
            ExpectedEndingDate = firstRequest.StartAt.AddDays(8),
            Plan = 7
        };
        var secondResponse = await mediator.Send(secondRequest);

        Assert.False(secondResponse.Result.IsSuccess);
        Assert.Contains(CoreExceptionCode.Conflict, secondResponse.Result.Errors.Select(x => x.StatusCode));
    }

    [Fact]
    public async Task Send_ShouldReturnConflictInExpectedEndingDate_WhenVehicleIsAlreadyRented()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // Create prerequisite entities
        var (deliveryDriver, motorcycle) = await CreatePrerequisiteEntities(mediator);

        // Create first rent
        var firstRequest = CreateRandom(deliveryDriver.Code, motorcycle.Code);
        var firstResponse = await mediator.Send(firstRequest);
        firstResponse.Result.EnsureSuccess();

        // Try to create second rent for same vehicle with overlapping dates
        var secondRequest = firstRequest with
        {
            StartAt = firstRequest.StartAt.AddDays(-6), // before the firstRequest.StartAt
            ExpectedEndingDate = firstRequest.StartAt.AddDays(-6).AddDays(15), // getting 15 days to conflict with 'ExpectedEndingDate'
            Plan = 15
        };
        var secondResponse = await mediator.Send(secondRequest);

        Assert.False(secondResponse.Result.IsSuccess);
        Assert.Contains(CoreExceptionCode.Conflict, secondResponse.Result.Errors.Select(x => x.StatusCode));
    }

    [Fact]
    public async Task Send_ShouldAllowRent_WhenVehicleIsNotRentedInPeriod()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // Create prerequisite entities
        var (deliveryDriver, motorcycle) = await CreatePrerequisiteEntities(mediator);

        // Create first rent
        var firstRequest = CreateRandom(deliveryDriver.Code, motorcycle.Code) with
        {
            StartAt = DateTime.UtcNow.AddDays(1),
            ExpectedEndingDate = DateTime.UtcNow.AddDays(8)
        };
        var firstResponse = await mediator.Send(firstRequest);
        firstResponse.Result.EnsureSuccess();

        // Create second rent for different period (should be allowed)
        var secondRequest = CreateRandom(deliveryDriver.Code, motorcycle.Code) with
        {
            StartAt = DateTime.UtcNow.AddDays(10),
            ExpectedEndingDate = DateTime.UtcNow.AddDays(17)
        };
        var secondResponse = await mediator.Send(secondRequest);

        Assert.Empty(secondResponse.Result.Errors);
        Assert.True(secondResponse.Result.IsSuccess);
    }

    [Fact]
    public async Task Send_ShouldMatchRentDates()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var (deliveryDriver, motorcycle) = await CreatePrerequisiteEntities(mediator);

        var specificStartDate = DateTime.UtcNow.AddDays(1);
        var specificEndDate = DateTime.UtcNow.AddDays(10);

        var request = CreateRandom(deliveryDriver.Code, motorcycle.Code) with
        {
            StartAt = specificStartDate,
            ExpectedEndingDate = specificEndDate,
            Plan = 7
        };
        var response = await mediator.Send(request);

        Assert.True(response.Result.IsSuccess);
        Assert.Equal(specificStartDate, response.Result.RequiredResult.StartAt);
        Assert.Equal(specificEndDate, response.Result.RequiredResult.ExpectedEndingDate);
        Assert.Equal(7, response.Result.RequiredResult.Plan);
    }

    private async Task<(DeliveryDriverModel DeliveryDriver, MotorcycleModel Motorcycle)> CreatePrerequisiteEntities(IMediator mediator)
    {
        // Create delivery driver with A category
        var driverRequest = CreateRandomDeliveryDriverRequest() with { CnhCategory = "A" };
        var driverResponse = await mediator.Send(driverRequest);
        Assert.True(driverResponse.Result.IsSuccess);

        // Create motorcycle
        var motorcycleRequest = CreateRandomMotorcycleRequest();
        var motorcycleResponse = await mediator.Send(motorcycleRequest);
        Assert.True(motorcycleResponse.Result.IsSuccess);

        return (driverResponse.Result.RequiredResult, motorcycleResponse.Result.RequiredResult);
    }

    private static CreateVehicleRentRequest CreateRandom(string deliveryDriverCode, string vehicleCode)
    {
        var startDate = DateTime.UtcNow.AddDays(1);
        return new CreateVehicleRentRequest(
            DeliveryDriverCode: deliveryDriverCode,
            VehicleCode: vehicleCode,
            StartAt: startDate,
            EndedAt: null,
            ExpectedEndingDate: startDate.AddDays(7),
            Plan: 7);
    }

    private static CreateDeliveryDriverRequest CreateRandomDeliveryDriverRequest()
    {
        return new CreateDeliveryDriverRequest(
            Code: $"DRIVER-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            Name: $"TEST DRIVER",
            Cnpj: $"{Random.Shared.NextInt64(10_000_000_0000_00, 99_999_999_9999_99)}",
            BirthDate: new DateOnly(2000, 1, 1),
            CnhNumber: $"{Random.Shared.NextInt64(10_000_000_000, 99_999_999_999)}",
            CnhCategory: "A");
    }

    private static CreateMotorcycleRequest CreateRandomMotorcycleRequest()
    {
        return new CreateMotorcycleRequest(
            Code: $"MOTO-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            Placa: PlacaMock.NextPlaca(),
            Model: "HONDA CB 500F",
            Year: Random.Shared.Next(2010, 2024));
    }
}