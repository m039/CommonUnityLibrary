using UnityEngine;

using static m039.Common.UIUtils;

namespace m039.Common
{

    public class FPSDisplay : MonoBehaviour
    {
        const float UpdateInterval = 0.5f;

        const float UICastShadowMargin = 4f;

        const float HighLoadFPS = 10f;

        const float MediumLoadFPS = 30f;

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

        [SerializeField]
        bool _ShowFrame = true;

        #endregion

        GUIStyle _labelStyle;

        GUIStyle _frameStyle;

        float _timeLeft;

        float _timeAccumulator;

        float _fps;

        int _frames;

        Texture2D _frameTexture;

        public float FPS => _fps;

        public bool Visibility { get; set; } = true;

        void Awake()
        {
            _labelStyle = new GUIStyle();
            _labelStyle.alignment = TextAnchor.UpperLeft;
            _labelStyle.normal.textColor = Color.blue;
            _labelStyle.font = LoadFont(FontCategory.Monospace, FontStyle.Normal);

            _frameTexture = new Texture2D(1, 1);
            _frameTexture.wrapMode = TextureWrapMode.Repeat;
            _frameTexture.SetPixel(0, 0, Color.black.WithAlpha(0.2f));
            _frameTexture.Apply();

            _frameStyle = new GUIStyle();
            _frameStyle.normal.background = _frameTexture;
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
            if (!Visibility)
                return;

            _labelStyle.fontSize = (int) (_FontSize * UICoeff);

            string text;

            if (float.IsNaN(_fps))
            {
                text = "???? FPS (??? ms)";
            } else
            {
                text = string.Format("{0,4:0.} FPS ({1,3:0.0} ms)", _fps, 1f / _fps * 1000f);
            }

            var textSize = _labelStyle.CalcSize(new GUIContent(text));
            var rect = new Rect(UIMediumMargin * UICoeff, UIMediumMargin * UICoeff, textSize.x, textSize.y);

            // Draw a frame.

            if (_ShowFrame)
            {
                var frameRect = new Rect(rect);
                
                frameRect.x -= UISmallPadding;
                frameRect.y -= UISmallPadding;
                frameRect.width += UISmallPadding * 2;
                frameRect.height += UISmallPadding * 2;

                GUI.Box(frameRect, _frameTexture, _frameStyle);
            }

            // Draw a cast shadow.

            var castShadowRect = rect;

            _labelStyle.normal.textColor = _FontCastShadowColor;
            castShadowRect.center += Vector2.one * UICoeff * UICastShadowMargin;

            GUI.Label(castShadowRect, text, _labelStyle);

            // Draw the text.

            var color = _FontNormalLoadColor;

            if (_fps < HighLoadFPS)
            {
                color = _FontHighLoadColor;
            }
            else if (_fps < MediumLoadFPS)
            {
                color = _FontMediumLoadColor;
            }

            _labelStyle.normal.textColor = color;

            GUI.Label(rect, text, _labelStyle);
        }
    }

}
