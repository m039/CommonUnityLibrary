using UnityEngine;

namespace m039.Common.BehaviourTrees.Nodes
{
    public abstract class NodeBase : MonoBehaviour, INode, IHasName, IHasPriority
    {
        [SerializeField]
        int _Priority = 0;

        public int priority => _Priority;

        public abstract Status Process();

        void INode.Reset() {
            OnReset();
        }

        protected virtual void OnReset()
        {
        }
    }
}
