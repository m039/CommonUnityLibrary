using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace m039.Common.BehaviourTrees
{
    public enum Status { Success, Failure, Running }

    public interface IHasPriority
    {
        int priority { get; }
    }

    public interface IHasName
    {
        string name { get; }
    }

    public interface IHasChildren
    {
        IList<INode> children { get; }
    }

    public interface INode
    {
        Status Process();

        void Reset()
        {
            // Noop
        }
    }

    public static class NodeExt
    {
        static Lazy<IList<INode>> Empty = new(() =>
        {
            return new List<INode>().AsReadOnly();
        });

        public static int GetPriority(this INode node)
        {
            return node switch
            {
                IHasPriority hasPriority => hasPriority.priority,
                _ => 0
            };
        }

        public static string GetName(this INode node)
        {
            return node switch
            {
                IHasName hasName => hasName.name,
                _ => node.GetType().Name
            };
        }

        public static IList<INode> GetChildren(this INode node)
        {
            return node switch
            {
                IHasChildren hasChildren => hasChildren.children,
                _ => Empty.Value
            };
        }
    }

    public class ActionNode : INode, IHasName
    {
        readonly Action _doSomething;

        public string name { get; }

        public ActionNode(string name, Action doSomething)
        {
            this.name = name;
            _doSomething = doSomething;
        }

        public Status Process()
        {
            _doSomething();
            return Status.Success;
        }
    }

    public class ConditionNode : INode, IHasName
    {
        readonly Func<bool> _predicate;

        public string name { get; }

        public ConditionNode(string name, Func<bool> predicate)
        {
            this.name = name;
            _predicate = predicate;
        }

        public Status Process() => _predicate() ? Status.Success : Status.Failure;
    }

    public class UntilFailNode : Node
    {
        public UntilFailNode(string name) : base(name) { }

        public override Status Process()
        {
            if (children[0].Process() == Status.Failure)
            {
                Reset();
                return Status.Failure;
            }

            return Status.Running;
        }
    }

    public class InverterNode : Node
    {
        public InverterNode(string name) : base(name) {}

        public override Status Process()
        {
            switch (children[0].Process())
            {
                case Status.Running:
                    return Status.Running;
                case Status.Failure:
                    return Status.Success;
                default:
                    return Status.Failure;
            }
        }
    }

    public class RandomSelectorNode : PrioritySelectorNode
    {
        protected override List<INode> SortChildren()
        {
            var copy = children.ToList();
            copy.Shuffle();
            return copy;
        }

        public RandomSelectorNode(string name, int priority = 0) : base(name, priority) { }
    }

    public class PrioritySelectorNode : SelectorNode
    {
        List<INode> _sortedChildren;

        List<INode> SortedChildren => _sortedChildren ??= SortChildren();

        protected virtual List<INode> SortChildren() => children.OrderByDescending(child => child.GetPriority()).ToList();

        public PrioritySelectorNode(string name, int priority = 0) : base(name, priority) { }

        public override void Reset()
        {
            base.Reset();
            _sortedChildren = null;
        }

        public override Status Process()
        {
            foreach (var child in SortedChildren)
            {
                switch (child.Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Success:
                        Reset();
                        return Status.Success;
                    default:
                        continue;
                }
            }

            Reset();
            return Status.Failure;
        }
    }

    public class SelectorNode : Node
    {
        public SelectorNode(string name, int priority = 0) : base(name, priority) { }

        public override Status Process()
        {
            if (currentChild < children.Count)
            {
                switch (children[currentChild].Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Success:
                        Reset();
                        return Status.Success;
                    default:
                        currentChild++;
                        return Status.Running;
                }
            }

            Reset();
            return Status.Failure;
        }
    }

    public class SequenceNode : Node
    {
        public SequenceNode(string name, int priority = 0) : base(name, priority) {
        }

        public override Status Process()
        {
            if (currentChild < children.Count)
            {
                switch (children[currentChild].Process())
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Failure:
                        Reset();
                        return Status.Failure;
                    default:
                        currentChild++;
                        return currentChild == children.Count ? Status.Success : Status.Running;
                }
            }

            Reset();
            return Status.Success;
        }
    }

    public abstract class Node : INode, IHasChildren, IHasName
    {
        public int priority { get; private set; }

        public string name { get; private set; }

        public IList<INode> children { get; private set; } = new List<INode>();

        public int currentChild;

        public Node(string name = "Node", int priority = 0)
        {
            this.name = name;
            this.priority = priority;
        }

        public void AddChild(INode node) => children.Add(node);

        public virtual Status Process() => children[currentChild].Process();

        public virtual void Reset()
        {
            currentChild = 0;
            foreach (var child in children)
            {
                child.Reset();
            }
        }
    }

    public delegate bool Policy(Status status);

    public static class Policies
    {
        public static readonly Policy RunForever = (status) => false;
        public static readonly Policy RunUntilSuccess = (status) => status == Status.Success;
        public static readonly Policy RunUntilFailure = (status) => status == Status.Failure;
    }

    public class BehaviourTree : Node
    {
        readonly Policy _policy;

        public BehaviourTree(string name = "BehaviourTree", Policy policy = null) : base(name)
        {
            _policy = policy ?? Policies.RunForever;
        }

        public void Update()
        {
            Process();
        }

        public override Status Process()
        {
            var status = children[currentChild].Process();
            if (_policy(status))
            {
                return status;
            }

            currentChild = (currentChild + 1) % children.Count;
            return Status.Running;
        }

        public void PrintTree()
        {
            var sb = new StringBuilder();
            PrintNode(this, 0, sb);
            Debug.Log(sb.ToString());
        }

        static void PrintNode(INode node, int indentLevel, StringBuilder sb)
        {
            sb.Append(' ', indentLevel * 2).AppendLine(node.GetName());

            foreach (var child in node.GetChildren())
            {
                PrintNode(child, indentLevel + 1, sb);
            }
        }
    }
}
