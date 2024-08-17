using System;
using UnityEngine;

namespace m039.Common.Pathfindig
{
    public enum NodeType
    {
        Open = 0,
        Blocked = 1
    }

    public class Node : IComparable<Node>
    {

        public int xIndex = -1;

        public int yIndex = -1;

        public NodeType type;

        public Vector3 position;

        internal Node[] neighbors;

        internal float distanceTraveled = float.PositiveInfinity;

        internal Node previous = null;

        internal float priority;

        public Node(int xIndex, int yIndex, NodeType type)
        {
            this.xIndex = xIndex;
            this.yIndex = yIndex;
            this.type = type;
        }

        public int CompareTo(Node other)
        {
            if (priority < other.priority)
            {
                return -1;
            }
            else if (priority > other.priority)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public void Reset()
        {
            previous = null;
            distanceTraveled = float.PositiveInfinity;
        }
    }
}
