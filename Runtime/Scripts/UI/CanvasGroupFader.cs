using m039.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Common
{

    public class CanvasGroupFader : MonoBehaviour
    {
        #region Inspector

#pragma warning disable 0649

        [SerializeField]
        CanvasGroup _CanvasGroup;

        [SerializeField]
        float _FadeOutDuration = 1.5f;

        [SerializeField]
        float _FadeInDuration = 1.5f;

        [CurveRange]
        [SerializeField]
        AnimationCurve _FadeOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [CurveRange]
        [SerializeField]
        AnimationCurve _FadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

#pragma warning restore 0649

        #endregion

        public bool IsFading { get; private set; }

        void Awake()
        {
            _CanvasGroup.gameObject.SetActive(false);    
        }

        IEnumerator Fade(float startAlpha, float finalAlpha, CanvasGroup canvasGroup, float duration, AnimationCurve curve)
        {
            var previousBlocksRaycasts = canvasGroup.blocksRaycasts;

            IsFading = true;
            canvasGroup.blocksRaycasts = true;

            yield return null;

            var time = 0f;
            var deltaAlpha = finalAlpha - startAlpha;

            while (time < duration)
            {
                canvasGroup.alpha = startAlpha + deltaAlpha * curve.Evaluate(time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = finalAlpha;
            canvasGroup.blocksRaycasts = previousBlocksRaycasts;
            IsFading = false;
        }

        /// <summary>
        /// Appear the canvas group.
        /// </summary>
        public IEnumerator FadeOut(float startAlpha = float.NaN)
        {
            _CanvasGroup.gameObject.SetActive(true);
            yield return StartCoroutine(Fade(float.IsNaN(startAlpha)? _CanvasGroup.alpha : startAlpha, 1, _CanvasGroup, _FadeOutDuration, _FadeOutCurve));
        }

        /// <summary>
        /// Disappear the canvas group.
        /// </summary>
        public IEnumerator FadeIn(float startAlpha = float.NaN)
        {
            _CanvasGroup.gameObject.SetActive(true);
            yield return StartCoroutine(Fade(float.IsNaN(startAlpha) ? _CanvasGroup.alpha : startAlpha, 0, _CanvasGroup, _FadeInDuration, _FadeInCurve));
            _CanvasGroup.gameObject.SetActive(false);
        }

    }

}
