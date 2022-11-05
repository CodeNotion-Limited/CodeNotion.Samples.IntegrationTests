using CodeNotion.Template.Data.SqlServer.Models;
using Microsoft.EntityFrameworkCore;
using CodeNotion.Template.Data.SqlServer.Persistence;
using CodeNotion.Template.Tests.Scenarios;

namespace CodeNotion.Template.Migrator;

public class MigratingContext : FullCodeNotionTemplateContext
{
    public MigratingContext(DbContextOptions<CodeNotionTemplateContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var seeing = new SerialDataSet();
        seeing.Seed(modelBuilder);
    }
}