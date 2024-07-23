using System.Linq;
using UnityEngine;

namespace m039.Common
{
    public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        static protected T s_Instance = null;

        public static T Instance
        {
            get
            {
                if (!s_Instance)
                {
                    var temp = CreateInstance<T>() as ScriptableObjectSingleton<T>;

                    try
                    {
                        if (temp == null || !temp.UseResourceFolder)
                        {
                            s_Instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
                        }
                        else
                        {
                            s_Instance = Resources.Load<T>(temp.PathToResource);
                        }
                    }
                    finally
                    {
                        DestroyImmediate(temp);
                    }

                    if (s_Instance == null)
                    {
                        Log.Error<T>("Can't find or load resources for creating a singleton");
                    }
                }

                return s_Instance;
            }
        }

        protected virtual bool UseResourceFolder => false;

        protected virtual string PathToResource => null;

    }
}
