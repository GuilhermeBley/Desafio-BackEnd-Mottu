using Bl.Mottu.Maintenance.Core.Commands.CreateDeliveryDriver;
using Smartec.Web.Infrastructure.Tests.TestBase;

namespace Bl.Mottu.Maintenance.Infrastructure.Tests.Tests;

[Collection(InfrastructureTestFixtureCollection.CollectionName)]
public class CreateDeliveryDriverTest
{
    private readonly InfrastructureTestFixture _fixture;

    public CreateDeliveryDriverTest(InfrastructureTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Send_ShouldCreateNewDeliveryDriver()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var request = CreateRandom();
        var response = await mediator.Send(request);

        Assert.NotNull(response);
        Assert.True(response.Result.IsSuccess);
    }

    [Fact]
    public async Task Send_ShouldMatchCategory()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var request = CreateRandom();
        var response = await mediator.Send(request with
        {
            CnhCategory = "ab"
        });

        Assert.Equal("AB", response.Result.RequiredResult.CnhCategory);
    }

    [Fact]
    public async Task Send_ShouldMatchName()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var request = CreateRandom();
        var response = await mediator.Send(request with
        {
            Name = "Name"
        });

        Assert.Equal("NAME", response.Result.RequiredResult.Name);
    }

    [Fact]
    public async Task Send_ShouldMatchCode()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var request = CreateRandom();
        var response = await mediator.Send(request with
        {
            Code = " RandomCode-123 "
        });

        Assert.Equal("RANDOMCODE-123", response.Result.RequiredResult.Code);
    }

    [Fact]
    public async Task Send_ShouldMatchCnh()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var request = CreateRandom();
        var response = await mediator.Send(request with
        {
            CnhNumber = "8716619205-0"
        });

        Assert.Equal("87166192050", response.Result.RequiredResult.CnhNumber);
    }

    [Fact]
    public async Task Send_ShouldMatchCnpj()
    {
        await using var scope = _fixture.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var request = CreateRandom();
        var response = await mediator.Send(request with
        {
            Cnpj = "90.307.814/0001-66"
        });

        Assert.Equal("90307814000166", response.Result.RequiredResult.Cnpj);
    }

    private static CreateDeliveryDriverRequest CreateRandom()
    {
        return new CreateDeliveryDriverRequest(
            Code: $"{Guid.NewGuid()}",
            Name: $"NAME",
            Cnpj: $"{Random.Shared.NextInt64(10_000_000_0000_00, 99_999_999_9999_99)}",
            BirthDate: new DateOnly(2000, 1, 1),
            CnhNumber: $"{Random.Shared.NextInt64(10_000_000_000, 99_999_999_999)}",
            CnhCategory: "AB");
    }
}
