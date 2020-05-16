using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SQLProto.Utils
{
    public static class Extensions
    {
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> obj)
        {
            return obj ?? new T[0];
        }

        public static IEnumerable<string> AndAlso(this IEnumerable<string> obj, params string[] toAdd)
        {
            if (obj == null)
                return toAdd;
            else
                return obj.Concat(toAdd);
        }
    }
}