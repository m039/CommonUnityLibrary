using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace m039.Common.AI
{
    public class Arbiter
    {
        readonly List<IExpert> _experts = new();

        IExpert[] _randomizer; // For selecting a random expert if the insistence is equal.

        public void Register(IExpert expert)
        {
            Assert.IsNotNull(expert);
            _experts.Add(expert);
        }

        public void Unregister(IExpert expert)
        {
            Assert.IsNotNull(expert);
            _experts.Remove(expert);
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

            foreach (IExpert expert in _experts)
            {
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
        }
    }
}
