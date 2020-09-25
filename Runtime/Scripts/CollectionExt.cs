using System.Collections;
using System.Collections.Generic;

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

    }

}
