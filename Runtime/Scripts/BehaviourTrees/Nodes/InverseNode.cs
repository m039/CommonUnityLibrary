using UnityEngine;

namespace m039.Common.BehaviourTrees.Nodes {
    public class InverseNode : NodeBase
    {
        #region Inspector

        [SerializeField]
        NodeBase _Child;

        #endregion

        public override Status Process()
        {
            if (_Child == null)
            {
                return Status.Failure;
            }

            switch (_Child.Process())
            {
                case Status.Success:
                    return Status.Failure;
                case Status.Failure:
                    return Status.Success;
                default:
                    return Status.Running;
            }
        }
    }
}
