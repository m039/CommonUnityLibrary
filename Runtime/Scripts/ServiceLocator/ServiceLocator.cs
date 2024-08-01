using System;
using System.Collections.Generic;

namespace m039.Common
{
    public class ServiceLocator
    {
        static public Lazy<ServiceLocator> s_Global = new(() => new ServiceLocator("Global"));

        public static ServiceLocator Global => s_Global.Value;

        readonly Dictionary<Type, Dictionary<int, object>> _services = new();

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
            return TryGet(0, out service);
        }

        public bool TryGet<T>(int key, out T service) where T : class
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var dict) && dict.ContainsKey(key))
            {
                service = dict[key] as T;
                return true;
            }

            service = null;
            return false;
        }

        public T Get<T>() where T : class
        {
            return Get<T>(0);
        }

        public T Get<T>(int key) where T : class
        {
            var type = typeof(T);
            if (_services.TryGetValue(type, out var dict) && dict.ContainsKey(key))
            {
                return dict[key] as T;
            }

            throw new ArgumentException($"ServiceLocator.Get: Service of type {type.FullName} with {key} key is not registered");
        }

        public ServiceLocator Register<T>(T service)
        {
            return Register(0, service);
        }

        public ServiceLocator Register<T>(int key, T service)
        {
            return Register(key, typeof(T), service);
        }

        public ServiceLocator Register(Type type, object service)
        {
            return Register(0, type, service);
        }

        public ServiceLocator Register(int key, Type type, object service)
        {
            if (service == null)
            {
                Logger.Error($"Can't register the service of type {type.FullName} with {key} key. It is null.");
                return this;
            }

            if (!type.IsInstanceOfType(service))
            {
                throw new ArgumentException("Type of service does not match type of service interface", nameof(service));
            }

            if (!_services.ContainsKey(type))
            {
                _services[type] = new();
            }

            if (!_services[type].TryAdd(key, service))
            {
                Logger.Error($"Register: Service of type {type.FullName} with {key} key is already registered");
            }

            return this;
        }

        public ServiceLocator Unregister<T>()
        {
            return Unregister<T>(0);
        }

        public ServiceLocator Unregister<T>(int key)
        {
            return Unregister(key, typeof(T));
        }

        public ServiceLocator Unregister(Type type)
        {
            return Unregister(0, type);
        }

        public ServiceLocator Unregister(int key, Type type)
        {
            if (!_services.ContainsKey(type) || !_services[type].Remove(key))
            {
                Logger.Error($"Unregister: Service of type {type.FullName} with {key} key is already unregistered");
                return this;
            }

            if (_services[type].Count <= 0)
            {
                _services.Remove(type);
            }

            return this;
        }
    }
}
