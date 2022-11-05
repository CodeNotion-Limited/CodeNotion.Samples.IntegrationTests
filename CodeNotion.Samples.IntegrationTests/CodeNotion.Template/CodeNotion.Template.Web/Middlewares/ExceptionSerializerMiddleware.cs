using System;
using System.Net;
using System.Threading.Tasks;
using CodeNotion.Template.Web.Middlewares.Base;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using CodeNotion.Template.Business.Enums;
using CodeNotion.Template.Business.Exceptions;

namespace CodeNotion.Template.Web.Middlewares;

public class ExceptionSerializerMiddleware : BaseMiddleware
{
    public ExceptionSerializerMiddleware(RequestDelegate next) : base(next)
    {
    }

    public override async Task Invoke(HttpContext context)
    {
        try
        {
            await Next.Invoke(context);
        }
        catch (Exception e)
        {
            switch (e)
            {
                case BadRequestException badRequestException:
                    await SerializeException(context, HttpStatusCode.BadRequest, badRequestException.ErrorType, e, badRequestException.Data);
                    break;
                case DbUpdateException dbUpdateException:
                    await SerializeException(context, HttpStatusCode.Conflict, ErrorType.CannotUpdateDatabase, dbUpdateException, dbUpdateException.InnerException?.Message);
                    break;
                case InvalidOperationException { HResult: -2146233079 } invalidOperationException:
                    await SerializeException(context, HttpStatusCode.NotFound, ErrorType.EntityNotFound, invalidOperationException);
                    break;
                default:
                    await SerializeException(context, HttpStatusCode.InternalServerError, ErrorType.NotHandled, e, "Errore non gestito durante l'esecuzione dell'operazione");
                    break;
            }
        }
    }

    private static Task SerializeException(HttpContext context, HttpStatusCode statusCode, ErrorType errorType, Exception exception, object? data = null, string? message = null)
    {
        var response = context.Response;
        response.StatusCode = (int)statusCode;
        response.ContentType = "application/json";

        var error = new ErrorDescription
        {
            ErrorType = errorType,
            Message = message ?? exception.Message,
            Data = data ?? exception.StackTrace
        };

        var settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        return context.Response.WriteAsync(JsonConvert.SerializeObject(error, settings));
    }
}

internal class ErrorDescription
{
    public ErrorType ErrorType { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
    public string ErrorTypeDescription => ErrorType.ToString().Humanize();
}

public static class ExceptionSerializerMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionSerializerMiddleware(this IApplicationBuilder source)
    {
        source.UseMiddleware<ExceptionSerializerMiddleware>();
        return source;
    }
}