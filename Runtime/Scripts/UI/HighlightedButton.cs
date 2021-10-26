using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace m039.Common
{
    /// <summary>
    /// The code is taken from here <see cref="https://answers.unity.com/questions/940456/how-to-change-text-color-on-hover-in-new-gui.html"/>
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class HighlightedButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        #region Inspector

        public Color normalColor = Color.white;
        public Color disabledColor = Color.white;
        public Color pressedColor = Color.white;
        public Color highlightedColor = Color.white;
        public float fadeDuration = 0.1f;

        #endregion

        TextMeshProUGUI _textMesh;

        Button _button;

        CrossFader _crossFader;

        protected HighlightedButton()
        {
            _crossFader = new CrossFader(this);
        }

        void Start()
        {
            Reset();
        }

        ButtonStatus _lastButtonStatus = ButtonStatus.Normal;
        bool _isHighlightDesired = false;
        bool _isPressedDesired = false;

        public void Reset()
        {
            _textMesh = GetComponentInChildren<TextMeshProUGUI>();
            _button = gameObject.GetComponent<Button>();

            _lastButtonStatus = ButtonStatus.Normal;
            _isHighlightDesired = false;
            _isPressedDesired = false;
            StartColorTween(normalColor, true);
        }

        void Update()
        {
            var desiredButtonStatus = ButtonStatus.Normal;
            var desiredColor = normalColor;

            if (!_button.interactable)
                desiredButtonStatus = ButtonStatus.Disabled;
            else
            {
                if (_isHighlightDesired)
                    desiredButtonStatus = ButtonStatus.Highlighted;

                if (_isPressedDesired)
                    desiredButtonStatus = ButtonStatus.Pressed;
            }

            if (desiredButtonStatus != _lastButtonStatus)
            {
                _lastButtonStatus = desiredButtonStatus;
                switch (_lastButtonStatus)
                {
                    case ButtonStatus.Normal:
                        desiredColor = normalColor;
                        break;
                    case ButtonStatus.Disabled:
                        desiredColor = disabledColor;
                        break;
                    case ButtonStatus.Pressed:
                        desiredColor = pressedColor;
                        break;
                    case ButtonStatus.Highlighted:
                        desiredColor = highlightedColor;
                        break;
                }
                StartColorTween(desiredColor, false);
            }
        }

        void StartColorTween(Color targetColor, bool instant)
        {
            if (_textMesh.color.Equals(targetColor))
            {
                _crossFader.Stop();
                return;
            }

            _crossFader.Start(instant ? 0f : fadeDuration, _textMesh.color, targetColor, true);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isHighlightDesired = true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isPressedDesired = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isPressedDesired = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _isHighlightDesired = false;
        }

        enum ButtonStatus
        {
            Normal,
            Disabled,
            Highlighted,
            Pressed
        }

        internal void SetTextColor(Color color)
        {
            _textMesh.color = color;
        }

        internal class CrossFader
        {
            HighlightedButton _parent;

            Coroutine _coroutine;

            public CrossFader(HighlightedButton parent)
            {
                _parent = parent;
            }

            void SetColor(Color color)
            {
                _parent.SetTextColor(color);
            }

            public void Start(float duration, Color startColor, Color targetColor, bool ignoreTimeScale)
            {
                IEnumerator Tween()
                {
                    var elapsedTime = 0f;

                    while (elapsedTime < duration)
                    {
                        elapsedTime += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                        var percentage = Mathf.Clamp01(elapsedTime / duration);
                        SetColor(Color.Lerp(startColor, targetColor, percentage));
                        yield return null;
                    }
                    SetColor(targetColor);
                }

                if (_coroutine != null)
                {
                    _parent.StopCoroutine(_coroutine);
                }

                if (duration <= 0.0f)
                {
                    SetColor(targetColor);
                }
                else
                {
                    _coroutine = _parent.StartCoroutine(Tween());
                }
            }

            public void Stop()
            {
                if (_coroutine != null)
                {
                    _parent.StopCoroutine(_coroutine);
                }

                _coroutine = null;
            }
        }
    }

}
