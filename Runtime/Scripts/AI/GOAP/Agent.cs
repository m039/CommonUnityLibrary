using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace m039.Common.GOAP
{
    public class Agent
    {
        public HashSet<AgentAction> actions = new();

        public HashSet<AgentGoal> goals = new();

        public Dictionary<string, AgentBelief> beliefs = new();

        ActionPlan _actionPlan;

        AgentAction _currentAction;

        AgentGoal _currentGoal;

        AgentGoal _lastGoal;

        readonly Planner _planner = new();

        bool _planUpdated;

        public void Update()
        {
            if (_planUpdated)
            {
                _planUpdated = false;

                if (_actionPlan != null && _actionPlan.actions.Count > 0)
                {
                    var newAction = _actionPlan.actions.Pop();

                    if (_currentAction != null && _currentAction != newAction)
                    {
                        _currentAction.Stop();
                    }

                    if (_currentAction == null || _currentAction != newAction)
                    {
                        _currentAction = newAction;
                        _currentAction.Start();
                    }
                }
            }

            if (_currentAction != null)
            {
                if (_currentAction.complete)
                {
                    _currentAction.Stop();
                    _currentAction = null;

                    if (_actionPlan.actions.Count <= 0)
                    {
                        _actionPlan = null;
                    }
                }
                else
                {
                    _currentAction.Update(Time.deltaTime);
                }
            } else
            {
                CalculatePlan();
            }

            if (_actionPlan != null && _actionPlan.actions.Count > 0 && _currentAction == null)
            {
                _planUpdated = false;
                _currentAction = _actionPlan.actions.Pop();

                if (_currentAction.preconditions.All(p => p.Evaluate()))
                {
                    _currentAction.Start();
                } else
                {
                    _currentAction = null;
                    _currentGoal = null;
                    _actionPlan = null;
                }
            }
        }

        public void ClearState()
        {
            _currentAction = null;
            _currentGoal = null;
            _actionPlan = null;
        }

        public void CalculatePlan()
        {
            var priorityLevel = _currentGoal?.priority ?? 0;

            HashSet<AgentGoal> goalsToCheck = goals;

            if (_currentGoal != null)
            {
                goalsToCheck = new (goals.Where(g => g.priority > priorityLevel));
            }

            var potentialPlan = _planner.Plan(this, goalsToCheck, _lastGoal);
            if (potentialPlan != null)
            {
                _actionPlan = potentialPlan;
                _planUpdated = true;
            }
        }
    }

}
