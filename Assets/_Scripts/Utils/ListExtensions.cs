using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Monads;

namespace _Scripts.Utils
{
    public static class ListExtensions
    {
        public static void Shuffle<T>(this IList<T> ts) {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i) {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }

        public static bool TryGetFirst<T>(this IList<T> list, Func<T, bool> predicate, out T match)
        {
            var first = list.FirstOrEmpty(predicate);
            return first.TryGetValue(out match);
        }
    }
}