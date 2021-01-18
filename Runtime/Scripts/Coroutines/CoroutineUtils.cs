using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Common
{
    public static class CoroutineUtils
    {
        // 
        // Warning:
        //
        // This function spawns coroutines and you can stop them only by <see cref="MonoBehaviour.StopAllCoroutines"/>.
        // It looks like there is no other option to execute coroutines parallel.
        //
        static public IEnumerator DoParallel(MonoBehaviour behaviour, params IEnumerator[] sequence)
        {
            if (sequence != null && behaviour != null) {
                var coroutines = new List<Coroutine>();

                foreach (var s in sequence)
                {
                    if (s != null)
                    {
                        coroutines.Add(behaviour.StartCoroutine(s));
                    }
                }

                foreach (var c in coroutines)
                {
                    yield return c;
                }
            }
        }
    }
}
