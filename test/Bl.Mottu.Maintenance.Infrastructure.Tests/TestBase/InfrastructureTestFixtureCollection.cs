using Bl.Mottu.Maintenance.Infrastructure.Config;
using Bl.Mottu.Maintenance.Infrastructure.Extensions;
using Testcontainers.Azurite;
using Testcontainers.PostgreSql;

namespace Smartec.Web.Infrastructure.Tests.TestBase;

[CollectionDefinition(CollectionName)]
public class InfrastructureTestFixtureCollection : ICollectionFixture<InfrastructureTestFixture>
{
    public const string CollectionName = "InfrastructureTestCollection";
}

public class InfrastructureTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _sqlContainer;
    private readonly AzuriteContainer _azuriteContainer;
    public string ConnectionString => _sqlContainer.GetConnectionString();
    public IServiceProvider ServiceProvider { get; private set; } = null!;

    public InfrastructureTestFixture()
    {
        _sqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15-alpine")
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithCleanUp(true)
            .Build();

        _azuriteContainer = new AzuriteBuilder()
            .WithCleanUp(true)
            .Build();
    }

    public AsyncServiceScope CreateScope() => ServiceProvider.CreateAsyncScope();

    /// <summary>
    /// Dispose the fake infrastructure services. Don't call it the tests, it will be called in the end of the test collection.
    /// </summary>
    public async Task DisposeAsync()
    {
        try
        {
            await _sqlContainer.DisposeAsync();
        }
        catch { /* ignore */ }
        try
        {
            await _azuriteContainer.DisposeAsync();
        }
        catch { /* ignore */ }
    }

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();
        await _azuriteContainer.StartAsync();

        var services = new ServiceCollection();
        services.AddOptions<PostgreConfig>()
            .Configure(options =>
            {
                options.ConnectionString = _sqlContainer.GetConnectionString();
            });
        services.AddOptions<StorageAccountConfig>()
            .Configure(options =>
            {
                options.ConnectionString = _azuriteContainer.GetConnectionString();
            });
        services.AddInfrastructure();

        ServiceProvider = services.BuildServiceProvider();

        await Task.WhenAll(
            [SetupDataBaseAsync<Bl.Mottu.Maintenance.Core.Repository.DataContext>()]);
    }

    private async Task SetupDataBaseAsync<TDbContext>(CancellationToken cancellationToken = default) where TDbContext : DbContext
    {
        await using var scope = CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        await dbContext.Database.MigrateAsync(cancellationToken);
    }
}

