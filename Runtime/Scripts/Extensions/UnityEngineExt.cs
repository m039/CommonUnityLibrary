using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Common
{

    public static class UnityEngineExt
    {

        public static T GetComponentInParentRecursively<T>(this Component component)
        {
            T result;

            do
            {
                result = component.GetComponent<T>();
                component = component.transform.parent;
            } while (result == null && component != null);
           
            return result;
        }

        public static Color WithValue(this Color color, float value)
        {
            Color.RGBToHSV(color, out float h, out float s, out _);
            return Color.HSVToRGB(h, s, value);
        }

        public static Color WithAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }
    }

}
