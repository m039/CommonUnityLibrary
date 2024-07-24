using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace m039.Common
{
    public interface ISubscriber
    {
    }

    public static class EventBus
    {
        static readonly Dictionary<Type, SubscriberList<ISubscriber>> s_Subscribers = new();

        static readonly Dictionary<Type, List<Type>> s_CachedSubscriberTypes = new();

        public static Log.ILogger Logger = Log.Get(typeof(EventBus)).SetEnabled(false);

        public static void Subscribe(ISubscriber subscriber)
        {
            var subscriberTypes = GetSubscriberTypes(subscriber);

            foreach (var t in subscriberTypes)
            {
                if (!s_Subscribers.ContainsKey(t))
                {
                    s_Subscribers[t] = new SubscriberList<ISubscriber>();
                }
                s_Subscribers[t].Add(subscriber);
            }
        }

        public static void Unsubscribe(ISubscriber subscriber)
        {
            var subscriberTypes = GetSubscriberTypes(subscriber);
            foreach (var t in subscriberTypes)
            {
                if (s_Subscribers.ContainsKey(t))
                {
                    s_Subscribers[t].Remove(subscriber);
                }
            }
        }

        public static void Raise<T>(Action<T> action) where T : ISubscriber
        {
            if (!s_Subscribers.ContainsKey(typeof(T)))
            {
                Logger.Warning($"Can't raise an event [{typeof(T).Name}], no subscribers.");
                return;
            }

            var cleanUpCount = 0;
            var subscribers = s_Subscribers[typeof(T)];
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

        static List<Type> GetSubscriberTypes(ISubscriber subscriber)
        {
            var type = subscriber.GetType();
            if (s_CachedSubscriberTypes.ContainsKey(type))
            {
                return s_CachedSubscriberTypes[type];
            }

            var subscriberTypes = type
                .GetInterfaces()
                .Where(it => typeof(ISubscriber).IsAssignableFrom(it) && it != typeof(ISubscriber))
                .ToList();

            s_CachedSubscriberTypes[type] = subscriberTypes;

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
                    List.RemoveAt(i);
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
