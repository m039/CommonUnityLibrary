using UnityEngine;

namespace m039.Common
{

    public static class UIUtils
    {
        public const float UIReferenceHeight = 1920;

        public static float UICoeff => Screen.height / UIReferenceHeight;

        public static float UIMediumMargin => 100f * UICoeff;

        public static float UISmallMargin => 32f * UICoeff;

        public enum FontCategory
        {
            SansSerif, Monospace
        }

        public static Font LoadFont(FontCategory category, FontStyle style)
        {
            var fontCategory = "SansSerif";
            var fontStyle = "Regular";
            var fontName = "Roboto";

            if (category == FontCategory.Monospace)
            {
                fontCategory = "Monospace";
                fontName = "RobotoMono";
            }

            if (style == FontStyle.Bold)
            {
                fontStyle = "Bold";
            }else if (style == FontStyle.BoldAndItalic)
            {
                fontStyle = "BoldItalic";
            } else if (style == FontStyle.Italic)
            {
                fontStyle = "Italic";
            }

            return Resources.Load<Font>(string.Format("CommonUnityLibrary/{0}Font/{1}-{2}", fontCategory, fontName, fontStyle));
        }
    
    }

}
