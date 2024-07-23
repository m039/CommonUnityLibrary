using System;
using System.Collections;
using UnityEngine;

namespace m039.Common
{
    public class Coroutines : DontDestroyMonoBehaviourSingleton<Coroutines>
    {
        static bool s_IsDestroyed;

        public static void Stop(IEnumerator coroutine)
        {
            if (s_IsDestroyed)
                return;

            Instance.StopCoroutine(coroutine);
        }

        public static void Stop(Coroutine coroutine)
        {
            if (s_IsDestroyed)
                return;

            Instance.StopCoroutine(coroutine);
        }

        public static Coroutine WaitForSeconds(float delay, Action callback)
        {
            if (s_IsDestroyed)
                return null;

            var routine = WaitCoroutine(new WaitForSeconds(delay), callback);
            return Instance.StartCoroutine(routine);
        }

        public static Coroutine WaitForSecondsRealtime(float delay, Action callback)
        {
            if (s_IsDestroyed)
                return null;

            var routine = WaitRoutine(new WaitForSecondsRealtime(delay), callback);
            return Instance.StartCoroutine(routine);
        }

        public static Coroutine RepeatEverySeconds(float repeatDelay, Action repeatCallback)
        {
            if (s_IsDestroyed)
                return null;

            var routine = RepeatRoutine(new WaitForSeconds(repeatDelay), repeatCallback);
            return Instance.StartCoroutine(routine);
        }

        public static Coroutine RepeatEverySecondsRealtime(float repeatDelay, Action repeatCallback)
        {
            if (s_IsDestroyed)
                return null;

            var routine = RepeatRoutine(new WaitForSecondsRealtime(repeatDelay), repeatCallback);
            return Instance.StartCoroutine(routine);
        }

        static IEnumerator WaitCoroutine(YieldInstruction yieldInstruction, Action callback)
        {
            yield return yieldInstruction;

            callback?.Invoke();
        }

        static IEnumerator WaitRoutine(IEnumerator enumerator, Action callback)
        {
            yield return enumerator;

            callback?.Invoke();
        }

        static IEnumerator RepeatRoutine(YieldInstruction yieldInstruction, Action repeatCallback)
        {
            while (true)
            {
                yield return yieldInstruction;

                repeatCallback?.Invoke();
            }
        }

        static IEnumerator RepeatRoutine(IEnumerator enumerator, Action repeatCallback)
        {
            while (true)
            {
                yield return enumerator;

                repeatCallback?.Invoke();
            }
        }

        protected override void DoDestroy()
        {
            base.DoDestroy();

            StopAllCoroutines();

            s_IsDestroyed = true;
        }
    }

}
