using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Common
{

    public static class AnimatorUtils
    {

        /// <summary>
        /// This function returns a hash using <code>Animator.StringToHash()</code> and caches the result.
        /// </summary>
        /// <typeparam name="T">An enum type which name will be passed to <code>Animator.StringToHash()</code></typeparam>
        /// <param name="enumIndex">Index of the enum</param>
        /// <param name="cache">A cache</param>
        public static int EnumToHash<T>(int enumIndex, ref int[] cache) where T : System.Enum
        {
            if (cache == null)
            {
                var type = typeof(T);
                var names = System.Enum.GetNames(type);
                cache = new int[names.Length];
                for (int i = 0; i < cache.Length; i++)
                {
                    cache[i] = Animator.StringToHash(names[i]);
                }
            }

            return cache[enumIndex];
        }
    }

}
