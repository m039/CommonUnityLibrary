namespace m039.Common.BehaviourTrees.Nodes
{
    public class SequenceNode : CompositeNodeBase
    {
        public override Status Process()
        {
            for (; currentChild < children.Count; currentChild++)
            {
                switch (children[currentChild].Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Failure:
                        OnReset();
                        return Status.Failure;
                }
            }

            OnReset();
            return Status.Success;
        }
    }
}
