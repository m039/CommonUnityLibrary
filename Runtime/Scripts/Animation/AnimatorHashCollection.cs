using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Common
{

    public class AnimatorHashCollection
    {
        readonly Dictionary<int, AnimatorHash> _states;

        public AnimatorHashCollection(params AnimatorHash[] states)
        {
            _states = new Dictionary<int, AnimatorHash>();

            foreach (var state in states)
            {
                _states.Add(state.hash, state);
            }
        }

        public bool Contains(int hash)
        {
            return _states.ContainsKey(hash);
        }

        public bool Contains(AnimatorHash hash)
        {
            return _states.ContainsKey(hash.hash);
        }

        public AnimatorHash GetState(int hash)
        {
            if (_states.TryGetValue(hash, out AnimatorHash value)) {
                return value;
            }

            return AnimatorHash.Null;
        }
    }

}
