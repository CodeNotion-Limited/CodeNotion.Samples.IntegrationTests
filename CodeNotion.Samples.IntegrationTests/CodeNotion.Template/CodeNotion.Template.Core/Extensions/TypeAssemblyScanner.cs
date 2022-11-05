using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeNotion.Template.Business.Extensions;

public static class TypeAssemblyScanner
{
    private static readonly Dictionary<Assembly, TypeInfo[]> CachedTypes = new();
    private const string CurrentSolutionNamespace = "CodeNotion.Template";
    private static TypeInfo[]? _solutionTypes;

    public static TypeInfo[] GetTypesInAssembly(this Assembly assembly)
    {
        if (CachedTypes.ContainsKey(assembly))
        {
            return CachedTypes[assembly];
        }

        var types = assembly
            .DefinedTypes
            .Where(x => !x.IsAbstract)
            .Where(x => !x.IsInterface)
            .ToArray();

        CachedTypes[assembly] = types;
        return types;
    }

    public static TypeInfo[] GetTypesInAssembly<TRoot>()
    {
        var assembly = typeof(TRoot).Assembly;
        return assembly.GetTypesInAssembly();
    }

    public static IEnumerable<TypeInfo> GetTypesInSolution() =>
        _solutionTypes ??= AppDomain
            .CurrentDomain
            .GetAssemblies()
            .Where(x => x.FullName!.Contains(CurrentSolutionNamespace))
            .SelectMany(assembly => assembly.DefinedTypes)
            .ToArray();

    public static IEnumerable<Type> GetSubTypesInSolution<TType>() =>
        GetTypesInSolution()
            .Where(x => !x.IsAbstract)
            .Where(x => !x.IsInterface)
            .Where(x => typeof(TType).IsAssignableFrom(x));

    public static IEnumerable<Type> GetSubTypesInSolution(Type type) =>
        GetTypesInSolution()
            .Where(x => !x.IsAbstract)
            .Where(x => !x.IsInterface)
            .Where(type.IsAssignableFrom);
}