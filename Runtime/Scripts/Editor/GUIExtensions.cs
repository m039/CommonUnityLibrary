using UnityEngine;

namespace m039.Common
{
    public static class GUIExtensions
    {
        static GUIStyle _sRectStyle;

        static Texture2D _sRectTexture;

        static public void DrawRect(Rect position, Color color)
        {
            if (_sRectStyle == null || _sRectTexture == null)
            {
                _sRectTexture = new Texture2D(1, 1);
                _sRectStyle = new GUIStyle();

                _sRectStyle.normal.background = _sRectTexture;
            }

            _sRectTexture.SetPixel(0, 0, color);
            _sRectTexture.Apply();

            GUI.Box(position, GUIContent.none, _sRectStyle);
        }
    }
}
