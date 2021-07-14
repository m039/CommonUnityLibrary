using System.Linq;
using UnityEngine;

namespace m039.Common
{

    public class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject {

        static T _Instance = null;

        public static T Instance
        {
            get
            {
                if (!_Instance)
                {
                    var temp = CreateInstance<T>() as SingletonScriptableObject<T>;

                    try
                    {
                        if (temp == null || !temp.UseResourceFolder)
                        {
                            _Instance = Resources.FindObjectsOfTypeAll<T>().FirstOrDefault();
                        }
                        else
                        {
                            _Instance = Resources.Load<T>(temp.PathToResource);
                        }
                    } finally
                    {
                        DestroyImmediate(temp);
                    }

                    if (_Instance == null)
                    {
                        Debug.LogError("Can't find or load resources for " + typeof(T).Name + " in the scene");
                    }
                }

                return _Instance;
            }
        }

        protected virtual bool UseResourceFolder => false;

        protected virtual string PathToResource => null;

    }

}
