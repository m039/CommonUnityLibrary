using UnityEngine;

namespace m039.Common
{
    [DefaultExecutionOrder(-1000)]
    public class InjectorMonoBehaviour : CommonMonoBehaviour
    {
        readonly Injector _injector = new();

        public Log.ILogger Logger => _injector.Logger;

        void Awake()
        {
            _injector.Inject();
        }

        public void ValidateDependencies()
        {
            _injector.ValidateDependencies();
        }

        public void ClearDependencies()
        {
            _injector.ClearDependencies();
        }
    }
}
