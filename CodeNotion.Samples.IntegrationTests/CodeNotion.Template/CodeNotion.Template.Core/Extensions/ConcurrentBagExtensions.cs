using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CodeNotion.Template.Business.Extensions;

public static class ConcurrentBagExtensions
{
    public static ConcurrentBag<T> AddRange<T>(this ConcurrentBag<T> source, IEnumerable<T> elements)
    {
        foreach (var element in elements)
        {
            source.Add(element);
        }

        return source;
    }
}