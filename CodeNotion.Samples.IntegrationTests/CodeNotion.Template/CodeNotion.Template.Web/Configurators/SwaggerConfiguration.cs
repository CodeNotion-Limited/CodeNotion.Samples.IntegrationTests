using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using CodeNotion.Template.Business.Cqrs.Query;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using OpenApiInfo = Microsoft.OpenApi.Models.OpenApiInfo;
using OpenApiOAuthFlow = Microsoft.OpenApi.Models.OpenApiOAuthFlow;
using OpenApiOAuthFlows = Microsoft.OpenApi.Models.OpenApiOAuthFlows;
using OpenApiOperation = Microsoft.OpenApi.Models.OpenApiOperation;
using OpenApiResponse = Microsoft.OpenApi.Models.OpenApiResponse;
using OpenApiSecurityRequirement = Microsoft.OpenApi.Models.OpenApiSecurityRequirement;
using OpenApiSecurityScheme = Microsoft.OpenApi.Models.OpenApiSecurityScheme;

namespace CodeNotion.Template.Web.Configurators;

public static class SwaggerConfiguration
{
    public const string ApiTitle = "Sicor CodeNotionTemplate Web API";
    public const string DocumentName = "Sicor CodeNotionTemplate Web API";
    public const string ApiVersion = "1.0.0";
    public const string ApiName = "api";
    public const string SecurityScheme = "oauth2";

