using System.Data.Common;
using System.Net.Http;
using System.Threading.Tasks;
using CodeNotion.Template.Data.SqlServer;
using CodeNotion.Template.Migrator;
using CodeNotion.Template.Web;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Respawn;
using Xunit;

namespace CodeNotion.Samples.IntegrationTests;

// TestContainer: https://www.youtube.com/watch?v=8IRNC7qZBmk
public class TestApiFactory : WebApplicationFactory<Startup>, IAsyncLifetime
{
    public HttpClient HttpClient { get; private set; } = null!;

    private DbConnection _connection = null!;
    private Respawner Respawner = null!;

    private readonly TestcontainersContainer _testContainersContainer = new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("mcr.microsoft.com/mssql/server:2019-latest")
        .WithEnvironment("ACCEPT_EULA", "Y")
        .WithEnvironment("SA_PASSWORD", "P@ssw0rd")
        .WithPortBinding(5555, 1433)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(1433))
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Integration");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IDataConfiguration));
            services.AddSingleton<IDataConfiguration, TestDataConfiguration>();
        });
    }

    public async Task InitializeAsync()
    {
        await _testContainersContainer.StartAsync();
        HttpClient = CreateClient();
        await HttpClient.GetAsync("/api/health"); // waiting for the server to start

        _connection = new SqlConnection(new TestDataConfiguration().ConnectionString);
        await _connection.OpenAsync();
        Respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.SqlServer,
            SchemasToInclude = new[] {"dbo"},
            WithReseed = true,
            // TablesToIgnore = new[] {new Table("__EFMigrationsHistory", "dbo")}
        });
    }
    
    public void Migrate()
    {
        MigrationService.Migrate(Services, new TestDataConfiguration());
    }

    public new async Task DisposeAsync()
    {
        await _testContainersContainer.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await Respawner.ResetAsync(_connection);
    }
}

internal class TestDataConfiguration : IDataConfiguration
{
    public string ConnectionString => "Server=localhost,5555;Database=CodeNotionTemplate;User Id=sa;Password=P@ssw0rd;TrustServerCertificate=True;Connection Timeout=30;";
}