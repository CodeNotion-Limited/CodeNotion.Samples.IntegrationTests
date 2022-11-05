using System.Threading.Tasks;
using CodeNotion.Template.Data.SqlServer;
using CodeNotion.Template.Web;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace CodeNotion.Samples.IntegrationTests;

// TestContainer: https://www.youtube.com/watch?v=8IRNC7qZBmk
public class TestApiFactory : WebApplicationFactory<Startup>, IAsyncLifetime
{
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
    }

    public new async Task DisposeAsync()
    {
        await _testContainersContainer.DisposeAsync();
    }
}

internal class TestDataConfiguration : IDataConfiguration
{
    public string ConnectionString => "Data Source=localhost:5555;Initial Catalog=CodeNotionTemplate;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
}