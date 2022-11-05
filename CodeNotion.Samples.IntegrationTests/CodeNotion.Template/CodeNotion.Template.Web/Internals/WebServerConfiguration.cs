using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using CodeNotion.Template.Business.Configurators;
using CodeNotion.Template.Data.SqlServer;

namespace CodeNotion.Template.Web.Internals;

public class WebServerConfiguration : ICoreConfiguration
{
    public readonly IConfigurationRoot ConfigurationRoot;

    public WebServerConfiguration(IConfigurationRoot configurationRoot)
    {
        ConfigurationRoot = configurationRoot;

        ValidateSettings();
    }

    private void ValidateSettings()
    {
        // ReSharper disable once CollectionNeverUpdated.Local
        var nullables = new List<PropertyInfo>();

        foreach (var property in GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (nullables.Contains(property))
            {
                continue;
            }

            var propertyValue = property.GetValue(this);
            if (propertyValue == default)
            {
                throw new InvalidOperationException($"Missing configuration key [{property.Name}]. Check the system environment variable configuration. The current detected ASPNETCORE_ENVIRONMENT variable is [{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}]");
            }
        }
    }

    public string ConnectionString => ConfigurationRoot[nameof(ConnectionString)];
    public string EmailConfigSender => ConfigurationRoot.GetSection("EmailConfig")["Sender"];
    public string EmailConfigHost => ConfigurationRoot.GetSection("EmailConfig")["Host"];
    public int EmailConfigPort => int.Parse(ConfigurationRoot.GetSection("EmailConfig")["Port"]);
    public string EmailConfigPassword => ConfigurationRoot.GetSection("EmailConfig")["Password"];
}