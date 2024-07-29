using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace m039.Common.BehaviourTrees.Nodes
{
    public class SelectorNode : CompositeNodeBase
    {
        public override Status Process()
        {
            for (; currentChild < children.Count; currentChild++)
            {
                switch (children[currentChild].Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Success:
                        OnReset();
                        return Status.Success;
                }
            }

            OnReset();
            return Status.Failure;
        }
    }
}
