using Microsoft.EntityFrameworkCore;

namespace CodeNotion.Template.Data.SqlServer.Persistence;

public class UserCodeNotionTemplateContext : CodeNotionTemplateContext
{
    private readonly int _identityUserId;

    public UserCodeNotionTemplateContext(DbContextOptions<CodeNotionTemplateContext> options, int identityUserId) : base(options)
    {
        _identityUserId = identityUserId;
    }
}