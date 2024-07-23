using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Common
{

    public static class CollectionExt
    {
        static public System.Nullable<T> Find<T>(this IList<T> list, System.Func<T, bool> predicate) where T : struct
        {
            if (list == null || predicate == null)
                return null;

            foreach (var item in list)
            {
                if (predicate(item))
                    return new System.Nullable<T>(item);
            }

            return null;
        }

        static public bool Contains<T>(this IList<T> list, T value)
        {
            if (list == null)
                return false;

            foreach (var item in list)
            {
                if (object.Equals(item, value))
                    return true;
            }

            return false;
        }

        static public bool Contains<T>(this IList<T> list, System.Func<T, bool> predicate)
        {
            if (list == null || predicate == null)
                return false;

            foreach (var item in list)
            {
                if (predicate(item))
                    return true;
            }

            return false;
        }

        public static void ForEach<T>(this IEnumerable<T> source, System.Action<T> action)
        {
            if (source == null || action == null)
                return;

            foreach (var item in source)
                action(item);
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            if (list == null)
                return;

            var n = list.Count;

            while (n > 1)
            {
                n--;
                var k = Random.Range(0, n + 1);

                if (n != k)
                {
                    T tmp = list[k];
                    list[k] = list[n];
                    list[n] = tmp;
                }
            }
        }

        public static void Fill<T>(this IList<T> list, T with)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = with;
            }
        }
    }

}
