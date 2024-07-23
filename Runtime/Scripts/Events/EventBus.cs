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

        public static Log.ILogger Logger = Log.Get(typeof(EventBus));

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

            var subscribers = s_Subscribers[typeof(T)];
            subscribers.Executing = true;
            foreach (var subscriber in subscribers.List)
            {
                try
                {
                    action.Invoke((T)subscriber);
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
            subscribers.Executing = false;
            subscribers.CleanUp();
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

            public readonly List<T> List = new();

            readonly List<T> _wasRemoved = new();

            public void Add(T subscriber)
            {
                List.Add(subscriber);
            }

            public void Remove(T subscriber)
            {
                if (Executing)
                {
                    var i = List.IndexOf(subscriber);
                    if (i >= 0)
                    {
                        _needsCleanUp = true;
                        _wasRemoved.Add(subscriber);
                    }
                } else
                {
                    List.Remove(subscriber);
                }
            }

            public void CleanUp()
            {
                if (!_needsCleanUp)
                    return;

                foreach (var s in _wasRemoved)
                {
                    List.Remove(s);
                }

                _wasRemoved.Clear();
                _needsCleanUp = false;
            }
        }
    }
}
