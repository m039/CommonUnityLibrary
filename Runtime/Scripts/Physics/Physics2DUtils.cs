using UnityEngine;

namespace GP4
{

    public static class Physics2DUtils
    {
        public static bool Within(Bounds bigger, Bounds smaller)
        {
            var leftP = smaller.center + Vector3.left * smaller.extents.x;
            var topP = smaller.center + Vector3.up * smaller.extents.y;
            var rightP = smaller.center + Vector3.right * smaller.extents.x;
            var bottomP = smaller.center + Vector3.down * smaller.extents.y;

            return bigger.Contains(leftP) ||
                bigger.Contains(topP) ||
                bigger.Contains(rightP) ||
                bigger.Contains(bottomP);
        }

        public static Rect ToRect(this Bounds bounds)
        {
            var center = bounds.center;
            var size = bounds.size;

            return new Rect(center.x - size.x / 2, center.y - size.y / 2, size.x, size.y);
        }
    }

}
