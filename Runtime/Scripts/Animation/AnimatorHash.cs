using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace m039.Common
{

    public struct AnimatorHash
    {
        static public readonly AnimatorHash Null = new AnimatorHash("Null", 0);

        public readonly string name;

        public readonly int hash;

        public AnimatorHash(string name) : this(name, Animator.StringToHash(name))
        {
        }

        AnimatorHash(string name, int hash)
        {
            this.name = name;
            this.hash = hash;
        }

        public override string ToString() => name;

        public override bool Equals(object obj)
        {
            var otherValue = obj as AnimatorHash?;
            if (!otherValue.HasValue)
                return false;

            return GetType().Equals(obj.GetType()) &&
                hash == otherValue.Value.hash;
        }

        public override int GetHashCode() => hash;

        public static bool operator ==(AnimatorHash c1, AnimatorHash c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(AnimatorHash c1, AnimatorHash c2)
        {
            return !c1.Equals(c2);
        }

        public static implicit operator int(AnimatorHash hash)
        {
            return hash.hash;
        }
    }

}
