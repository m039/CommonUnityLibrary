using UnityEngine;

namespace m039.Common
{

    public static class UnityEngineExt
    {
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

        public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            if (x.HasValue)
            {
                vector.x = x.Value;
            }

            if (y.HasValue)
            {
                vector.y = y.Value;
            }

            if (z.HasValue)
            {
                vector.z = z.Value;
            }

            return vector;
        }

        public static string GetPath(this Transform current)
        {
            if (current == null)
                return "null";

            if (current.parent == null)
                return current.name;

            return current.parent.GetPath() + "/" + current.name;
        }
    }

}
