using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Versioning;
using UnityEngine;

namespace m039.Common
{
    /// <summary>
    /// Inherit from this base class to create a singleton.
    /// e.g. public class MyClassName : SingletonMonoBehaviour<MyClassName> {}
    /// </summary>
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        private static T _sInstance;

        /// This flag is needed to prevent creating GameObjects when a scene is closed.
        private static bool _sIsDestroying; 

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

                    if (instance == null && !_sIsDestroying)
                    {
                        // Search for existing instance.
                        instance = (T)FindObjectOfType(typeof(T));

                        // Create new instance if one doesn't already exist.
                        if (instance == null)
                        {
                            // We need proxyObject and proxy only for virtual functions.
                            var proxyObject = new GameObject();
                            var proxy = proxyObject.AddComponent<T>();

                            if (proxy.ShouldCreateIfNotExist)
                            {
                                instance = proxy.CreateInstance();
                                instance.OnCreateInstance();

                                // Make instance persistent.
                                if (!proxy.ShouldDestroyOnLoad)
                                {
                                    DontDestroyOnLoad(instance.gameObject);
                                }
                            }

                            DestroyImmediate(proxyObject);
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

        protected virtual T CreateInstance()
        {
            var obj = new GameObject();
            var instance = obj.AddComponent<T>();

            obj.name = typeof(T).Name + " (Singleton)";

            return instance;
        }

        protected virtual void OnCreateInstance()
        {
        }

        protected virtual void OnDestroy()
        {
            if (ShouldDestroyOnLoad)
            {
                _sInstance = null;
            } else
            {
                _sIsDestroying = true;
            }
        }

        protected virtual bool ShouldDestroyOnLoad => true;

        protected virtual bool ShouldCreateIfNotExist => true;
    }

    public class SingletonMonoBehaviour<T, I> : SingletonMonoBehaviour<T> where T : SingletonMonoBehaviour<T, I>, I
    {
        public new static I Instance => SingletonMonoBehaviour<T>.Instance;
    }
}
