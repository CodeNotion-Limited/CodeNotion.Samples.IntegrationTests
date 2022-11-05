using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CodeNotion.Template.Data.SqlServer.Models;
using Machine = CodeNotion.Template.Data.SqlServer.Models.Machine;

namespace CodeNotion.Template.Business.Cqrs.Query.Joins;

public static class JoinQueryableExtensions
{
    internal static readonly Dictionary<Enum, JoinDescriptor> Joins = new()
    {
        {MachineJoins.MachineToMeshes, new JoinDescriptor<Machine>(e => e.Include(x => x.Meshes))},
        {MachineJoins.MachineToAccessory, new JoinDescriptor<Machine>(e => e.Include(x => x.MachineAccessories))},
        {MeshGroupJoins.MeshGroupToMesh, new JoinDescriptor<MeshGroup>(e => e.Include(x => x.Meshes))},
        {MeshJoins.MeshToMachine, new JoinDescriptor<Mesh>(e => e.Include(x => x.Machine))},
        {MeshJoins.MeshToMeshGroup, new JoinDescriptor<Mesh>(e => e.Include(x => x.MeshGroup))},
        {ArticleJoins.NewArticleReplacementArticle, new JoinDescriptor<Article>(e => e.Include(x => x.NewArticleReplacements!).ThenInclude(replacement => replacement.NewArticle))},
        {ArticleJoins.ArticleToArticleParameter, new JoinDescriptor<Article>(e => e.Include(x => x.ArticleParameters))},
        {ArticleReplacementJoins.NewArticle, new JoinDescriptor<ArticleReplacement>(e => e.Include(x => x.NewArticle))},
        {MachineAccessoryJoins.MachineAccessoryToMachine, new JoinDescriptor<MachineAccessory>(e => e.Include(x => x.Machine))},
        {MachineAccessoryJoins.MachineAccessoryToAccessory, new JoinDescriptor<MachineAccessory>(e => e.Include(x => x.Accessory))},
        {AccessoryJoins.AccessoryToMachine, new JoinDescriptor<Accessory>(e => e.Include(x => x.MachineAccessories))},
        {OrderJoins.OrderToMesh, new JoinDescriptor<Order>(e => e.Include(x => x.OrderMeshes!).ThenInclude(x => x.Mesh))},
        {OrderJoins.OrderToArticleRelation, new JoinDescriptor<Order>(e => e.Include(x => x.OrderArticlesRelations!).ThenInclude(x => x.ArticleRelation).ThenInclude(x => x!.ArticleChild))},
        {OrderJoins.OrderToAccessory, new JoinDescriptor<Order>(e => e.Include(x => x.OrderAccessories!).ThenInclude(x => x.Accessory))},
        {OrderJoins.OrderToSerial, new JoinDescriptor<Order>(e => e.Include(x => x.Serial))},
        {OrderJoins.OrderToCustomer, new JoinDescriptor<Order>(e => e.Include(x => x.Customer))},
        {ArticleRelationJoins.ArticleRelationToChild, new JoinDescriptor<ArticleRelation>(e => e.Include(x => x.ArticleChild))},
        {ArticleRelationJoins.ArticleRelationToParent, new JoinDescriptor<ArticleRelation>(e => e.Include(x => x.ArticleParent))},
    };

    public static IQueryable<TEntity> IncludeJoins<TEntity>(this IQueryable<TEntity> source, IJoinableQuery query)
        where TEntity : class => IncludeJoins(source, query.QueryJoins);

    public static IQueryable<TEntity> IncludeJoins<TEntity>(this IQueryable<TEntity> source, Enum[]? joins)
        where TEntity : class
    {
        if (joins == null || joins.Length == 0)
        {
            return source;
        }

        var entityType = typeof(TEntity);
        foreach (var join in joins)
        {
            source = IncludeJoins(source, join, entityType);
        }

        return source;
    }

    private static IQueryable<TEntity> IncludeJoins<TEntity>(this IQueryable<TEntity> source, Enum join, Type entityType)
        where TEntity : class
    {
        if (!Joins.ContainsKey(join))
        {
            throw new NotSupportedException($"Enum {join} does not map to any Supported join. Please add the desired {nameof(JoinDescriptor)} to the {nameof(Joins)} dictionary.");
        }

        var joinDescriptor = Joins[join];
        if (joinDescriptor.Type != entityType)
        {
            throw new InvalidOperationException($"{nameof(joinDescriptor)} for join {join} {joinDescriptor.Type.Name} does not match current Queryable Entity {entityType.Name}.");
        }

        var descriptor = (JoinDescriptor<TEntity>) joinDescriptor;
        source = descriptor.IncludeExpression.Invoke(source);
        return source;
    }
}

internal abstract class JoinDescriptor
{
    public readonly Type Type;

    protected JoinDescriptor(Type type)
    {
        Type = type;
    }
}

internal class JoinDescriptor<TEntity> : JoinDescriptor
    where TEntity : class
{
    public readonly Func<IQueryable<TEntity>, IQueryable<TEntity>> IncludeExpression;

    public JoinDescriptor(Func<IQueryable<TEntity>, IQueryable<TEntity>> includeExpression) : base(typeof(TEntity))
    {
        IncludeExpression = includeExpression;
    }
}
