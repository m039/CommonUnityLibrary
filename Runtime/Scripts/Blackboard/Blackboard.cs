using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

namespace m039.Common.Blackboard
{
    public interface IReleasable
    {
        void Release();
    }

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
        public BlackboardKey(string name) : base(name)
        {
        }
    }

    [Serializable]
    public abstract class BlackboardEntry
    {
        public abstract object GetValue();

        public abstract Type GetValueType();

        public abstract void Clear();
    }

    public class BlackboardEntry<T> : BlackboardEntry
    {
        public T Value { get; set; }

        static readonly bool s_IsReleasable;

        static BlackboardEntry() {
            s_IsReleasable = typeof(IReleasable).IsAssignableFrom(typeof(T));
        }

        public override object GetValue() => Value;

        public override Type GetValueType()
        {
            return typeof(T);
        }

        public override void Clear()
        {
            if (s_IsReleasable && Value is IReleasable releasable)
            {
                releasable.Release();
            }
            Value = default;
        }
    }

    [Serializable]
    public abstract class BlackboardBase
    {
        public T GetValue<T>(BlackboardKey<T> key, T @default = default)
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

        public void UpdateValue<T>(BlackboardKey<T> key, Func<T, T> callback, T @default = default)
        {
            if (TryGetValue(key, out T v))
            {
                SetValue(key, callback(v));
            } else
            {
                SetValue(key, callback(@default));
            }
        }

        public abstract bool TryGetValue<T>(BlackboardKey<T> key, out T value);

        public abstract void SetValue<T>(BlackboardKey<T> key, T value);

        public abstract bool ContainsKey<T>(BlackboardKey<T> key);

        public abstract void Remove<T>(BlackboardKey<T> key);

        public abstract void Clear();

        public abstract int Count { get; }
    }

    public class Blackboard : BlackboardBase
    {
        static readonly Dictionary<Type, Queue<BlackboardEntry>> s_EntryCache = new();

        readonly Dictionary<BlackboardKey, BlackboardEntry> _entries = new();

        public Dictionary<BlackboardKey, BlackboardEntry>.Enumerator GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        public T GetValueRaw<T>(BlackboardKey key, T @default)
        {
            if (TryGetValueRaw(key, out T v))
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
            if (_entries.TryGetValue(key, out var entry) && entry is BlackboardEntry<T> castedEntry)
            {
                value = castedEntry.Value;
                return true;
            }

            value = default;
            return false;
        }

        public override bool TryGetValue<T>(BlackboardKey<T> key, out T value)
        {
            if (_entries.TryGetValue(key, out var entry))
            {
                value = ((BlackboardEntry<T>)entry).Value;
                return true;
            }

            value = default;
            return false;
        }

        public void SetValueRaw<T>(BlackboardKey key, T value)
        {
            if (_entries.TryGetValue(key, out BlackboardEntry entry1))
            {
                if (entry1 is BlackboardEntry<T> castedEntry)
                {
                    castedEntry.Value = value;
                    return;
                } else {
                    ReleaseEntry(entry1);
                }
            }

            var entry2 = GetEntry<T>();
            entry2.Value = value;
            _entries[key] = entry2;
        }

        public override void SetValue<T>(BlackboardKey<T> key, T value)
        {
            if (_entries.TryGetValue(key, out BlackboardEntry entry1)) {
                ((BlackboardEntry<T>)entry1).Value = value;
            } else
            {
                var entry2 = GetEntry<T>();
                entry2.Value = value;
                _entries[key] = entry2;
            }
        }

        public override bool ContainsKey<T>(BlackboardKey<T> key) => _entries.ContainsKey(key);

        public void RemoveRaw(BlackboardKey key)
        {
            if (_entries.TryGetValue(key, out BlackboardEntry entry))
            {
                ReleaseEntry(entry);
            }

            _entries.Remove(key);
        }

        public override void Remove<T>(BlackboardKey<T> key)
        {
            if (_entries.TryGetValue(key, out BlackboardEntry entry))
            {
                ReleaseEntry(entry);
            }
            
            _entries.Remove(key);
        }

        public override void Clear()
        {
            foreach (var e in _entries.Values)
            {
                ReleaseEntry(e);
            }
            _entries.Clear();
        }

        public override int Count => _entries.Count;

        static BlackboardEntry<T> GetEntry<T>()
        {
            var type = typeof(T);

            BlackboardEntry<T> entry;
            if (s_EntryCache.ContainsKey(type) && s_EntryCache[type].Count > 0)
            {
                entry = (BlackboardEntry<T>)s_EntryCache[type].Dequeue();
            }
            else
            {
                entry = new BlackboardEntry<T>();
            }
            return entry;
        }

        static void ReleaseEntry(BlackboardEntry entry)
        {
            var type = entry.GetValueType();
            entry.Clear();

            if (!s_EntryCache.ContainsKey(type))
            {
                s_EntryCache[type] = new();
            }

            s_EntryCache[type].Enqueue(entry);
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
            }
            else
            {
                UnityEngine.Debug.Log("The blackboard is empty.");
            }
        }
    }
}
