using CodeNotion.Template.Data.SqlServer;

namespace CodeNotion.Template.Business.Configurators;

public interface ICoreConfiguration : IDataConfiguration
{
    public string EmailConfigSender { get; }
    public string EmailConfigHost { get; }
    public int EmailConfigPort { get; }
    public string EmailConfigPassword { get; }
}