using System.Collections.Generic;
using UnityEngine;

namespace m039.Common.Pathfindig
{
    public interface IModifier
    {
        void Apply(Path p);
    }

    public class RaycastModifier : MonoBehaviour, IModifier
    {
        static List<Vector3> s_Buffer = new List<Vector3>();

        #region Inspector

        public bool thickRaycast;

        public float thickRaycastRadius;

        public LayerMask mask;

        public Vector3 raycastOffset = Vector3.zero;

        #endregion

        private void OnEnable()
        {
            // noop
        }

        private void OnDisable()
        {
            // noop
        }

        public void Apply(Path p)
        {
            if (!enabled)
                return;

            var points = p.vectorPath;

            if (ValidateLine(p.vectorPath[0], p.vectorPath[p.vectorPath.Count - 1]))
            {
                var s = p.vectorPath[0];
                var e = p.vectorPath[p.vectorPath.Count - 1];
                points.Clear();
                points.Add(s);
                points.Add(e);
            }
            else
            {
                points = ApplyGreedy(p, points);
            }

            p.vectorPath = points;
        }

        List<Vector3> ApplyGreedy(Path p, List<Vector3> points)
        {
            bool canBeOriginalNodes = points.Count == p.path.Count;
            int startIndex = 0;

            while (startIndex < points.Count)
            {
                Vector3 start = points[startIndex];
                s_Buffer.Add(start);

                int mn = 1, mx = 2;
                while (true)
                {
                    int endIndex = startIndex + mx;
                    if (endIndex >= points.Count)
                    {
                        mx = points.Count - startIndex;
                        break;
                    }
                    Vector3 end = points[endIndex];
                    if (!ValidateLine(start, end)) break;
                    mn = mx;
                    mx *= 2;
                }

                while (mn + 1 < mx)
                {
                    int mid = (mn + mx) / 2;
                    int endIndex = startIndex + mid;
                    Vector3 end = points[endIndex];

                    if (ValidateLine(start, end))
                    {
                        mn = mid;
                    }
                    else
                    {
                        mx = mid;
                    }
                }
                startIndex += mn;
            }

            Swap(ref s_Buffer, ref points);
            s_Buffer.Clear();
            return points;
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            T tmp = a;

            a = b;
            b = tmp;
        }

        bool ValidateLine(Vector3 v1, Vector3 v2)
        {
            if (thickRaycast && thickRaycastRadius > 0 && Physics2D.CircleCast(v1 + raycastOffset, thickRaycastRadius, v2 - v1, (v2 - v1).magnitude, mask)) {
                return false;
            }

            if (Physics2D.Linecast(v1 + raycastOffset, v2 + raycastOffset, mask))
            {
                return false;
            }

            return true;
        }
    }
}
