using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace DoopExtensions.Extensions
{
    internal static class EnumberableExtensions
    {
        public static async Task AsyncParallelForEach<T>(this IEnumerable<T> source, Func<T, Task> body, int maxDegreeOfParallelism = DataflowBlockOptions.Unbounded)
        {
            // this can not be a default parameter as it has to be compile time constant
            if (maxDegreeOfParallelism == DataflowBlockOptions.Unbounded)
            {
                maxDegreeOfParallelism = Environment.ProcessorCount; // is this reasonable?
            }

            var tasks = new List<Task>();
            using var semaphore = new SemaphoreSlim(maxDegreeOfParallelism);

            foreach (var item in source)
            {
                await semaphore.WaitAsync();
                try
                {
                    await body(item);
                }
                finally
                {
                    semaphore.Release();
                }
            }

            await Task.WhenAll(tasks);
        }

        public static TOut GetCount<TOut>(this IEnumerable source) where TOut : struct, IComparable, IConvertible, IFormattable
        {
            var count = source.Cast<object>().Count();
            var maxValue = Convert.ChangeType(typeof(TOut).GetField("MaxValue").GetValue(null), typeof(TOut));

            if (Comparer<TOut>.Default.Compare((TOut)Convert.ChangeType(count, typeof(TOut)), (TOut)maxValue) > 0)
            {
                throw new OverflowException($"Count exceeds {typeof(TOut).Name}.MaxValue");
            }

            return (TOut)Convert.ChangeType(count, typeof(TOut));
        }

        public static T GetMostFrequentValue<T>(this IEnumerable<T> items)
        {
            var error = "Error in GetMostFrequentValue! ";
            if (items?.Any() != true)
            {
                error += $"Submitted enumerable is empty.";
                throw new ArgumentException(error);
            }

            if (items.Any(x => x == null))
            {
                error += $"Submitted enumerables contains null values.";
                throw new ArgumentNullException(error);
            }

            var values = items.GroupBy(i => i).ToDictionary(k => k.Key, v => v.Count());
            var highestFrequency = values.Max(i => i.Value);
            var mostCommon = values.Where(v => v.Value.Equals(highestFrequency)).Select(k => k.Key);

            if (mostCommon.Count() == 1)
            {
                return mostCommon.First();
            }

            // there are multiple most frequent, throw for now.
            error += $"Multiple values tie for most frequent at {highestFrequency} repetitions: {mostCommon}.";
            throw new IndexOutOfRangeException(error);
        }

        public static IEnumerable<IEnumerable<TIn>> ChunkBy<TIn>(this IEnumerable<TIn> source, int chunkSize)
        {
            while (source.Any())
            {
                yield return source.Take(chunkSize);
                source = source.Skip(chunkSize);
            }
        }

        public static IEnumerable<T> Denullify<T>(this IEnumerable<T?> source) where T : struct // no classes
        {
            foreach (var item in source)
            {
                if (item is T value)
                {
                    yield return value;
                }
            }
        }
    }
}