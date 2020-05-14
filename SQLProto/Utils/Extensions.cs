using System.Collections;
using System.Collections.Generic;

namespace SQLProto.Utils
{
    public static class Extensions
    {
        public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T> obj)
        {
            return obj ?? new T[0];
        }
    }
}