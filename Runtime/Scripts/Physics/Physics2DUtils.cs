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

            return Within(bigger, leftP, topP, rightP, bottomP);
        }

        public static bool CircleWithin(Bounds area, Vector2 position, float radius)
        {
            var topY = area.center.y + area.size.y / 2;
            var bottomY = area.center.y - area.size.y / 2;
            var leftX = area.center.x - area.size.x / 2;
            var rightX = area.center.x + area.size.x / 2;

            return position.x >= leftX - radius &&
                position.x <= rightX + radius &&
                position.y <= topY + radius &&
                position.y >= bottomY - radius;
        }

        public static bool Within(Bounds area, params Vector2[] points)
        {
            foreach (var p in points)
            {
                if (!area.Contains(p))
                    return false;
            }

            return true;
        }

        public static Rect ToRect(this Bounds bounds)
        {
            var center = bounds.center;
            var size = bounds.size;

            return new Rect(center.x - size.x / 2, center.y - size.y / 2, size.x, size.y);
        }
    }

}
