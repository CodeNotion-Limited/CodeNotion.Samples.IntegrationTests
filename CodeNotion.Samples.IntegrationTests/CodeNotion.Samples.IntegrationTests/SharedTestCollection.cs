using System.ComponentModel;
using Xunit;

namespace CodeNotion.Samples.IntegrationTests;

[CollectionDefinition(DefinitionName)]
public class SharedTestCollection : ICollectionFixture<TestApiFactory>
{
    public const string DefinitionName = "WebApplication Shared Instance";
}