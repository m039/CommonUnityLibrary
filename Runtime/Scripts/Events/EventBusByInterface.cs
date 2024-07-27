using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace m039.Common.Events
{
    public interface IEventSubscriber
    {
    }

    public class EventBusByInterface
    {
        static public Lazy<EventBusByInterface> s_Global = new(() => new EventBusByInterface("Global"));

        public static EventBusByInterface Global => s_Global.Value;

        readonly Dictionary<Type, EventSubscriberList<IEventSubscriber>> _subscribers = new();

        readonly Dictionary<Type, List<Type>> _cachedSubscriberTypes = new();

        public readonly Log.ILogger Logger;

        public EventBusByInterface() : this(null) {
        }

        public EventBusByInterface(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Logger = Log.Get(typeof(EventBusByInterface));
            } else
            {
                Logger = Log.Get(nameof(EventBusByInterface) + " (" + name + ")");
            }

#if M039_COMMON_VERBOSE
            Logger.SetEnabled(true);
#else
            Logger.SetEnabled(false);
#endif
        }

        public void Subscribe(IEventSubscriber subscriber)
        {
            var subscriberTypes = GetSubscriberTypes(subscriber);

            foreach (var t in subscriberTypes)
            {
                if (!_subscribers.ContainsKey(t))
                {
                    _subscribers[t] = new EventSubscriberList<IEventSubscriber>();
                }
                _subscribers[t].Add(subscriber);
            }
        }

        public void Unsubscribe(IEventSubscriber subscriber)
        {
            var subscriberTypes = GetSubscriberTypes(subscriber);
            foreach (var t in subscriberTypes)
            {
                if (_subscribers.ContainsKey(t))
                {
                    _subscribers[t].Remove(subscriber);

                    if (_subscribers[t].List.Count <= 0)
                    {
                        _subscribers.Remove(t);
                    }
                }
            }
        }

        public void Raise<T>(Action<T> action) where T : IEventSubscriber
        {
            if (!_subscribers.ContainsKey(typeof(T)))
            {
                Logger.Warning($"Can't raise an event [{typeof(T).Name}], no subscribers.");
                return;
            }

            var cleanUpCount = 0;
            var subscribers = _subscribers[typeof(T)];
            subscribers.Executing = true;
            for (int i = 0; i < subscribers.List.Count; i++) {
                try
                {
                    var subscriber = subscribers.List[i];
                    if (subscriber.Target == null)
                    {
                        subscribers.RemoveAt(i);
                        cleanUpCount++;
                    }
                    else
                    {
                        action.Invoke((T)subscriber.Target);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }

            subscribers.Executing = false;
            subscribers.CleanUp();

            if (subscribers.List.Count <= 0)
            {
                _subscribers.Remove(typeof(T));
            }

            if (cleanUpCount > 0)
            {
                Logger.Info($"Cleaned up {cleanUpCount} references.");
            }
        }

        List<Type> GetSubscriberTypes(IEventSubscriber subscriber)
        {
            var type = subscriber.GetType();
            if (_cachedSubscriberTypes.ContainsKey(type))
            {
                return _cachedSubscriberTypes[type];
            }

            var subscriberTypes = type
                .GetInterfaces()
                .Where(it => typeof(IEventSubscriber).IsAssignableFrom(it) && it != typeof(IEventSubscriber))
                .ToList();

            _cachedSubscriberTypes[type] = subscriberTypes;

            return subscriberTypes;
        }
    }
}
