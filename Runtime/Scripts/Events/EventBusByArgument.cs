using System;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Common
{
    public class EventBusByArgument
    {
        static public Lazy<EventBusByArgument> s_Global = new(() => new EventBusByArgument("Global"));

        public static EventBusByArgument Global => s_Global.Value;

        public readonly Log.ILogger Logger;

        readonly Dictionary<Type, EventSubscriberList<object>> _subscribers = new();

        public EventBusByArgument() : this(null)
        {
        }

        public EventBusByArgument(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                Logger = Log.Get(typeof(EventBusByArgument));
            }
            else
            {
                Logger = Log.Get(nameof(EventBusByArgument) + " (" + name + ")");
            }

#if M039_COMMON_VERBOSE
            Logger.SetEnabled(true);
#else
            Logger.SetEnabled(false);
#endif
        }

        public void Subscribe<T>(Action<T> subscriber) where T: struct
        {
            var type = typeof(T);

            if (!_subscribers.ContainsKey(type))
            {
                _subscribers[type] = new EventSubscriberList<object>();
            }
            _subscribers[type].Add(subscriber);
        }

        public void Unsubscribe<T>(Action<T> subscriber) where T : struct
        {
            var type = typeof(T);

            if (_subscribers.ContainsKey(type))
            {
                _subscribers[type].Remove(x => subscriber.Equals(x));

                if (_subscribers[type].List.Count <= 0)
                {
                    _subscribers.Remove(type);
                }
            }
        }

        public void Raise<T>(T argument) where T : struct
        {
            if (!_subscribers.ContainsKey(typeof(T)))
            {
                Logger.Warning($"Can't raise an event [{typeof(T).Name}], no subscribers.");
                return;
            }

            var cleanUpCount = 0;
            var subscribers = _subscribers[typeof(T)];
            subscribers.Executing = true;
            for (int i = 0; i < subscribers.List.Count; i++)
            {
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
                        ((Action<T>)subscriber.Target)(argument);
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
    }
}
