using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Common
{

    /// <summary>
    /// Same as <see cref="SingletonMonoBehaviour", but an instance of the singleton is loaded from a Resources folder./>
    /// </summary>
    public class SingletonMonoBehaviourFromResources<T> : SingletonMonoBehaviour<T> where T : SingletonMonoBehaviourFromResources<T>
    {
        protected override T CreateInstance()
        {
            T instance = null;

            // Load a singleton from the Resources folder.
            if (UseResourceFolder && !string.IsNullOrEmpty(PathToResource))
            {
                var proxyObject = Resources.Load<GameObject>(PathToResource);
                if (proxyObject != null && proxyObject.GetComponent<T>() != null)
                {
                    instance = Instantiate(proxyObject).GetComponent<T>();
                    instance.gameObject.name = typeof(T).Name + " (Singleton [R])";
                }
            }

            return instance == null? base.CreateInstance() : instance;
        }

        protected virtual bool UseResourceFolder => false;

        protected virtual string PathToResource => null;

    }

}
