using UnityEngine;
using System.Collections;

namespace m039.Common
{

    public static class CameraUtils
    {
        public static Rect GetMainCameraScreenRect()
        {
            var size = new Vector2();

            size.y = Camera.main.pixelHeight;
            size.x = Camera.main.pixelWidth;

            return new Rect(Vector2.zero, size);
        }

        public static Rect GetMainCameraRect()
        {
            var size = new Vector2();

            size.y = Camera.main.orthographicSize * 2;
            size.x = size.y * Camera.main.aspect;

            return new Rect(Vector2.zero, size);
        }

        public static Bounds GetMainCameraBounds()
        {
            var size = new Vector3();

            size.y = Camera.main.orthographicSize * 2;
            size.x = size.y * Camera.main.aspect;

            return new Bounds
            (
                center: (Vector2)Camera.main.transform.position,
                size: size
            );
        }

    }

}
