using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CodeNotion.Template.Data.SqlServer.Models;
using Xunit;

namespace CodeNotion.Samples.IntegrationTests;

[Collection(SharedTestCollection.DefinitionName)]
public class UnitTest2
{
    private readonly HttpClient _client;

    public UnitTest2(TestApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Test1()
    {
        var result = await _client.PutAsJsonAsync("api/machine", new Machine
        {
            Name = "Test",
        });

        result.EnsureSuccessStatusCode()
            ;
        ;
    }
}