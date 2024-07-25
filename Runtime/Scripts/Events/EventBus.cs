using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace m039.Common
{
    public interface ISubscriber
    {
    }

    public class EventBus
    {
        static public Lazy<EventBus> s_Global = new(() => new EventBus("Global"));

        public static EventBus Global => s_Global.Value;

        readonly Dictionary<Type, SubscriberList<ISubscriber>> _subscribers = new();

        readonly Dictionary<Type, List<Type>> _cachedSubscriberTypes = new();

        public readonly Log.ILogger Logger;

        public EventBus() : this(null) {
        }

        public EventBus(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Logger = Log.Get(typeof(EventBus));
            } else
            {
                Logger = Log.Get(nameof(EventBus) + " (" + name + ")");
            }

#if M039_COMMON_VERBOSE
            Logger.SetEnabled(true);
#else
            Logger.SetEnabled(false);
#endif
        }

        public void Subscribe(ISubscriber subscriber)
        {
            var subscriberTypes = GetSubscriberTypes(subscriber);

            foreach (var t in subscriberTypes)
            {
                if (!_subscribers.ContainsKey(t))
                {
                    _subscribers[t] = new SubscriberList<ISubscriber>();
                }
                _subscribers[t].Add(subscriber);
            }
        }

        public void Unsubscribe(ISubscriber subscriber)
        {
            var subscriberTypes = GetSubscriberTypes(subscriber);
            foreach (var t in subscriberTypes)
            {
                if (_subscribers.ContainsKey(t))
                {
                    _subscribers[t].Remove(subscriber);
                }
            }
        }

        public void Raise<T>(Action<T> action) where T : ISubscriber
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

            if (cleanUpCount > 0)
            {
                Logger.Info($"Cleaned up {cleanUpCount} references of [{typeof(T).Name}].");
            }
        }

        List<Type> GetSubscriberTypes(ISubscriber subscriber)
        {
            var type = subscriber.GetType();
            if (_cachedSubscriberTypes.ContainsKey(type))
            {
                return _cachedSubscriberTypes[type];
            }

            var subscriberTypes = type
                .GetInterfaces()
                .Where(it => typeof(ISubscriber).IsAssignableFrom(it) && it != typeof(ISubscriber))
                .ToList();

            _cachedSubscriberTypes[type] = subscriberTypes;

            return subscriberTypes;
        }

        class SubscriberList<T> where T : class
        {
            bool _needsCleanUp = false;

            public bool Executing;

            public readonly List<WeakReference> List = new();

            public void Add(T subscriber)
            {
                List.Add(new WeakReference(subscriber));
            }

            public void RemoveAt(int index)
            {
                if (Executing)
                {
                    _needsCleanUp = true;
                    List[index] = null;
                }
                else
                {
                    List.RemoveAt(index);
                }
            }

            public void Remove(T subscriber)
            {
                if (Executing)
                {
                    var i = List.FindIndex(x => x.Target == subscriber);
                    if (i >= 0)
                    {
                        _needsCleanUp = true;
                        List[i] = null;
                    }
                } else
                {
                    var i = List.FindIndex(x => x.Target == subscriber);
                    if (i >= 0)
                    {
                        List.RemoveAt(i);
                    }
                }
            }

            public void CleanUp()
            {
                if (!_needsCleanUp)
                    return;

                List.RemoveAll(x => x == null);
                _needsCleanUp = false;
            }
        }
    }
}
