namespace m039.Common.BehaviourTrees.Nodes
{
    public class SequenceNode : CompositeNodeBase
    {
        public override Status Process()
        {
            for (; currentChild < children.Count; currentChild++)
            {
                var status = children[currentChild].Process();
                switch (status)
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
