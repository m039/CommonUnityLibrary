using UnityEngine;

namespace m039.Common
{
    public abstract class MonoBehaviourSingleton<T> : CommonMonoBehaviour
        where T: MonoBehaviourSingleton<T>
    {
        protected static T s_Instance;

        public static T Instance => s_Instance;

        void Awake()
        {
            if (s_Instance == null)
            {
                s_Instance = (T)this;

                DoAwake();
            } else
            {
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            if (s_Instance == this)
            {
                s_Instance = null;
                DoDestroy();
            }
        }

        protected virtual void DoAwake()
        {
        }

        protected virtual void DoDestroy()
        {
        }
    }
}
