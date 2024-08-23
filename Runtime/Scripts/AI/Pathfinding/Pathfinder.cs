using System.Collections.Generic;
using UnityEngine;

namespace m039.Common.Pathfindig
{
    public enum PathfinderNodeType
    {
        Frontier, Explored, Path, Untouched
    }

    public class Pathfinder
    {
        readonly Graph _graph;

        Node _startNode;

        Node _goalNode;

        const float GoalPriorityCoeff = 1f;

        readonly PriorityQueue<Node> _frontierNodes = new();

        readonly bool[] _isExploredNodes;

        readonly bool[] _isFrontierNodes;

        readonly List<IModifier> _modifiers = new();

        List<Node> _pathNodes;

        public bool DiagonalsWalkable = false;

        internal Pathfinder(Graph graph)
        {
            _graph = graph;
            _isExploredNodes = new bool[_graph.Width * _graph.Height];
            _isFrontierNodes = new bool[_graph.Width * _graph.Height];
        }

        public Path Search(Node start, Node goal)
        {
            if (start == null || goal == null)
            {
#if M039_COMMON_VERBOSE
                Debug.LogWarning("Pathfinder.Search: missing component(s)!");
#endif
                return null;
            }

            if (start.type == NodeType.Blocked || goal.type == NodeType.Blocked)
            {
#if M039_COMMON_VERBOSE
                Debug.LogWarning("Pathfinder.Search: start and goal must be unblocked!");
#endif
                return null;
            }

            _pathNodes = null;
            _startNode = start;
            _goalNode = goal;

            _frontierNodes.Clear();
            System.Array.Clear(_isExploredNodes, 0, _isExploredNodes.Length);
            System.Array.Clear(_isFrontierNodes, 0, _isFrontierNodes.Length);

            Enqueue(_startNode);

            _graph.ResetNodes();
            _startNode.distanceTraveled = 0;

#if M039_COMMON_VERBOSE
            float time = Time.realtimeSinceStartup;
#endif

            while (true)
            {
                if (_frontierNodes.Count > 0)
                {
                    Node currentNode = Dequeue();

                    if (!IsExplored(currentNode))
                    {
                        SetExplored(currentNode, true);
                    }

                    ExpandFrontier(currentNode);

                    if (IsFrontier(_goalNode))
                    {
                        _pathNodes = GetPathNodes(_goalNode);
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

#if M039_COMMON_VERBOSE
            Debug.Log("Pathfinder.Search: elapse time = " + ((Time.realtimeSinceStartup - time) * 1000) + " ms.");
#endif

            return CreatePath(_pathNodes);
        }

        void Enqueue(Node node)
        {
            _frontierNodes.Enqueue(node);
            SetFrontier(node, true);
        }

        Node Dequeue()
        {
            var node = _frontierNodes.Dequeue();
            SetFrontier(node, false);
            return node;
        }

        bool IsExplored(Node node)
        {
            return Contains(_isExploredNodes, node);
        }

        bool IsFrontier(Node node)
        {
            return Contains(_isFrontierNodes, node);
        }

        void SetExplored(Node node, bool value)
        {
            SetValue(_isExploredNodes, node, value);
        }

        void SetFrontier(Node node, bool value)
        {
            SetValue(_isFrontierNodes, node, value);
        }

        bool Contains(bool[] array, Node node)
        {
            return array[_graph.Width * node.yIndex + node.xIndex];
        }

        void SetValue(bool[] array, Node node, bool value)
        {
            array[_graph.Width * node.yIndex + node.xIndex] = value;
        }

        public void AddModifier(IModifier modifier)
        {
            _modifiers.Add(modifier);
        }

        Path CreatePath(List<Node> nodes)
        {
            Path path = new()
            {
                path = nodes,
                vectorPath = new List<Vector3>()
            };

            if (nodes != null && nodes.Count > 0)
            {
                foreach (var n in nodes)
                {
                    path.vectorPath.Add(n.position);
                }

                if (_modifiers != null)
                {
                    foreach (var m in _modifiers)
                    {
                        m.Apply(path);
                    }
                }
            }

            return path;
        }

        /// To stay consistent, A* Pathfinding Project checks if diagonals are blocked.
        bool CheckIfDiagonalsWalkable(Node node, Node neighbor)
        {
            if (DiagonalsWalkable)
                return true;

            var dx = neighbor.xIndex - node.xIndex;
            var dy = neighbor.yIndex - node.yIndex;

            bool isBlocked(int dx, int dy)
            {
                var newX = node.xIndex + dx;
                var newY = node.yIndex + dy;
                return newX >= 0 && newX < _graph.Width &&
                    newY >= 0 && newY < _graph.Height &&
                    _graph.GetNode(newX, newY).type == NodeType.Blocked;
            }

            if (dx == 1 && dy == 1)
            {
                return !(isBlocked(0, 1) && isBlocked(1, 0));
            }

            if (dx == 1 && dy == -1)
            {
                return !(isBlocked(1, 0) && isBlocked(0, -1));
            }

            if (dx == -1 && dy == 1)
            {
                return !(isBlocked(-1, 0) && isBlocked(0, 1));
            }

            if (dx == -1 && dy == -1)
            {
                return !(isBlocked(-1, 0) && isBlocked(0, -1));
            }

            return true;
        }

        void ExpandFrontier(Node node)
        {
            if (node != null)
            {
                for (int i = 0; i < node.neighbors.Length; i++)
                {
                    var neighbor = node.neighbors[i];
                    if (!CheckIfDiagonalsWalkable(node, neighbor))
                    {
                        continue;
                    }

                    if (!IsExplored(neighbor))
                    {
                        float distanceToNeighbor = _graph.GetNodeDistance(node, neighbor);
                        float newDistanceTraveled = distanceToNeighbor + node.distanceTraveled;

                        if (float.IsPositiveInfinity(neighbor.distanceTraveled) ||
                            newDistanceTraveled < neighbor.distanceTraveled)
                        {
                            neighbor.previous = node;
                            neighbor.distanceTraveled = newDistanceTraveled;
                        }

                        if (!IsFrontier(neighbor))
                        {
                            float distanceToGoal = _graph.GetNodeDistance(neighbor, _goalNode);
                            neighbor.priority = neighbor.distanceTraveled + distanceToGoal * GoalPriorityCoeff;
                            Enqueue(neighbor);
                        }
                    }
                }
            }
        }

        List<Node> GetPathNodes(Node endNode)
        {
            if (endNode != null)
            {
                List<Node> path = new()
                {
                    endNode
                };

                Node currentNode = endNode.previous;

                while (currentNode != null)
                {
                    path.Add(currentNode);
                    currentNode = currentNode.previous;
                }

                path.Reverse();

                return path;
            }

            return null;
        }

        internal PathfinderNodeType GetNodeType(Node node)
        {
            if (_pathNodes != null && _pathNodes.Contains(node))
            {
                return PathfinderNodeType.Path;
            }
            else if (IsFrontier(node))
            {
                return PathfinderNodeType.Frontier;
            }
            else if (IsExplored(node))
            {
                return PathfinderNodeType.Explored;
            }
            else
            {
                return PathfinderNodeType.Untouched;
            }
        }
    }
}
