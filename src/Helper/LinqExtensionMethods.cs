using System;
using System.Collections.Generic;
using System.Linq;

namespace src.Helper
{
    public static class LinqExtensionMethods
    {
        public static bool None<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return !source.Any(predicate);
        }
    }
}