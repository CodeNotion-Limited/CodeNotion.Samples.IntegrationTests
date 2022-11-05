using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using CodeNotion.Template.Web.Configurators;
using CodeNotion.Template.Web.Internals;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using CodeNotion.Template.Business;
using CodeNotion.Template.Migrator;
using CodeNotion.Template.Web.Middlewares;

namespace CodeNotion.Template.Web;

public class Startup
{
    private readonly WebServerConfiguration _configuration;

    public Startup(IWebHostEnvironment env)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile(@"appsettings.json", false, true)
            .AddJsonFile($@"appsettings.{env.EnvironmentName}.json", true, true)
            .AddJsonFile($@"appsettings.Local.json", true, true)
            .AddEnvironmentVariables();

        _configuration = new WebServerConfiguration(builder.Build());
    }

    public void ConfigureServices(IServiceCollection services)
    {
        IdentityModelEventSource.ShowPII = true;
        services
            .AddSingleton(_configuration.ConfigurationRoot)
            .AddBusiness(_configuration)
            .AddHttpContextAccessor()
            .AddMvcCore(o =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                // o.Filters.Add(new AuthorizeFilter(policy)); // todo: add when adding auth
                o.EnableEndpointRouting = false;
                o.AddSwagger();
            })
            .AddApiExplorer()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            }).Services
            .AddNSwag()
            .AddSwagger(_configuration.ConfigurationRoot)
            .AddMigrations(_configuration, !NSwagConfiguration.IsOpenApiSchemeGenerationRuntime)
            .AddResponseEnvelope()
            .AddCors();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCors(cors =>
        {
            cors
                .WithOrigins("http://localhost:4206", "http://localhost:4207", "https://localhost:4206", "https://localhost:4207", "https://support.sicor-spa.it/")
                .AllowAnyHeader()
                .AllowCredentials()
                .AllowAnyMethod();
        });

        app.UseDeveloperExceptionPage();
        app.UseExceptionSerializerMiddleware();
        app.UseRouting();
        app.UseRewriter(new RewriteOptions().Add(Html5Redirect));
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        app.UseApplicationSwagger();
        app.UseMvc();
    }

    private static void Html5Redirect(RewriteContext rewriteContext)
    {
        var requestPath = rewriteContext.HttpContext.Request.Path;
        if (!ShouldNotRewriteUrl(requestPath))
        {
            rewriteContext.HttpContext.Request.Path = "/index.html";
        }
    }

    private static bool ShouldNotRewriteUrl(PathString requestPath) =>
        requestPath.StartsWithSegments("/api") ||
        Path.HasExtension(requestPath);

    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssZ"));
        }
    }
}