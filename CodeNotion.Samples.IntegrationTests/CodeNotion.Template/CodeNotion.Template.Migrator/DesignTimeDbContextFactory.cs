using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using CodeNotion.Template.Data.SqlServer.Persistence;

namespace CodeNotion.Template.Migrator;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MigratingContext>
{
    public MigratingContext CreateDbContext(string[] args)
    {
        var config = new MigrationsConfig();
        var optionBuilder = new DbContextOptionsBuilder<CodeNotionTemplateContext>().UseSqlServer(config.ConnectionString);
        return new MigratingContext(optionBuilder.Options);
    }
}

internal class MigrationsConfig
{
    public string ConnectionString => "Server=.;Database=db;Trusted_Connection=True";
}