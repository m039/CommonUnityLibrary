using UnityEngine;

namespace m039.Common
{

    public abstract class DontDestroyMonoBehaviourSingleton<T> : MonoBehaviourSingleton<T>
        where T: DontDestroyMonoBehaviourSingleton<T>
    {
        public new static T Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    var go = new GameObject(typeof(T).Name);
                    s_Instance = go.AddComponent<T>();
                }

                return s_Instance;
            }
        }

        protected override void DoAwake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }

}
