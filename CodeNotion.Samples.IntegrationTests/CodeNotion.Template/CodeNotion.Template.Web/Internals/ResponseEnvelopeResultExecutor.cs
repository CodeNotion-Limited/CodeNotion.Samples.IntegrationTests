using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CodeNotion.Odata;

namespace CodeNotion.Template.Web.Internals;

internal class ResponseEnvelopeResultExecutor : ObjectResultExecutor
{
    public ResponseEnvelopeResultExecutor(OutputFormatterSelector formatterSelector, IHttpResponseStreamWriterFactory writerFactory, ILoggerFactory loggerFactory, IOptions<MvcOptions> mvcOptions)
        : base(formatterSelector, writerFactory, loggerFactory, mvcOptions)
    {
    }

    public override Task ExecuteAsync(ActionContext context, ObjectResult result)
    {
        switch (result.Value)
        {
            case IManagedPageResult:
                var dynamicObject = (dynamic)result.Value; // todo: find a better way to do this
                dynamicObject.Items = RecursionService.CleanRecursiveProperties(dynamicObject.Items);
                break;
        }

        return base.ExecuteAsync(context, result);
    }
}

public static class ResponseEnvelopeExtensions
{
    public static IServiceCollection AddResponseEnvelope(this IServiceCollection services) =>
        services.AddSingleton<IActionResultExecutor<ObjectResult>, ResponseEnvelopeResultExecutor>();
}