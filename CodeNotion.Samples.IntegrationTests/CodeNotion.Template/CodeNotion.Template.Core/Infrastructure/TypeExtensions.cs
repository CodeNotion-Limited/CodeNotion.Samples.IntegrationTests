using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeNotion.Template.Business.Infrastructure;

public static class TypeExtensions
{
    public static bool IsInstantiableSubclassOf(this TypeInfo source, Type baseType) =>
        baseType.IsAssignableFrom(source) && !source.IsAbstract && !source.IsInterface;

    public static bool IsInstantiableSubclassOf<TBaseType>(this TypeInfo source) =>
        source.IsInstantiableSubclassOf(typeof(TBaseType));

    public static IEnumerable<TypeInfo> GetInstantiableSubtypesOf(this Assembly source, Type baseType) =>
        source.DefinedTypes.Where(t => t.IsInstantiableSubclassOf(baseType)).ToList();

    public static IEnumerable<TypeInfo> GetInstantiableSubTypesOf<TBaseType>(this Assembly source) =>
        source.GetInstantiableSubtypesOf(typeof(TBaseType));

    public static IEnumerable<TypeInfo> GetInstantiableSubtypesOf<TBaseType>(this AppDomain source) =>
        source.GetAssemblies().SelectMany(a => a.GetInstantiableSubtypesOf(typeof(TBaseType)));
}