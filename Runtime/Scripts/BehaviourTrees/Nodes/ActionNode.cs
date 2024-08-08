using UnityEngine;
using UnityEngine.Events;

namespace m039.Common.BehaviourTrees.Nodes
{
    public class ActionNode : NodeBase
    {
        [SerializeField]
        UnityEvent _Event;

        protected override Status OnProcess()
        {
            _Event?.Invoke();
            return Status.Success;
        }
    }
}
