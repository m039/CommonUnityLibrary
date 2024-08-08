using System.Collections.Generic;
using UnityEngine;

namespace m039.Common.BehaviourTrees.Nodes {
    public class InverseNode : NodeBase, ICompositeNode
    {
        #region Inspector

        [SerializeField]
        NodeBase _Child;

        #endregion

        readonly NodeBase[] _childs = new NodeBase[1];

        public IList<INode> children
        {
            get
            {
                _childs[0] = _Child;
                return _childs;
            }
        }

        protected override Status OnProcess()
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

        public override void OnStartProccess()
        {
            base.OnStartProccess();

            if (_Child is IOnStartProccess onStartProcess)
            {
                onStartProcess.OnStartProccess();
            }
        }
    }
}
