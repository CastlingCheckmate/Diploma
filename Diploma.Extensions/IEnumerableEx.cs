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

        public static IEnumerable<T> Unnulable<T>(this IEnumerable<T> collection, T singleValue)
        {
            return collection ?? Enumerable.Repeat(singleValue, 1);
        }

    }

}