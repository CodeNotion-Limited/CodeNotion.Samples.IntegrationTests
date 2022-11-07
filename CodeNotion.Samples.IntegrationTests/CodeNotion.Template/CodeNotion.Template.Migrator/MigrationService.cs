using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using CodeNotion.Template.Business.Extensions;
using CodeNotion.Template.Data.SqlServer;
using CodeNotion.Template.Data.SqlServer.Persistence;

namespace CodeNotion.Template.Migrator;

public class MigrationService
{
    public static void Migrate(IServiceCollection serviceCollection, IDataConfiguration config)
    {
        Migrate(serviceCollection.BuildServiceProvider(), config);
    }

    public static void Migrate(IServiceProvider serviceProvider, IDataConfiguration config)
    {
        using var scope = serviceProvider.CreateScope();
        var optionBuilder = new DbContextOptionsBuilder<CodeNotionTemplateContext>().UseSqlServer(config.ConnectionString);
        var context = new MigratingContext(optionBuilder.Options);

        Console.WriteLine("Discovering migrations...");
        var migrations = context.Database.GetPendingMigrations().ToArray();
        if (migrations.Length == 0)
        {
            Console.WriteLine("No migrations were found");
            return;
        }

        var migrationClasses = typeof(MigratingContext)
            .Assembly
            .DefinedTypes
            .Where(x => x.HasAttribute<MigrationAttribute>())
            .ToArray();

        Console.WriteLine("Applying migrations...");
        foreach (var migrationId in migrations)
        {
            ApplyMigration(migrationClasses, migrationId, context, scope.ServiceProvider);
        }

        Console.WriteLine("Migrations applied...");
    }

    private static void ApplyMigration(IEnumerable<TypeInfo> migrationClasses, string migrationId, MigratingContext context, IServiceProvider serviceProvider)
    {
        var migrationType = migrationClasses.SingleOrDefault(x => x.GetCustomAttribute<MigrationAttribute>()?.Id == migrationId);
        if (migrationType == null)
        {
            throw new InvalidOperationException($"Cannot find migration class with id {migrationId}");
        }

        var scriptTypeMarker = typeof(IMigrationScript<>).MakeGenericType(migrationType);
        var scripts = TypeAssemblyScanner.GetSubTypesInSolution<IMigrationScript>()
            .Where(t => t.GetInterfaces().Any(x => x == scriptTypeMarker))
            .Select(Activator.CreateInstance)
            .Cast<IMigrationScript>()
            .ToArray();

        try
        {
            Console.WriteLine($"Applying {migrationId}");
            foreach (var script in scripts)
            {
                script.OnPreUp(context, serviceProvider);
            }

            context.Database.GetInfrastructure().GetService<IMigrator>()!.Migrate(migrationId);

            foreach (var script in scripts)
            {
                script.OnPostUp(context, serviceProvider);
            }
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(e);
            Console.ResetColor();
            throw;
        }
    }
}