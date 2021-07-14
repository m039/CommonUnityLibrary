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
                T instance = _sInstance;

                lock (_sLock)
                {
                    if (instance == null && !IsActiveSceneSkipped())
                    {
                        // Search for an existing instance.
                        instance = FindObjectOfType<T>();

                        // Create a new instance if no one exists.
                        if (instance == null)
                        {
                            var proxyObject = new GameObject();
                            var proxy = proxyObject.AddComponent<T>();

                            // We need proxyObject and proxy for virtual functions only.
                            try
                            {
                                if (proxy.ShouldCreateIfNotExist)
                                {
                                    instance = proxy.CreateInstance();
                                    instance.OnCreateInstance();

                                    // Make the instance persistent.
                                    if (!proxy.ShouldDestroyOnLoad)
                                    {
                                        DontDestroyOnLoad(instance.gameObject);
                                    }
                                }
                                else
                                {
                                    MarkActiveSceneAsSkipped();
                                }
                            }
                            finally
                            {
                                DestroyImmediate(proxyObject);
                            }
                        }
                    }

                    return _sInstance = instance;
                }
            }
        }

        static bool IsActiveSceneSkipped()
        {
            if (!Application.isPlaying)
                return false;

            var buildIndex = SceneManager.GetActiveScene().buildIndex;

            return _sScenesToSkip == null || buildIndex < 0? false : _sScenesToSkip[buildIndex];
        }

        static void MarkActiveSceneAsSkipped()
        {
            if (!Application.isPlaying)
                return;

            var buildIndex = SceneManager.GetActiveScene().buildIndex;

            if (_sScenesToSkip == null)
            {
                _sScenesToSkip = new BitArray(SceneManager.sceneCountInBuildSettings, false);
            }

            if (buildIndex >= 0) // A scene could not be in the build settings.
            {
                _sScenesToSkip[SceneManager.GetActiveScene().buildIndex] = true;
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
            }
        }

        protected virtual bool ShouldDestroyOnLoad => true;

        protected virtual bool ShouldCreateIfNotExist => Application.isPlaying; // Don't add new objects to the scene while the game is playing.
    }

    public class SingletonMonoBehaviour<T, I> : SingletonMonoBehaviour<T> where T : SingletonMonoBehaviour<T, I>, I
    {
        public new static I Instance => SingletonMonoBehaviour<T>.Instance;
    }
}
