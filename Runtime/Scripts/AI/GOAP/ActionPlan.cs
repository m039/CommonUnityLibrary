using System.Collections.Generic;

namespace m039.Common.GOAP
{
    public class ActionPlan
    {
        public AgentGoal goal { get; }
        public Stack<AgentAction> actions { get; }
        public float totalCost { get; set; }

        public ActionPlan(AgentGoal goal, Stack<AgentAction> actions, float totalCost)
        {
            this.goal = goal;
            this.actions = actions;
            this.totalCost = totalCost;
        }
    }
}
