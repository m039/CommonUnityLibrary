using System.Collections.Generic;
using UnityEngine;

namespace m039.Common.BehaviourTrees.Nodes
{
    public abstract class CompositeNodeBase : NodeBase, ICompositeNode
    {
        protected int currentChild { get; set; }

        [SerializeField]
        NodeBase[] _Nodes;

        public IList<INode> children => _Nodes;

        public void AddChild(INode node)
        {
            if (node == null)
            {
                Debug.LogError("AddChild: Failed to add a node");
                return;
            }
            children.Add(node);
        }

        public override Status Process() => children[currentChild].Process();

        protected override void OnReset()
        {
            currentChild = 0;
            foreach (var child in children)
            {
                child.Reset();
            }
        }
    }
}
