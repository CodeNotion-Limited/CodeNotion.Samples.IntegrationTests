using System;
using CodeNotion.Template.Data.SqlServer.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CodeNotion.Template.Data.SqlServer;

public static class ServiceProviderExtensions
{
    public static IServiceCollection AddConfiguratorData(this IServiceCollection services, IDataConfiguration configuration) => services
        .AddSingleton(configuration)
        .AddScoped(sp => new CodeNotionTemplateContext(ConfigureOptions(new DbContextOptionsBuilder<CodeNotionTemplateContext>(), configuration).Options))
        .AddScoped(sp => new FullCodeNotionTemplateContext(ConfigureOptions(new DbContextOptionsBuilder<CodeNotionTemplateContext>(), configuration).Options))
        .AddScoped(sp => BuildRoleBasedDbContext(sp, configuration));

    private static CodeNotionTemplateContext BuildRoleBasedDbContext(IServiceProvider sp, IDataConfiguration configuration)
    {
        var options = new DbContextOptionsBuilder<CodeNotionTemplateContext>();
        ConfigureOptions(options, configuration);
        return new FullCodeNotionTemplateContext(options.Options);
    }

    private static DbContextOptionsBuilder<TContext> ConfigureOptions<TContext>(DbContextOptionsBuilder<TContext> options, IDataConfiguration configuration)
        where TContext : DbContext
    {
        options
            .UseSqlServer(configuration.ConnectionString)
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging();

        return options;
    }
}