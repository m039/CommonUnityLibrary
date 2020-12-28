using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Common
{

    public static class CommonExt
    {
        public static string Decorate(this string str)
        {
            return $"<<< {str} >>>";
        }

        public static void Times(this int count, System.Action<int> action)
        {
            if (count > 0 && action != null)
            {
                for (int i = 0; i < count; i++)
                {
                    action(i);
                }
            }
        }

        public static void Times(this int count, System.Action action)
        {
            if (count > 0 && action != null)
            {
                for (int i = 0; i < count; i++)
                {
                    action();
                }
            }
        }
    }

}
