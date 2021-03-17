using UnityEngine;

namespace m039.Common
{

    public static class GizmosUtils
    {
        static public void DrawRect(Rect rect)
        {
            Gizmos.DrawWireCube(rect.center, rect.size);
        }

        static public void DrawRect(params Vector2[] points)
        {
            for (int i = 1; i < points.Length; i++)
            {
                Gizmos.DrawLine(points[i - 1], points[i]);
            }

            if (points.Length > 1)
            {
                Gizmos.DrawLine(points[points.Length - 1], points[0]);
            }
        }
    }

}
