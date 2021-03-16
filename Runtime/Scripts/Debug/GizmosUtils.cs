using UnityEngine;

namespace m039.Common
{

    public static class GizmosUtils
    {
        static public void DrawRect(Rect rect)
        {
            Gizmos.DrawWireCube(rect.center, rect.size);
        }
    }

}
