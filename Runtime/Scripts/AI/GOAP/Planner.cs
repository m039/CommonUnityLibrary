using System.Collections.Generic;
using System.Linq;

namespace m039.Common.GOAP
{
    public class Planner
    {
        public ActionPlan Plan(Agent agent, HashSet<AgentGoal> goals, AgentGoal mostRecentGoal = null)
        {
            List<AgentGoal> orderedGoals = goals
                .Where(g => g.desiredEffects.Any(b => !b.Evaluate()))
                .OrderByDescending(g => g == mostRecentGoal ? g.priority - 0.01 : g.priority)
                .ToList();

            foreach (var goal in orderedGoals)
            {
                Node goalNode = new Node(null, null, goal.desiredEffects, 0);

                if (FindPath(goalNode, agent.actions))
                {
                    if (goalNode.isLeafDead) continue;

                    Stack<AgentAction> actionStack = new();
                    while (goalNode.leaves.Count > 0)
                    {
                        var cheapestLeaf = goalNode.leaves.OrderBy(leaf => leaf.cost).First();
                        goalNode = cheapestLeaf;
                        actionStack.Push(cheapestLeaf.action);
                    }

                    return new ActionPlan(goal, actionStack, goalNode.cost);
                }
            }

            return null;
        }

        bool FindPath(Node parent, HashSet<AgentAction> actions)
        {
            var orderedActions = actions.OrderBy(a => a.cost);
            var requiredEffects = parent.requiredEffects;

            requiredEffects.RemoveWhere(b => b.Evaluate());

            if (requiredEffects.Count == 0)
            {
                return true;
            }

            foreach (var action in orderedActions)
            {
                if (action.effects.Any(requiredEffects.Contains))
                {
                    var newRequiredEffects = new HashSet<AgentBelief>(requiredEffects);
                    newRequiredEffects.ExceptWith(action.effects);
                    newRequiredEffects.UnionWith(action.preconditions);

                    var newAvailableActions = new HashSet<AgentAction>(actions);
                    newAvailableActions.Remove(action);

                    var newNode = new Node(parent, action, newRequiredEffects, parent.cost + action.cost);

                    if (FindPath(newNode, newAvailableActions))
                    {
                        parent.leaves.Add(newNode);
                        newRequiredEffects.ExceptWith(newNode.action.preconditions);
                    }

                    if (newRequiredEffects.Count == 0)
                    {
                        return true;
                    }
                }
            }

            return parent.leaves.Count > 0;
        }

        class Node
        {
            public Node parent { get; }

            public AgentAction action { get; }

            public HashSet<AgentBelief> requiredEffects { get; }

            public List<Node> leaves { get; }

            public float cost { get; }

            public bool isLeafDead => leaves.Count == 0 && action == null;

            public Node(Node parent, AgentAction action, HashSet<AgentBelief> effects, float cost)
            {
                this.parent = parent;
                this.action = action;
                requiredEffects = new HashSet<AgentBelief>(effects);
                leaves = new List<Node>();
                this.cost = cost;
            }
        }
    }
}
