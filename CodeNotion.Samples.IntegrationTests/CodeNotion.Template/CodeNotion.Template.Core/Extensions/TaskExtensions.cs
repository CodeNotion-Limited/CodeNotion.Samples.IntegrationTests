using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable 8714

namespace CodeNotion.Template.Business.Extensions;

public static class TaskExtensions
{
    public static async Task<T2> Then<T1, T2>(this Task<T1> source, Func<T1, T2> continuation)
    {
        var result = await source;
        return continuation(result);
    }

    public static async Task<TDestination> Then<TSource, TDestination>(this Task<TSource> source, Func<TSource, Task<TDestination>> asyncContinuation)
    {
        var result = await source;
        return await asyncContinuation(result);
    }

    public static Task<Dictionary<TKey, TResult>> ParallelExecute<TKey, TResult>(this IEnumerable<TKey> source, Func<TKey, Task<TResult>> taskFactory)
        where TResult : class
    {
        return source.ParallelExecute(taskFactory, t =>
        {
            if (t.Exception != null)
            {
                throw t.Exception;
            }

            return t.Result;
        });
    }

    public static async Task<Dictionary<TKey, TResult>> ParallelExecute<TKey, TResult>(this IEnumerable<TKey> source, Func<TKey, Task<TResult>> taskFactory, Func<Task<(TKey, TResult)>, (TKey, TResult)> exceptionFunc)
        where TResult : class
    {
        async Task<(TKey, TResult)> TaskFunction(TKey key)
        {
            var actionResult = await taskFactory.Invoke(key);
            return (key, actionResult);
        }

        var parallelTasks = source.Select(TaskFunction).ToList();
        await Task.WhenAll(parallelTasks.AsParallel());

        return parallelTasks
            .Select(exceptionFunc)
            .ToDictionary(t => t.Item1, t => t.Item2);
    }
}