using Microsoft.EntityFrameworkCore;

namespace CodeNotion.Template.Data.SqlServer.Persistence;

public class FullCodeNotionTemplateContext : CodeNotionTemplateContext
{
    public FullCodeNotionTemplateContext(DbContextOptions<CodeNotionTemplateContext> options) : base(options)
    {
    }
}