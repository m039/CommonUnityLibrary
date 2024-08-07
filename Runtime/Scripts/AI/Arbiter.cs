using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace m039.Common.AI
{
    public class Arbiter
    {
        readonly List<IExpert> _experts = new();

        IExpert[] _randomizer; // For selecting a random expert if the insistence is equal.

        bool _isExecuting = false;

        bool _neededCleanUp = false;

        public void Register(IExpert expert)
        {
            Assert.IsNotNull(expert);
            if (!_experts.Contains(expert))
            {
                _experts.Add(expert);
            }
        }

        public void Unregister(IExpert expert)
        {
            Assert.IsNotNull(expert);
            if (_isExecuting)
            {
                var index = _experts.IndexOf(expert);
                if (index >= 0)
                {
                    _experts[index] = null;
                    _neededCleanUp = true;
                }
            }
            else
            {
                _experts.Remove(expert);
            }
        }

        public void Clear()
        {
            _experts.Clear();
        }

        public void Iteration()
        {
            if (_randomizer == null || _randomizer.Length != _experts.Count)
            {
                _randomizer = new IExpert[_experts.Count];
            }

            IExpert bestExpert = null;
            int highestInsistence = 0;
            int randomizerIndex = 0;

            for (int i = 0; i < _experts.Count; i++)
            {
                var expert = _experts[i];
                if (expert == null)
                    continue;

                int insistence = expert.GetInsistence();
                if (insistence > highestInsistence)
                {
                    highestInsistence = insistence;
                    bestExpert = expert;
                    randomizerIndex = 0;
                    _randomizer[randomizerIndex++] = expert;
                } else if (insistence == highestInsistence && highestInsistence != 0)
                {
                    _randomizer[randomizerIndex++] = expert;
                }
            }

            if (_randomizer.Length > 0)
            {
                _randomizer[Random.Range(0, randomizerIndex)]?.Execute();
            }

            _isExecuting = true;
            for (int i = 0; i < _experts.Count; i++)
            {
                var expert = _experts[i];
                if (expert == null)
                    continue;

                expert.AfterAllExecute();
            }
            _isExecuting = false;

            if (_neededCleanUp)
            {
                _experts.RemoveAll(x => x == null);
                _neededCleanUp = false;
            }
        }
    }
}
