using Microsoft.EntityFrameworkCore;

namespace CodeNotion.Template.Data.SqlServer.Persistence;

public class CodeNotionTemplateContext : DbContext
{
    public CodeNotionTemplateContext(DbContextOptions<CodeNotionTemplateContext> options) : base(options)
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        Database.SetCommandTimeout(120);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CodeNotionTemplateContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}