using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace m039.Common
{
    /// <summary>
    /// Inherit from this base class to create a singleton.
    /// 
    /// E.g. public class MyClassName : SingletonMonoBehaviour<MyClassName> {}
    /// </summary>
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
    {
        private static T _sInstance;

        /// This flag is needed to prevent creating GameObjects when a scene is closed.
        private static bool _sIsDestroying; 

        private static object _sLock = new object();

        /// <summary>
        /// Which scenes don't have this kind of singleton.
        /// To prevent some objects from constantly spawning.
        /// 
        /// This array is different for each subclass. <see cref="https://stackoverflow.com/a/49582829/675695"/>
        /// </summary>
        static BitArray _sScenesToSkip;

        /// <summary>
        /// Access a singleton instance through this propriety.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (!Application.isPlaying)
                {
                    return null; // Doesn't work with ExecuteInEditModeAttribute.
                }

                T instance = _sInstance;

                lock (_sLock)
                {
                    if (instance == null && !_sIsDestroying && !SkipCurrentScene())
                    {
                        // Search for an existing instance.
                        instance = (T)FindObjectOfType(typeof(T));

                        // Create a new instance if no one exists.
                        if (instance == null)
                        {
                            // We need proxyObject and proxy for virtual functions only.
                            var proxyObject = new GameObject();
                            var proxy = proxyObject.AddComponent<T>();

                            if (proxy.ShouldCreateIfNotExist)
                            {
                                instance = proxy.CreateInstance();
                                instance.OnCreateInstance();

                                // Make the instance persistent.
                                if (!proxy.ShouldDestroyOnLoad)
                                {
                                    DontDestroyOnLoad(instance.gameObject);
                                }
                            } else
                            {
                                MarkCurrentSceneAsSkipped();
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

        static bool SkipCurrentScene()
        {
            return _sScenesToSkip == null? false : _sScenesToSkip[SceneManager.GetActiveScene().buildIndex];
        }

        static void MarkCurrentSceneAsSkipped()
        {
            if (_sScenesToSkip == null)
            {
                _sScenesToSkip = new BitArray(SceneManager.sceneCountInBuildSettings, false);
            }

            _sScenesToSkip[SceneManager.GetActiveScene().buildIndex] = true;
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