    // required in order to fix the mentioned issue. Add it to .AddMvcCore(o => o.AddSwagger())
    public static MvcOptions AddSwagger(this MvcOptions source)
    {
        // Workaround: https://github.com/OData/WebApi/issues/1177
        foreach (var outputFormatter in source.OutputFormatters.OfType<ODataOutputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
        {
            outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
        }

        foreach (var inputFormatter in source.InputFormatters.OfType<ODataInputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
        {
            inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
        }

        return source;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection source, IConfigurationRoot configuration)
    {
        var tokenUrl = new Uri($"{configuration["ServerUrl"]}/api/user/authenticate");
        var scheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference {Id = SecurityScheme, Type = ReferenceType.SecurityScheme},
            Flows = new OpenApiOAuthFlows
            {
                Password = new OpenApiOAuthFlow
                {
                    Scopes = new Dictionary<string, string>
                    {
                        {ApiName, ApiTitle},
                    },
                    TokenUrl = tokenUrl,
                    RefreshUrl = tokenUrl
                }
            },
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.OAuth2,
            Scheme = "Bearer",
            Name = "Authentication",
            BearerFormat = "Bearer {token}"
        };

        return source.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(ApiVersion, new OpenApiInfo {Title = ApiTitle, Version = ApiVersion});
            options.DocumentFilter<SwaggerExcludeParametersFilter>();
            options.OperationFilter<AuthorizeCheckOperationFilter>(scheme);
            options.OperationFilter<SwaggerExcludeFilter>();
            options.OperationFilter<AddOdataParametersTypeFilter>();
            options.OperationFilter<SwaggerExcludeParameterTypeFilter<ODataQueryOptions>>();
            options.SchemaFilter<XEnumNamesSchemaFilter>();
            options.AddSecurityDefinition(SecurityScheme, scheme);
            options.CustomOperationIds(description => description.TryGetMethodInfo(out MethodInfo methodInfo) ? $"{methodInfo.DeclaringType!.Name.Replace("Controller", string.Empty)}_{methodInfo.Name}" : null);
        });
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    private sealed class AuthorizeCheckOperationFilter : IOperationFilter
    {
        private readonly OpenApiSecurityScheme _scheme;

        public AuthorizeCheckOperationFilter(OpenApiSecurityScheme scheme) =>
            _scheme = scheme;

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Responses.Add("401", new OpenApiResponse {Description = "Unauthorized"});
            operation.Responses.Add("403", new OpenApiResponse {Description = "Forbidden"});
            operation.Responses.Add("400", new OpenApiResponse {Description = "BadRequest"});

            operation.Security.Add(new OpenApiSecurityRequirement
            {
                {_scheme, new List<string> {ApiName}}
            });
        }
    }

    public static IApplicationBuilder UseApplicationSwagger(this IApplicationBuilder source)
    {
        source.UseSwagger(new Action<SwaggerOptions>(_ => { }));
        source.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint($"/swagger/{ApiVersion}/swagger.json", $"{ApiTitle} {ApiVersion}");
            c.DocExpansion(DocExpansion.None);
            c.ConfigObject.DisplayRequestDuration = true;
            c.DefaultModelExpandDepth(0);
            c.DefaultModelsExpandDepth(-1);
            c.DisplayOperationId();
        });

        // Activates filters before first server request
        source.ApplicationServices
            .GetRequiredService<ISwaggerProvider>()
            .GetSwagger(ApiVersion, null, "/");
        return source;
    }

    public class XEnumNamesSchemaFilter : ISchemaFilter
    {
        private const string Name = "x-enumNames";

        public void Apply(OpenApiSchema model, SchemaFilterContext context)
        {
            var typeInfo = context.Type;
            if (!typeInfo.IsEnum || model.Extensions.ContainsKey(Name))
            {
                return;
            }

            var names = Enum.GetNames(context.Type);
            var arr = new OpenApiArray();
            arr.AddRange(names.Select(name => new OpenApiString(name)));
            model.Extensions.Add(Name, arr);
        }
    }

    public class SwaggerExcludeParametersFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            // swaggerDoc.Paths contiene tutte le api dei vari controller sottoforma di dizionario path - oggetto
            var operations = swaggerDoc.Paths
                .Where(x => x.Value != null)
                .SelectMany(x => x.Value.Operations.Where(y => y.Value != null));

            foreach (var operation in operations)
            {
                // route.Operations contiene le API all'interno di un determinato controller sottoforma di tipo operazione - oggetto
                FilterParametersFromMethod(operation.Value);
            }
        }

        private static void FilterParametersFromMethod(OpenApiOperation operation)
        {
            // operation.Parameters contiene tutti i parametri di una determinata API
            for (var i = operation.Parameters.Count - 1; i >= 0; i--)
            {
                var parameter = operation.Parameters[i];
                if (parameter is null || parameter.Name != nameof(GetOdataQuery<object>.QueryJoins))
                {
                    continue;
                }

                operation.Parameters.RemoveAt(i);
            }
        }
    }

    private class SwaggerExcludeFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var ignoredParameters = context
                .ApiDescription
                .ParameterDescriptions
                .Where(pd =>
                {
                    var info = pd.ParameterInfo();
                    if (info == null)
                    {
                        return false;
                    }

                    return info
                        .CustomAttributes
                        .Any(x => x.AttributeType == typeof(SwaggerExcludeAttribute));
                });

            foreach (var ignoredParameter in ignoredParameters)
            {
                var paramToRemove = operation.Parameters.SingleOrDefault(x => x.Name == ignoredParameter.Name);
                if (paramToRemove != null)
                {
                    operation.Parameters.Remove(paramToRemove);
                }
            }
        }
    }

    private class AddOdataParametersTypeFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.ApiDescription.HttpMethod != "GET" || !context.ApiDescription.RelativePath!.EndsWith("/odata"))
            {
                return;
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "$count",
                In = ParameterLocation.Query,
                Description = "Defines if the total element count should be computed. ref: https://docs.microsoft.com/en-us/odata/concepts/queryoptions-overview#count",
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "boolean",
                    Nullable = true,
                    Default = new OpenApiBoolean(false)
                }
            });
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "$skip",
                In = ParameterLocation.Query,
                Description = "Defines how many elements to skip. ref: https://docs.microsoft.com/en-us/odata/concepts/queryoptions-overview#top-and-skip",
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "integer",
                    Nullable = true,
                    Default = new OpenApiInteger(0)
                }
            });
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "$top",
                In = ParameterLocation.Query,
                Description = "Defines how many elements to return. ref: https://docs.microsoft.com/en-us/odata/concepts/queryoptions-overview#top-and-skip",
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "integer",
                    Nullable = true,
                    Default = new OpenApiInteger(30)
                }
            });
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "$filter",
                In = ParameterLocation.Query,
                Description = "Defines the filtering expression. ref: https://docs.microsoft.com/en-us/odata/concepts/queryoptions-overview#filter",
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Nullable = true
                }
            });
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "$orderBy",
                In = ParameterLocation.Query,
                Description = "Defines the ordering expression. ref: https://docs.microsoft.com/en-us/odata/concepts/queryoptions-overview#orderby",
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Nullable = true
                }
            });
            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "$apply",
                In = ParameterLocation.Query,
                Description = "Defines the Aggregation behavior. ref: http://docs.oasis-open.org/odata/odata-data-aggregation-ext/v4.0/cs01/odata-data-aggregation-ext-v4.0-cs01.html#_Toc378326289",
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Nullable = true
                }
            });
        }
    }

    private class SwaggerExcludeParameterTypeFilter<T> : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var ignoredParameters = context
                .ApiDescription
                .ParameterDescriptions
                .Where(x => typeof(T).IsAssignableFrom(x.Type))
                .ToArray();

            foreach (var ignoredParameter in ignoredParameters)
            {
                context.ApiDescription.ParameterDescriptions.Remove(ignoredParameter);
                var paramToRemove = operation.Parameters.SingleOrDefault(x => x.Name == ignoredParameter.Name);
                if (paramToRemove != null)
                {
                    operation.Parameters.Remove(paramToRemove);
                }
            }
        }
    }
}

[AttributeUsage(AttributeTargets.Parameter)]
public class SwaggerExcludeAttribute : Attribute
{
}