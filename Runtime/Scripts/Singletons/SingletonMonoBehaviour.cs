using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Versioning;
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
                            // To use virtual functions of SingletonMonoBehaviour we need an instance of GameObject.
                            var singletonObject = new GameObject();
                            instance = singletonObject.AddComponent<T>();
                            singletonObject.name = typeof(T).ToString() + " (Singleton)";

                            if (instance.ShouldCreateIfNotExist)
                            {
                                // The signleton can be loaded from an asset stored in the Resources folder;
                                if (instance.UseResourceFolder && !string.IsNullOrEmpty(instance.PathToResource))
                                {
                                    var newSingletonObject = Resources.Load<GameObject>(instance.PathToResource);
                                    var newInstance = newSingletonObject == null ? null : newSingletonObject.GetComponent<T>();

                                    if (newSingletonObject != null && newInstance != null)
                                    {  
                                        newSingletonObject.name = singletonObject.name;
                                        DestroyImmediate(singletonObject);

                                        singletonObject = Instantiate(newSingletonObject);
                                        instance = newInstance;
                                    } else if (newSingletonObject != null)
                                    {
                                        DestroyImmediate(newSingletonObject);
                                    }
                                }

                                instance.OnSingletonCreated();

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

        /// <summary>
        /// This function is called when a singleton is created when no one exists.
        /// </summary>
        protected virtual void OnSingletonCreated()
        {
        }

        protected virtual void OnDestroy()
        {
            _sInstance = null;
        }

        protected virtual string PathToResource => null;

        protected virtual bool UseResourceFolder => false;

        protected virtual bool ShouldDestroyOnLoad => true;

        protected virtual bool ShouldCreateIfNotExist => true;
    }

    public class SingeltonMonoBehaviour<T, I> : SingletonMonoBehaviour<T> where T : SingeltonMonoBehaviour<T, I>, I
    {
        public new static I Instance => SingletonMonoBehaviour<T>.Instance;
    }
}
