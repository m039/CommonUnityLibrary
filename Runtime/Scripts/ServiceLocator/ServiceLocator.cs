using System;
using System.Collections.Generic;

namespace m039.Common
{
    public class ServiceLocator
    {
        static public Lazy<ServiceLocator> s_Global = new(() => new ServiceLocator("Global"));

        public static ServiceLocator Global => s_Global.Value;

        readonly Dictionary<Type, object> _services = new();

        public readonly Log.ILogger Logger;

        public ServiceLocator() : this(null)
        {
        }

        public ServiceLocator(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Logger = Log.Get(typeof(ServiceLocator));
            }
            else
            {
                Logger = Log.Get(nameof(ServiceLocator) + " (" + name + ")");
            }

#if M039_COMMON_VERBOSE
            Logger.SetEnabled(true);
#else
            Logger.SetEnabled(false);
#endif
        }

        public bool TryGet<T>(out T service) where T : class
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out object obj))
            {
                service = obj as T;
                return true;
            }

            service = null;
            return false;
        }

        public T Get<T>() where T : class
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out object obj))
            {
                return obj as T;
            }

            throw new ArgumentException($"ServiceLocator.Get: Service of type {type.FullName} not registered");
        }

        public ServiceLocator Register<T>(T service)
        {
            var type = typeof(T);
            if (!_services.TryAdd(type, service))
            {
                Logger.Error($"Register: Service of type {type.FullName} already registered");
            }

            return this;
        }

        public ServiceLocator Register(Type type, object service)
        {
            if (!type.IsInstanceOfType(service))
            {
                throw new ArgumentException("Type of service does not match type of service interface", nameof(service));
            }

            if (!_services.TryAdd(type, service))
            {
                Logger.Error($"Register: Service of type {type.FullName} already registered");
            }

            return this;
        }
    }
}
