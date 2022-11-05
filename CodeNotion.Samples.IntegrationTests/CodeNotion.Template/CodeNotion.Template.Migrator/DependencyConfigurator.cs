using Microsoft.Extensions.DependencyInjection;
using CodeNotion.Template.Data.SqlServer;

namespace CodeNotion.Template.Migrator;

public static class DependencyConfigurator
{
    public static IServiceCollection AddMigrations(this IServiceCollection services, IDataConfiguration configuration, bool migrate = false)
    {
        if (!migrate)
        {
            return services;
        }

        MigrationService.Migrate(services, configuration);
        return services;
    }
}