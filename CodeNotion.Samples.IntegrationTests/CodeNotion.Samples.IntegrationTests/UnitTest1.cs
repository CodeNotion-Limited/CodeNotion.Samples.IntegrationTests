using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CodeNotion.Template.Data.SqlServer.Models;
using Xunit;

namespace CodeNotion.Samples.IntegrationTests;

[Collection(SharedTestCollection.DefinitionName)]
public class UnitTest1 : IAsyncLifetime
{
    private readonly TestApiFactory _factory;

    public UnitTest1(TestApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Should_CreateEmptyMachine()
    {
        // Arrange & Act
        var result = await _factory.HttpClient.PutAsJsonAsync("api/machine", new Machine
        {
            Name = "Test",
        });

        // Assert
        result.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task Should_ThrowError_When_DeletingMachineWithLinkedVariant()
    {
        // Arrange & Act
        var result = await _factory.HttpClient.DeleteAsync($"api/machine/{27}");

        // Assert
        Assert.NotEqual(System.Net.HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task Should_ReturnOk_When_DeletingMachineAfterCreation()
    {
        // Arrange 
        var result = await _factory.HttpClient.PutAsJsonAsync("api/machine", new Machine
        {
            Name = "Test",
        });
        result.EnsureSuccessStatusCode();
        var response = await result.Content.ReadFromJsonAsync<Machine>();
        if (response is null)
        {
            throw new Exception("Response is null");
        }
        
        // Act
        result = await _factory.HttpClient.DeleteAsync($"api/machine/{response.Id}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
    }

    public Task InitializeAsync()
    {
        _factory.Migrate();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return _factory.ResetDatabaseAsync();
    }
}