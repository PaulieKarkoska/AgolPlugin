using System;
using System.Collections.Generic;

namespace AgolPlugin.Helpers
{
    internal static class CollectionHelper
    {
        public static List<List<T>> SplitList<T>(this List<T> list, int maxSize)
        {
            var final = new List<List<T>>();
            for (int i = 0; i < list.Count; i += maxSize)
                final.Add(list.GetRange(i, Math.Min(maxSize, list.Count - i)));

            return final;
        }
    }
}
