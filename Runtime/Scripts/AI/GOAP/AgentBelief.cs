using System;

namespace m039.Common.GOAP
{
    public class AgentBelief
    {
        public string Name { get; }

        Func<bool> condition = () => false;

        AgentBelief(string name)
        {
            Name = name;
        }

        public bool Evaluate() => condition();

        public class Builder
        {
            readonly AgentBelief belief;

            public Builder(string name)
            {
                belief = new AgentBelief(name);
            }

            public Builder WithCondition(Func<bool> condition)
            {
                belief.condition = condition;
                return this;
            }

            public AgentBelief Build()
            {
                return belief;
            }
        }
    }
}
