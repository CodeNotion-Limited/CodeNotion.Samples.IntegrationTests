using System.Net.Mail;
using Microsoft.Extensions.DependencyInjection;
using CodeNotion.Template.Business.Configurators;
using CodeNotion.Template.Business.Cqrs;
using CodeNotion.Template.Business.Services;
using CodeNotion.Template.Data.SqlServer;
using CodeNotion.Odata;

namespace CodeNotion.Template.Business;

public static class ServiceProviderExtensions
{
    public static IServiceCollection AddBusiness(this IServiceCollection services, ICoreConfiguration config) => services
        .AddApplicationOData()
        .AddSingleton(config)
        .AddConfiguratorData(config)
        .AddSingleton(services)
        .AddHttpClient()
        .AddMemoryCache()
        .AddTransient<SmtpClient>()
        .AddTransient<IEmailService, EmailService>()
        .AddTransient<IOrderArticlesElaboratorService, OrderArticlesElaboratorService>()
        .AddSingleton<IBackgroundTaskQueue>(new BackgroundTaskQueue())
        .AddScoped<ArticleRelationService>()
        .AddCqrs();
}