using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace m039.Common
{
    public static class AnimationUtils
    {
        public static IEnumerator AnimateAlpha(
            Image image,
            float startValue,
            float endValue,
            float duration,
            EasingFunction.Ease ease = EasingFunction.Ease.Linear,
            bool useUnscaledTime = false
            )
        {
            if (image == null)
                return null;

            return Animate(startValue, endValue, duration, (v) => image.color = image.color.WithAlpha(v), ease, useUnscaledTime);
        }

        public static IEnumerator AnimateAlpha(
            CanvasGroup canvsGroup,
            float startValue,
            float endValue,
            float duration,
            EasingFunction.Ease ease = EasingFunction.Ease.Linear,
            bool useUnscaledTime = false
            )
        {
            if (canvsGroup == null)
                return null;

            return Animate(startValue, endValue, duration, (v) => canvsGroup.alpha = v, ease, useUnscaledTime);
        }

        public static IEnumerator Animate(
            float startValue,
            float endValue,
            float duration,
            System.Action<float> setter,
            EasingFunction.Ease ease = EasingFunction.Ease.Linear,
            bool useUnscaledTime = false
            )
        {
            if (setter != null)
            {
                var easingFunction = EasingFunction.GetEasingFunction(ease);

                setter(startValue);

                var t = 0f;

                while (t < duration)
                {
                    yield return null;

                    t += useUnscaledTime? Time.unscaledDeltaTime : Time.deltaTime;

                    setter(easingFunction(startValue, endValue, t / duration));
                }

                setter(endValue);
            }
        }
    }
}
