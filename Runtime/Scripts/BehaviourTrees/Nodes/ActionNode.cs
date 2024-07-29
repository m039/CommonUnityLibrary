using UnityEngine;
using UnityEngine.Events;

namespace m039.Common.BehaviourTrees.Nodes
{
    public class ActionNode : NodeBase
    {
        [SerializeField]
        UnityEvent _Event;

        public override Status Process()
        {
            _Event?.Invoke();
            return Status.Success;
        }
    }
}
