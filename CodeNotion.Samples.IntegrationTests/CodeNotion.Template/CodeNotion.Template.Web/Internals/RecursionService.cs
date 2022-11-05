using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeNotion.Template.Data.SqlServer.Models.Interfaces;

namespace CodeNotion.Template.Web.Internals;

/// <summary>
/// Cleans recursive properties to avoid cartesian explosion on serialization
/// </summary>
public class RecursionService
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> OwnPropertyCache = new();

    public static IEnumerable<TEntity> CleanRecursiveProperties<TEntity>(IEnumerable<TEntity> items) =>
        items.Select(CleanRecursiveProperties);

    public static IEnumerable<TEntity> CleanRecursiveProperties<TEntity>(List<TEntity> items) =>
        items.Select(CleanRecursiveProperties);

    public static TEntity CleanRecursiveProperties<TEntity>(TEntity entity)
    {
        if (entity == null)
        {
            return entity;
        }

        var type = entity.GetType();
        CleanRecursiveChildProperties<TEntity>(type, entity);

        var relatedEntities = type.GetProperties().Where(x => typeof(IEntity).IsAssignableFrom(x.PropertyType));
        foreach (var relatedEntityProperty in relatedEntities)
        {
            var value = relatedEntityProperty.GetValue(entity);
            if (value is null)
            {
                continue;
            }

            CleanRecursiveChildProperties<TEntity>(relatedEntityProperty.PropertyType, value);
        }

        return entity;
    }

    private static PropertyInfo[] GetRecursiveProperties<TEntity>(Type type)
    {
        PropertyInfo[] props;
        if (!OwnPropertyCache.ContainsKey(type))
        {
            props = type
                .GetProperties()
                .Where(x => x.PropertyType == type || type.IsAssignableFrom(x.PropertyType) || typeof(IEnumerable<TEntity>).IsAssignableFrom(x.PropertyType))
                .ToArray();

            OwnPropertyCache.AddOrUpdate(type, _ => props, (_, _) => props);
            return props;
        }

        props = OwnPropertyCache[type];
        return props;
    }

    private static void CleanRecursiveChildProperties<TEntity>(Type childEntityType, object instance)
    {
        foreach (var recursiveProperty in GetRecursiveProperties<TEntity>(childEntityType))
        {
            recursiveProperty.SetValue(instance, null);
        }
    }
}