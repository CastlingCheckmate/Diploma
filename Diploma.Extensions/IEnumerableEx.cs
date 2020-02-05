using System;
using System.Linq;
using System.Collections.Generic;

namespace Diploma.Extensions
{

    public static class IEnumerableEx
    {

        public static IEnumerable<T> Unnulable<T>(this IEnumerable<T> collection)
        {
            return collection ?? Enumerable.Empty<T>();
        }

        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            return collection is null || !collection.Any();
        }

        public static bool IsNotEmpty<T>(this IEnumerable<T> collection)
        {
            return !IsEmpty(collection);
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            collection.ForEach((index, item) =>
            {
                action(item);
            });
        }

        public static void ForEach<T>(this IEnumerable<T> collection, Action<int, T> action)
        {
            if (collection is null)
            {
                return;
            }
            var index = 0;
            foreach (var item in collection)
            {
                action(index++, item);
            }
        }

    }

}