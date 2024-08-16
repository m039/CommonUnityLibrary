
using System;
using System.Collections.Generic;

namespace m039.Common.GOAP
{
    public class AgentGoal
    {
        private System.Func<float> _priority = delegate {  return 0f; };

        public string name { get; }

        public float priority => _priority();

        public HashSet<AgentBelief> desiredEffects { get; } = new();

        AgentGoal(string name)
        {
            this.name = name;
        }

        public class Builder
        {
            readonly AgentGoal goal;

            public Builder(string name)
            {
                goal = new AgentGoal(name);
            }

            public Builder WithPriority(float priority)
            {
                return WithPriority(() => priority);
            }

            public Builder WithPriority(Func<float> priority)
            {
                goal._priority = priority;
                return this;
            }

            public Builder WithDesiredEffect(AgentBelief effect)
            {
                goal.desiredEffects.Add(effect);
                return this;
            }

            public AgentGoal Build()
            {
                return goal;
            }
        }
    }
}
