using System;
using System.Linq;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using CodeNotion.Template.Business.Cqrs.Commands;
using CodeNotion.Template.Business.Cqrs.Query;
using CodeNotion.Template.Business.Extensions;
using CodeNotion.Template.Data.SqlServer.Models.Interfaces;
using CodeNotion.Template.Data.SqlServer.Persistence;

namespace CodeNotion.Template.Business.Cqrs;

public static class CqrsConfiguration
{
    public static IServiceCollection AddCqrs(this IServiceCollection services) => services
        .AddMediatR(typeof(CqrsConfiguration).Assembly)
        .RegisterEntityCommand(typeof(GetOdataQuery<>), typeof(GetOdataQueryHandler<>))
        .RegisterEntityCommand(typeof(GetEntityByIdQuery<>), typeof(GetEntityByIdQueryHandler<>))
        .RegisterEntityCommand(typeof(DeleteEntityByIdCommand<>), typeof(DeleteEntityByIdCommandHandler<>))
        .RegisterEntityCommand(typeof(UpsertEntityCommand<>), typeof(UpsertEntityCommandHandler<>));

    private static IServiceCollection RegisterEntityCommand(this IServiceCollection source, Type commandType, Type commandHandlerType, Func<Type, bool>? filter = null)
    {
        var iRequestHandlerType = typeof(IRequestHandler<,>);
        var types = TypeAssemblyScanner.GetTypesInAssembly<CodeNotionTemplateContext>();
        var entities = types.Where(x => typeof(IEntity).IsAssignableFrom(x));

        foreach (var entity in entities)
        {
            if (filter != null && !filter.Invoke(entity))
            {
                continue;
            }

            var command = commandType.MakeGenericType(entity.AsType());
            var resultType = command
                .GetInterfaces()
                .Single(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IRequest<>))
                .GetGenericArguments()[0];
            var requestHandler = iRequestHandlerType.MakeGenericType(command, resultType);
            var handler = commandHandlerType.MakeGenericType(entity.AsType());

            source.AddTransient(requestHandler, handler);
        }

        return source;
    }
}