using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Common
{

    public static class CommonUtils
    {
        static public void DestroyAllChildren(Transform transform)
        {
            if (transform == null)
            {
                return;
            }

            var toDestroy = new List<GameObject>();

            for (int i = 0; i < transform.childCount; i++)
            {
                toDestroy.Add(transform.GetChild(i).gameObject);
            }

            toDestroy.ForEach((obj) => GameObject.DestroyImmediate(obj));
        }

        static public bool IsNull(object o)
        {
            return o == null;
        }
    }

}
