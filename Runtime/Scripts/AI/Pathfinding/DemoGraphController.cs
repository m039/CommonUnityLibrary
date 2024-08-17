using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace m039.Common.Pathfindig
{
    public class DemoGraphController : GraphController
    {
        #region Inspector

        [SerializeField]
        bool _DebugWithMouse = false;

        #endregion

        Path _path;

        Node _startNode;

        Node _goalNode;

        Seeker _seeker;

        void Awake()
        {
            _seeker = GetComponent<Seeker>();
        }

        void Update()
        {
            if (!_DebugWithMouse)
            {
                return;
            }

            bool leftMouseButtonDown = false;
            bool rightMouseButtonDown = false;

            if (Input.GetMouseButtonDown(0))
            {
                leftMouseButtonDown = true;
            }

            if (Input.GetMouseButtonDown(1))
            {
                rightMouseButtonDown = true;
            }

            if (leftMouseButtonDown || rightMouseButtonDown)
            {
                var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                var node = GetNodeAt(worldPoint);
                if (node != null && node.type != NodeType.Blocked)
                {
                    bool doSearch = false;

                    if (leftMouseButtonDown)
                    {
                        _startNode = node;
                        doSearch = true;
                    }
                    else if (rightMouseButtonDown)
                    {
                        _goalNode = node;
                        doSearch = true;
                    }

                    if (doSearch && _startNode != null && _goalNode != null)
                    {
                        _path = _seeker.Search(_startNode.position, _goalNode.position);
                    }
                }
            }
        }

        public override Color GetNodeColor(int x, int y)
        {
            if (Graph != null)
            {
                Node node = Graph.GetNode(x, y);
                if (node != null)
                {
                    if (node == _startNode)
                    {
                        return Color.green;
                    }
                    else if (node == _goalNode)
                    {
                        return Color.red;
                    }

                    if (_seeker != null)
                    {
                        var pathfinderNodeType = _seeker.Pathfinder.GetNodeType(node);
                        if (pathfinderNodeType == PathfinderNodeType.Explored)
                        {
                            return Color.grey;
                        }
                        else if (pathfinderNodeType == PathfinderNodeType.Frontier)
                        {
                            return Color.magenta;
                        }
                        else if (pathfinderNodeType == PathfinderNodeType.Path)
                        {
                            return Color.cyan;
                        }
                    }

                    if (node.type == NodeType.Blocked)
                    {
                        return Color.blue;
                    }
                }
            }

            return base.GetNodeColor(x, y);
        }

        void OnDrawGizmos()
        {
            if (_path != null)
            {
                Gizmos.color = Color.green;

                for (int i = 0; i < _path.vectorPath.Count - 1; i++)
                {
                    var v1 = _path.vectorPath[i];
                    var v2 = _path.vectorPath[i + 1];

                    Gizmos.DrawLine(v1, v2);
                }
            }
        }
    }
}
