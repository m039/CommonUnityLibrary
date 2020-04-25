using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Common
{
    /// <summary>
    /// Inherit from this base class to create a singleton.
    /// e.g. public class MyClassName : Singleton<MyClassName> {}
    /// </summary>
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        private static T _sInstance;

        private static object _sLock = new object();

        /// <summary>
        /// Access singleton instance through this propriety.
        /// </summary>
        public static T Instance
        {
            get
            {
                T instance = _sInstance;

                lock (_sLock)
                {
                    if (!Application.isPlaying)
                    {
                        return null;
                    }

                    if (instance == null)
                    {
                        // Search for existing instance.
                        instance = (T)FindObjectOfType(typeof(T));

                        // Create new instance if one doesn't already exist.
                        if (instance == null)
                        {
                            // Need to create a new GameObject to attach the singleton to.
                            var singletonObject = new GameObject();
                            instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).ToString() + " (Singleton)";

                            if (instance.ShouldCreateIfNotExist)
                            {
                                // Make instance persistent.

                                if (!instance.ShouldDestroyOnLoad)
                                {
                                    DontDestroyOnLoad(singletonObject);
                                }

                            } else
                            {
                                DestroyImmediate(singletonObject);
                            }
                        }
                    }

                    if (instance != null)
                    {
                        _sInstance = instance;
                    }

                    return instance;
                }
            }
        }

        protected virtual void OnDestroy()
        {
            _sInstance = null;
        }

        public virtual bool ShouldDestroyOnLoad => true;

        public virtual bool ShouldCreateIfNotExist => true;
    }

    public class SingeltonMonoBehaviour<T, I> : SingletonMonoBehaviour<T> where T : SingeltonMonoBehaviour<T, I>, I
    {
        public new static I Instance => SingletonMonoBehaviour<T>.Instance;
    }
}
