using System;
using CodeNotion.Template.Data.SqlServer.Persistence;

namespace CodeNotion.Template.Migrator;

public interface IMigrationScript
{
    void OnPreUp(CodeNotionTemplateContext context, IServiceProvider serviceProvider);
    void OnPostUp(CodeNotionTemplateContext context, IServiceProvider serviceProvider);
}

public interface IMigrationScript<TMigration> : IMigrationScript
{
}