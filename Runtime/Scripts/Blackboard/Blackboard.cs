using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace m039.Common.Blackboard
{
    [Serializable]
    public class BlackboardKey : IEquatable<BlackboardKey>
    {
        public readonly string name;

        readonly int _hashedKey;

        public BlackboardKey(string name)
        {
            this.name = name;
            _hashedKey = name.ComputeFNV1aHash();
        }

        public bool Equals(BlackboardKey other) => _hashedKey == other._hashedKey;

        public override bool Equals(object obj) => obj is BlackboardKey other && Equals(other);
        public override int GetHashCode() => _hashedKey;
        public override string ToString() => name;

        public static bool operator ==(BlackboardKey lhs, BlackboardKey rhs) => lhs._hashedKey == rhs._hashedKey;
        public static bool operator !=(BlackboardKey lhs, BlackboardKey rhs) => !(lhs == rhs);
    }

    public class BlackboardKey<T> : BlackboardKey
    {
        static readonly Dictionary<string, BlackboardKey<T>> s_Dict = new();

        public BlackboardKey(string name) : base(name)
        {
        }

        public static BlackboardKey<T> Get(BlackboardKey key)
        {
            return Get(key.name);
        }

        public static BlackboardKey<T> Get(string name)
        {
            if (s_Dict.ContainsKey(name))
            {
                return s_Dict[name];
            } else
            {
                return s_Dict[name] = new BlackboardKey<T>(name);
            }
        }
    }

    [Serializable]
    public class BlackboardEntry<T>
    {
        public BlackboardKey<T> Key { get; private set; }

        public T Value { get; set; }

        public override bool Equals(object obj) => obj is BlackboardEntry<T> other && other.Key == Key;

        public override int GetHashCode() => Key.GetHashCode();

        public override string ToString()
        {
            return Value.ToString();
        }

        static readonly Queue s_Cache = new();

        public static BlackboardEntry<T> Get(BlackboardKey<T> key)
        {
            BlackboardEntry<T> entry;
            if (s_Cache.Count > 0)
            {
                entry = (BlackboardEntry<T>)s_Cache.Dequeue();
            }
            else
            {
                entry = new BlackboardEntry<T>();
            }
            entry.Key = key;
            return entry;
        }

        public static void Release(BlackboardEntry<T> entry)
        {
            entry.Value = default;

            s_Cache.Enqueue(entry);
        }
    }

    [Serializable]
    public abstract class BlackboardBase
    {
        public abstract bool TryGetValue<T>(BlackboardKey<T> key, out T value);

        public abstract void SetValue<T>(BlackboardKey<T> key, T value);

        public abstract bool ContainsKey<T>(BlackboardKey<T> key);

        public abstract void Remove<T>(BlackboardKey<T> key);

        public abstract void Clear();

        public abstract int Count { get; }
    }

    [Serializable]
    public class Blackboard : BlackboardBase
    {
        static readonly object[] s_RemoveArgs = new object[1];

        static readonly Dictionary<Type, MethodInfo> s_RemoveMethods = new();

        readonly Dictionary<BlackboardKey, object> _entries = new();

        public Dictionary<BlackboardKey, object>.Enumerator GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        public void PrintDebug()
        {
            var sb = new StringBuilder();

            foreach (var entry in _entries)
            {
                var entryType = entry.Value.GetType();

                if (entryType.IsGenericType && entryType.GetGenericTypeDefinition() == typeof(BlackboardEntry<>))
                {
                    var valueProperty = entryType.GetProperty("Value");
                    if (valueProperty == null) continue;
                    var value = valueProperty.GetValue(entry.Value);
                    sb.AppendLine($"Key: {entry.Key}, Value: {value}");
                }
            }

            if (sb.Length > 0)
            {
                UnityEngine.Debug.Log(sb.ToString());
            } else
            {
                UnityEngine.Debug.Log("The blackboard is empty.");
            }
        }

        public T GetValueRaw<T>(BlackboardKey key, T @default)
        {
            return GetValue(BlackboardKey<T>.Get(key), @default);
        }

        public T GetValue<T>(BlackboardKey<T> key, T @default)
        {
            if (TryGetValue(key, out T v))
            {
                return v;
            }
            else
            {
                return @default;
            }
        }

        public bool TryGetValueRaw<T>(BlackboardKey key, out T value)
        {
            return TryGetValue(BlackboardKey<T>.Get(key), out value);
        }

        public override bool TryGetValue<T>(BlackboardKey<T> key, out T value)
        {
            if (_entries.TryGetValue(key, out var entry) && entry is BlackboardEntry<T> castedEntry)
            {
                value = castedEntry.Value;
                return true;
            }

            value = default;
            return false;
        }

        public void SetValueRaw<T>(BlackboardKey key, T value)
        {
            SetValue(BlackboardKey<T>.Get(key), value);
        }

        public override void SetValue<T>(BlackboardKey<T> key, T value)
        {
            if (_entries.TryGetValue(key, out object entry1) && entry1 is BlackboardEntry<T> castedEntry) {
                castedEntry.Value = value;
            } else
            {
                var entry2 = BlackboardEntry<T>.Get(key);
                entry2.Value = value;
                _entries[key] = entry2;
            }
        }

        public override bool ContainsKey<T>(BlackboardKey<T> key) => _entries.ContainsKey(key);

        public void Remove(BlackboardKey key)
        {
            if (_entries.TryGetValue(key, out object entry))
            {
                var type = entry.GetType();

                if (!s_RemoveMethods.ContainsKey(type))
                {
                    s_RemoveMethods[type] = entry.GetType().GetMethod("Release");
                }

                s_RemoveArgs[0] = entry;
                s_RemoveMethods[type].Invoke(null, s_RemoveArgs);
            }

            _entries.Remove(key);
        }

        public override void Remove<T>(BlackboardKey<T> key)
        {
            if (_entries.TryGetValue(key, out object entry) && entry is BlackboardEntry<T> castedEntry)
            {
                BlackboardEntry<T>.Release(castedEntry);
            }

            _entries.Remove(key);
        }

        public override void Clear() => _entries.Clear();

        public override int Count => _entries.Count;
    }

}
