using System.Collections;
using UnityEngine;

namespace m039.Common
{
    public static class AnimationUtils
    {

        public static IEnumerator AnimateAlpha(
            CanvasGroup canvsGroup,
            float startValue,
            float endValue,
            float duration,
            EasingFunction.Ease ease = EasingFunction.Ease.Linear
            )
        {
            if (canvsGroup == null)
                return null;

            return Animate(startValue, endValue, duration, (v) => canvsGroup.alpha = v, ease);
        }

        public static IEnumerator Animate(
            float startValue,
            float endValue,
            float duration,
            System.Action<float> setter,
            EasingFunction.Ease ease = EasingFunction.Ease.Linear
            )
        {
            if (setter != null)
            {
                var easingFunction = EasingFunction.GetEasingFunction(ease);

                setter(startValue);

                var t = 0f;

                while (t < duration)
                {
                    t += Time.deltaTime;

                    setter(easingFunction(startValue, endValue, t / duration));

                    yield return null;
                }

                setter(endValue);
            }
        }
    }
}
