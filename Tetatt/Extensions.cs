using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tetatt
{
    static class Extensions
    {
        public static void RemoveAll<T>(this List<T> list, Predicate<T> predicate)
        {
            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (predicate(list[i]))
                    list.RemoveAt(i);
            }
        }
    }
}
