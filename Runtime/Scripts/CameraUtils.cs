using UnityEngine;
using System.Collections;

namespace m039.Common
{

	public static class CameraUtils
	{

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