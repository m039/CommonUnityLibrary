using System;
using System.Collections.Generic;
using System.Text;

namespace m039.Common.Blackboard
{
    [Serializable]
    public readonly struct BlackboardKey : IEquatable<BlackboardKey>
    {
        readonly string _name;
        readonly int _hashedKey;

        public BlackboardKey(string name)
        {
            _name = name;
            _hashedKey = name.ComputeFNV1aHash();
        }

        public bool Equals(BlackboardKey other) => _hashedKey == other._hashedKey;

        public override bool Equals(object obj) => obj is BlackboardKey other && Equals(other);
        public override int GetHashCode() => _hashedKey;
        public override string ToString() => _name;

        public static bool operator ==(BlackboardKey lhs, BlackboardKey rhs) => lhs._hashedKey == rhs._hashedKey;
        public static bool operator !=(BlackboardKey lhs, BlackboardKey rhs) => !(lhs == rhs);
    }

    [Serializable]
    public readonly struct BlackboardEntry<T>
    {
        public BlackboardKey Key { get; }
        public T Value { get; }

        public BlackboardEntry(BlackboardKey key, T value)
        {
            Key = key;
            Value = value;
        }

        public override bool Equals(object obj) => obj is BlackboardEntry<T> other && other.Key == Key;
        public override int GetHashCode() => Key.GetHashCode();
    }

    [Serializable]
    public class Blackboard
    {
        readonly Dictionary<BlackboardKey, object> _entries = new();

        public void Debug()
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

        public bool TryGetValue(BlackboardKey key, out object value)
        {
            if (_entries.TryGetValue(key, out var entry))
            {
                value = entry.GetType().GetProperty("Value").GetValue(entry);
                return true;
            } else
            {
                value = null;
                return false;
            }
        }

        public bool TryGetValue<T>(BlackboardKey key, out T value)
        {
            if (_entries.TryGetValue(key, out var entry) && entry is BlackboardEntry<T> castedEntry)
            {
                value = castedEntry.Value;
                return true;
            }

            value = default;
            return false;
        }

        public void SetValue<T>(BlackboardKey key, T value)
        {
            _entries[key] = new BlackboardEntry<T>(key, value);
        }

        public bool ContainsKey(BlackboardKey key) => _entries.ContainsKey(key);

        public void Remove(BlackboardKey key) => _entries.Remove(key);

        public void Clear() => _entries.Clear();
    }

}
