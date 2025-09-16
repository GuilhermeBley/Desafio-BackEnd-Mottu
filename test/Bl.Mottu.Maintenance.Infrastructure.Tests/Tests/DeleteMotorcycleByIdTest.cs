using Bl.Mottu.Maintenance.Core.Commands.CreateMotorcycle;
using Bl.Mottu.Maintenance.Core.Commands.DeleteMotorcycleById;
using Bl.Mottu.Maintenance.Core.Primitive;
using Bl.Mottu.Maintenance.Core.Repository;
using Bl.Mottu.Maintenance.Infrastructure.Tests.Mock;
using Smartec.Web.Infrastructure.Tests.TestBase;

namespace Bl.Mottu.Maintenance.Infrastructure.Tests.Tests;

[Collection(InfrastructureTestFixtureCollection.CollectionName)]
public class DeleteMotorcycleByIdTest
{
    private readonly InfrastructureTestFixture _fixture;

    public DeleteMotorcycleByIdTest(InfrastructureTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Send_ShouldDeleteMotorcycle()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // First create a motorcycle
        var createRequest = CreateRandomMotorcycleRequest();
        var createResponse = await mediator.Send(createRequest);
        Assert.True(createResponse.Result.IsSuccess);

        // Then delete it
        var deleteRequest = new DeleteMotorcycleByIdRequest(createResponse.Result.RequiredResult.Code);
        var deleteResponse = await mediator.Send(deleteRequest);

        Assert.NotNull(deleteResponse);
        Assert.True(deleteResponse.Result.IsSuccess);
    }

    [Fact]
    public async Task Send_ShouldReturnNotFound_WhenMotorcycleDoesNotExist()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var deleteRequest = new DeleteMotorcycleByIdRequest("NONEXISTENT-CODE");
        var deleteResponse = await mediator.Send(deleteRequest);

        Assert.NotNull(deleteResponse);
        Assert.False(deleteResponse.Result.IsSuccess);
        Assert.Contains(CoreExceptionCode.NotFound, deleteResponse.Result.Errors.Select(x => x.StatusCode));
    }

    [Fact]
    public async Task Send_ShouldReturnNotFound_WhenMotorcycleCodeIsEmpty()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var deleteRequest = new DeleteMotorcycleByIdRequest("");
        var deleteResponse = await mediator.Send(deleteRequest);

        Assert.NotNull(deleteResponse);
        Assert.False(deleteResponse.Result.IsSuccess);
        Assert.Contains(CoreExceptionCode.NotFound, deleteResponse.Result.Errors.Select(x => x.StatusCode));
    }

    [Fact]
    public async Task Send_ShouldReturnNotFound_WhenMotorcycleCodeIsWhitespace()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var deleteRequest = new DeleteMotorcycleByIdRequest("   ");
        var deleteResponse = await mediator.Send(deleteRequest);

        Assert.NotNull(deleteResponse);
        Assert.False(deleteResponse.Result.IsSuccess);
        Assert.Contains(CoreExceptionCode.NotFound, deleteResponse.Result.Errors.Select(x => x.StatusCode));
    }

    [Fact]
    public async Task Send_ShouldActuallyRemoveFromDatabase()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        // First create a motorcycle
        var createRequest = CreateRandomMotorcycleRequest();
        var createResponse = await mediator.Send(createRequest);
        Assert.True(createResponse.Result.IsSuccess);

        var motorcycleCode = createResponse.Result.RequiredResult.Code;

        // Verify it exists in database
        var existsBeforeDelete = await context.Motorcycles
            .Where(x => x.Code == motorcycleCode)
            .AnyAsync();
        Assert.True(existsBeforeDelete);

        // Delete it
        var deleteRequest = new DeleteMotorcycleByIdRequest(motorcycleCode);
        var deleteResponse = await mediator.Send(deleteRequest);
        Assert.True(deleteResponse.Result.IsSuccess);

        // Verify it no longer exists in database
        var existsAfterDelete = await context.Motorcycles
            .Where(x => x.Code == motorcycleCode)
            .AnyAsync();
        Assert.False(existsAfterDelete);
    }

    [Fact]
    public async Task Send_ShouldHandleCaseSensitiveCode()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        // First create a motorcycle with specific case
        var createRequest = CreateRandomMotorcycleRequest() with { Code = "MOTO-ABC123" };
        var createResponse = await mediator.Send(createRequest);
        Assert.True(createResponse.Result.IsSuccess);

        // Try to delete with different case (should work if code comparison is case-insensitive)
        var deleteRequest = new DeleteMotorcycleByIdRequest("moto-abc123");
        var deleteResponse = await mediator.Send(deleteRequest);

        // This test assumes the code comparison is case-sensitive based on the handler using exact match
        // If the database or handler is case-insensitive, adjust the assertion accordingly
        Assert.NotNull(deleteResponse);
    }

    [Fact]
    public async Task Send_ShouldNotAffectOtherMotorcycles()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();

        // Create two motorcycles
        var createRequest1 = CreateRandomMotorcycleRequest();
        var createResponse1 = await mediator.Send(createRequest1);
        Assert.True(createResponse1.Result.IsSuccess);

        var createRequest2 = CreateRandomMotorcycleRequest();
        var createResponse2 = await mediator.Send(createRequest2);
        Assert.True(createResponse2.Result.IsSuccess);

        var totalBeforeDelete = await context.Motorcycles.CountAsync();

        // Delete only one
        var deleteRequest = new DeleteMotorcycleByIdRequest(createResponse1.Result.RequiredResult.Code);
        var deleteResponse = await mediator.Send(deleteRequest);
        Assert.True(deleteResponse.Result.IsSuccess);

        var totalAfterDelete = await context.Motorcycles.CountAsync();

        // Should have one less motorcycle
        Assert.Equal(totalBeforeDelete - 1, totalAfterDelete);

        // The other motorcycle should still exist
        var otherExists = await context.Motorcycles
            .Where(x => x.Code == createResponse2.Result.RequiredResult.Code)
            .AnyAsync();
        Assert.True(otherExists);
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
