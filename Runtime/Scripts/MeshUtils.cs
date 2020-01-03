﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common
{

	public static class MeshUtils
	{
		public static Mesh CreateQuad()
		{
			return CreateQuad(1, 1, Color.white, new Vector2(0.0f, 0.0f));
		}

		public static Mesh CreateQuad(float width, float height, Color color, Vector2 position)
		{
			var mesh = new Mesh();

			var halfWidth = width / 2;
			var halfHeight = height / 2;

			mesh.vertices = new Vector3[] {
				new Vector3(position.x - halfWidth, position.y - halfHeight, 0),
				new Vector3(position.x + halfWidth, position.y - halfHeight, 0),
				new Vector3(position.x - halfWidth, position.y + halfHeight, 0),
				new Vector3(position.x + halfWidth, position.y + halfHeight, 0)
			};

			mesh.triangles = new int[] {
				// lower left triangle
				0, 2, 1,
				// upper right triangle
				2, 3, 1
			};

			mesh.normals = new Vector3[] {
				-Vector3.forward,
				-Vector3.forward,
				-Vector3.forward,
				-Vector3.forward
			};

			mesh.uv = new Vector2[] {
				new Vector2(0, 0),
				new Vector2(1, 0),
				new Vector2(0, 1),
				new Vector2(1, 1)
			};

			mesh.colors = new Color[]
			{
				color,
				color,
				color,
				color
			};

			return mesh;
		}

	}

}