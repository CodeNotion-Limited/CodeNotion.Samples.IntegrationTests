using System.Collections.Generic;

namespace CodeNotion.Template.Business.Extensions;

public static class HashSetExtensions
{
    public static HashSet<T> AddRange<T>(this HashSet<T> source, IEnumerable<T> elements)
    {
        foreach (var element in elements)
        {
            source.Add(element);
        }

        return source;
    }
}