using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Common 
{

	public class MathUtils: MonoBehaviour 
	{ 

		public static float Max(float a1, float a2, float a3)
        {
            return Mathf.Max(Mathf.Max(a1, a2), a3);
        }

	}

}