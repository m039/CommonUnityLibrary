using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Common
{

    public class FPSDisplay : MonoBehaviour
    {
        const float UpdateInterval = 0.5f;

        const float UIReferenceHeight = 1920f;

        const float UIMargin = 100f;

        const float UICastShadowMargin = 4f;

        #region Inspector

        [Tooltip("Font size of the counter. This value is relative to full hd (1920 x 1080).")]
        [SerializeField]
        float _FontSize = 60;

        [SerializeField]
        Color _FontNormalLoadColor = new Color32(0x4f, 0xe7, 0x4c, 0xff);

        [SerializeField]
        Color _FontMediumLoadColor = new Color32(0xec, 0xf1, 0x4a, 0xff);

        [SerializeField]
        Color _FontHighLoadColor = new Color32(0xde, 0x62, 0x4f, 0xff);

        [SerializeField]
        Color _FontCastShadowColor = Color.black;

        #endregion

        GUIStyle _style;

        float _timeLeft;

        float _timeAccumulator;

        float _fps;

        int _frames;

        void Awake()
        {
            _style = new GUIStyle();
            _style.alignment = TextAnchor.UpperLeft;
            _style.normal.textColor = Color.blue;
            _style.font = Resources.Load<Font>("CommonUnityLibrary/MonospaceFont/RobotoMono-Regular");
        }

        void OnEnable()
        {
            ResetCounter();
            _fps = float.NaN;
        }

        void Update()
        {
            _timeLeft -= Time.unscaledDeltaTime;
            _timeAccumulator += 1 / Time.unscaledDeltaTime;
            _frames++;

            if (_timeLeft <= 0f)
            {
                _fps = _timeAccumulator / (float) _frames;
                ResetCounter();
            }
        }

        void ResetCounter()
        {
            _timeLeft = UpdateInterval;
            _timeAccumulator = 0f;
            _frames = 0;
        }

        void OnGUI()
        {
            var width = Screen.width;
            var height = Screen.height;
            var coeff = Screen.height / UIReferenceHeight;
            var rect = new Rect(UIMargin * coeff, UIMargin * coeff, width, height);

            _style.fontSize = (int) (_FontSize * coeff);

            string text;

            if (float.IsNaN(_fps))
            {
                text = "???? FPS (??? ms)";
            } else
            {
                text = string.Format("{0,4:0.} FPS ({1,3:0.0} ms)", _fps, 1f / _fps * 1000f);
            }

            // Draw a cast shadow.

            var castShadowRect = rect;

            _style.normal.textColor = _FontCastShadowColor;
            castShadowRect.center += Vector2.one * coeff * UICastShadowMargin;

            GUI.Label(castShadowRect, text, _style);

            // Draw the text.

            var color = _FontNormalLoadColor;

            if (_fps < 10)
            {
                color = _FontHighLoadColor;
            }
            else if (_fps < 30)
            {
                color = _FontMediumLoadColor;
            }

            _style.normal.textColor = color;

            GUI.Label(rect, text, _style);
        }
    }

}
