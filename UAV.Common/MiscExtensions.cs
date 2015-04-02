using System;
using System.Collections.Generic;
using System.Linq;

namespace UAV.Common
{
    public static class MiscExtensions
    {
        public static IEnumerable<T> TakeLast<T>(this ICollection<T> source, int n)
        {
            return source.Skip(Math.Max(0, source.Count - n));
        }
    }
}

