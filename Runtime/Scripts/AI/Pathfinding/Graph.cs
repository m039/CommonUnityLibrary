using System.Collections.Generic;
using UnityEngine;

namespace m039.Common.Pathfindig
{
    public class Graph
    {
        readonly int _width;

        readonly int _height;

        float _cellWidth;

        float _cellHeight;

        float _cellDiagonal;

        readonly Node[,] _nodes;

        public int Width => _width;

        public int Height => _height;

        static readonly Vector2Int[] AllDirections = {
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1)
        };

        public Graph(IGraphController graphController, int[,] values)
        {
            _width = values.GetLength(0);
            _height = values.GetLength(1);
            SetAspectRatio(graphController.Width / graphController.Height);

            _nodes = new Node[_width, _height];

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _nodes[x, y] = new Node(x, y, (NodeType) values[x, y]);
                    _nodes[x, y].position = graphController.GetNodePosition(x, y);
                }
            }

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_nodes[x, y].type != NodeType.Blocked)
                    {
                        _nodes[x, y].neighbors = GetNeighbors(x, y);
                    }
                }
            }
        }

        public Pathfinder CreatePahtfinder()
        {
            return new Pathfinder(this);
        }

        public void SetAspectRatio(float aspectRatio)
        {
            _cellWidth = 1;
            _cellHeight = (float)_width / (float)_height * _cellWidth / aspectRatio;
            _cellDiagonal = Mathf.Sqrt(_cellWidth * _cellWidth + _cellHeight * _cellHeight);
        }

        public Node GetNode(int x, int y)
        {
            if (x >= 0 && x < _width && y >= 0 && y < _height)
                return _nodes[x, y];
            else
                return null;
        }

        Node[] GetNeighbors(int x, int y)
        {
            return GetNeighbors(x, y, _nodes, AllDirections);
        }

        Node[] GetNeighbors(int x, int y, Node[,] nodeArray, Vector2Int[] directions)
        {
            List<Node> neighborNodes = new();

            foreach (Vector2Int dir in directions)
            {
                int newX = x + dir.x;
                int newY = y + dir.y;

                if (IsWithinBounds(newX, newY) &&
                    nodeArray[newX, newY] != null &&
                    nodeArray[newX, newY].type != NodeType.Blocked)
                {
                    neighborNodes.Add(nodeArray[newX, newY]);
                }
            }

            return neighborNodes.ToArray();
        }

        public bool IsWithinBounds(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height;
        }

        internal void ResetNodes()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++) {
                    _nodes[x, y].Reset();
                }
            }
        }

#if false

        public float GetNodeDistance(Node source, Node target)
        {
            float dx = Mathf.Abs(source.xIndex - target.xIndex) * _cellWidth;
            float dy = Mathf.Abs(source.yIndex - target.yIndex) * _cellHeight;
            float min = Mathf.Min(dx, dy);
            float max = Mathf.Max(dx, dy);

            float diagonalSteps = min;
            float straightSteps = max - min;

            return _cellDiagonal * diagonalSteps + straightSteps;
        }

#else

        public float GetNodeDistance(Node source, Node target)
        {
            return Vector2.Distance(source.position, target.position);
        }

#endif


    }

}
