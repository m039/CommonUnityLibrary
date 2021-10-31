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

        public static Vector3 WithX(this Vector3 vector, float x)
        {
            vector.x = x;
            return vector;
        }

        public static Vector3 WithY(this Vector3 vector, float y)
        {
            vector.y = y;
            return vector;
        }

        public static Vector3 WithZ(this Vector3 vector, float z)
        {
            vector.z = z;
            return vector;
        }

        public static string ToStringVerbose(this Vector3 vector, int precision = 3)
        {
            var formatString = "F" + precision;
            return string.Format($"({{0:{formatString}}}, {{1:{formatString}}}, {{2:{formatString}}})", vector.x, vector.y, vector.z);
        }

        public static void DestroyAllChildrenImmediate(this Transform tr)
        {
            if (tr == null)
                return;

            int count = tr.childCount;
            for (int i = count - 1; i >= 0; i--)
            {
                GameObject.DestroyImmediate(tr.GetChild(i).gameObject);
            }
        }
    }

}
