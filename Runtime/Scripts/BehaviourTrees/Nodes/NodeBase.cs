using UnityEngine;

namespace m039.Common.BehaviourTrees.Nodes
{
    public abstract class NodeBase : MonoBehaviour, INode, IHasName, IHasPriority, IOnStartProccess
    {
        [SerializeField]
        int _Priority = 0;

        public int priority => _Priority;

        public Status lastStatus;

        public bool lastProcessedStatus = true;

        public Status Process()
        {
            lastStatus = OnProcess();
            lastProcessedStatus = true;
            return lastStatus;
        }

        protected abstract Status OnProcess();

        void INode.Reset() {
            OnReset();
        }

        protected virtual void OnReset()
        {
        }

        public virtual void OnStartProccess()
        {
            lastProcessedStatus = false;
        }
    }
}
