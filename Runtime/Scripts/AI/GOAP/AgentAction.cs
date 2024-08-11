using System.Collections.Generic;

namespace m039.Common.GOAP
{
    public class AgentAction 
    {
        public string name { get; }

        public float cost { get; private set; }

        public HashSet<AgentBelief> preconditions { get; } = new();

        public HashSet<AgentBelief> effects { get; } = new();

        IActionStrategy strategy;

        public bool complete => strategy.complete;

        AgentAction(string name)
        {
            this.name = name;
        }

        public void Start() => strategy.Start();

        public void Update(float deltaTime)
        {
            if (strategy.canPerform)
            {
                strategy.Update(deltaTime);
            }

            if (!strategy.complete) return;

            foreach (var effect in effects)
            {
                effect.Evaluate();
            }
        }

        public void Stop() => strategy.Stop();

        public class Builder
        {
            readonly AgentAction action;

            public Builder(string name)
            {
                action = new AgentAction(name)
                {
                    cost = 1
                };
            }

            public Builder WithCost(float cost)
            {
                action.cost = cost;
                return this;
            }

            public Builder WithStrategy(IActionStrategy strategy)
            {
                action.strategy = strategy;
                return this;
            }

            public Builder AddPrecondition(AgentBelief precondition)
            {
                action.preconditions.Add(precondition);
                return this;
            }

            public Builder AddEffect(AgentBelief effect)
            {
                action.effects.Add(effect);
                return this;
            }

            public AgentAction Build()
            {
                return action;
            }
        }
    }
}
