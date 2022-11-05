using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeNotion.Template.Business.Extensions;

public static class TypeExtensions
{
    public static bool IsInstantiableSubclassOf(this TypeInfo source, Type baseType) =>
        baseType.IsAssignableFrom(source) && !source.IsAbstract && !source.IsInterface;

    public static bool IsInstantiableSubclassOf<TBaseType>(this TypeInfo source) =>
        source.IsInstantiableSubclassOf(typeof(TBaseType));

    public static IEnumerable<TypeInfo> GetInstantiableSubTypesOf(this IEnumerable<TypeInfo> source, Type baseType) =>
        source.Where(t => t.IsInstantiableSubclassOf(baseType));

    public static IEnumerable<TypeInfo> GetInstantiableSubTypesOf<TBaseType>(this IEnumerable<TypeInfo> source) =>
        source.Where(t => t.IsInstantiableSubclassOf(typeof(TBaseType)));

    public static IEnumerable<TypeInfo> GetInstantiableSubTypesOf(this Assembly source, Type baseType) =>
        source.DefinedTypes.GetInstantiableSubTypesOf(baseType);

    public static IEnumerable<TypeInfo> GetInstantiableSubTypesOf<TBaseType>(this Assembly source) =>
        source.GetInstantiableSubTypesOf(typeof(TBaseType));

    public static bool IsSimple(this Type type) =>
        type.IsPrimitive || type == typeof(string) || type == typeof(decimal) || type.IsEnum || type.IsValueType;
        
    public static bool HasAttribute<TAttribute>(this Type type) =>
        type.HasAttribute(typeof(TAttribute));
        
    public static bool HasAttribute(this Type type, Type attributeType) =>
        type.CustomAttributes.Any(a => a.AttributeType == attributeType);
}