using System;
using UnityEditor;
using UnityEngine;

namespace m039.Common.Pathfindig
{
    public interface IGraphController
    {
        Vector3 GetNodePosition(int x, int y);

        Node GetNodeAt(Vector3 v1);

        float Width { get; }

        float Height { get; }

        Graph Graph { get; }

        System.Action onGraphChanged { get; set; }
    }

    public class GraphController : MonoBehaviour, IGraphController
    {

        public float width = 10;

        public float height = 10;

        public int rows = 10;

        public int columns = 10;

        public LayerMask obstacleMask;

        [Header("Diagnostics")]

        public float borderSize = 0.2f;

        public bool showDebug = true;

        Graph _graph;

        float IGraphController.Width => width;

        float IGraphController.Height => height;

        public Graph Graph
        {
            get
            {
                _graph ??= new Graph(this, GetMapData());
                return _graph;
            }
        }

        public Action onGraphChanged { get; set; }

        int[,] GetMapData()
        {
            int[,] values = new int[columns, rows];

            Vector2 boxSize = new(GetCellWidth(), GetCellHeight());

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    if (Physics2D.OverlapBox(GetNodePosition(x, y), boxSize, 0, obstacleMask) != null)
                    {
                        values[x, y] = 1;
                    }
                    else
                    {
                        values[x, y] = 0;
                    }
                }
            }

            return values;
        }

        public void Refresh()
        {
            if (_graph == null)
                return;

            _graph = new Graph(this, GetMapData());
            onGraphChanged?.Invoke();
        }

        public Node GetNodeAt(Vector3 v1)
        {
            v1 -= GetLeftBottomPoint();

            int x = (int)Mathf.Floor(v1.x / GetCellWidth());
            int y = (int)Mathf.Floor(v1.y / GetCellHeight());

            return Graph.GetNode(x, y);
        }

        Vector3 GetLeftBottomPoint()
        {
            var center = transform.position;
            return new Vector3(center.x - width / 2, center.y - height / 2, center.z);
        }

        public Vector3 GetNodePosition(int x, int y)
        {
            Vector3 center = transform.position;
            float cellWidth = GetCellWidth();
            float cellHeight = GetCellHeight();

            return new Vector3(
                center.x - width / 2 + cellWidth / 2 + x * cellWidth,
                center.y - height / 2 + cellHeight / 2 + y * cellHeight,
                center.z
                );
        }

        public float GetCellWidth()
        {
            return width / columns;
        }

        public float GetCellHeight()
        {
            return height / rows;
        }

        public virtual Color GetNodeColor(int x, int y)
        {
            if (_graph != null)
            {
                Node node = _graph.GetNode(x, y);
                if (node != null && node.type == NodeType.Blocked)
                {
                    return Color.blue;
                }
            }

            return Color.white;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(GraphController), true)]
    public class GraphControllerEditor : Editor
    {
        public virtual void OnSceneGUI()
        {
            GraphController controller = (GraphController)target;

            if (controller.showDebug)
            {
                DrawNodes(controller);
            }

            DrawOutline(controller);
            DrawBoundsPositionHandles(controller);
        }

        void DrawNodes(GraphController controller)
        {
            float borderSize = controller.borderSize;
            var center = controller.transform.position;
            var cellWidth = controller.width / controller.columns;
            var cellHeight = controller.height / controller.rows;
            var z = center.z;

            Vector3[] verts = new Vector3[4];

            var y = center.y - controller.height / 2 + cellHeight / 2;

            for (int r = 0; r < controller.rows; r++)
            {
                var x = center.x - controller.width / 2 + cellWidth / 2;

                for (int c = 0; c < controller.columns; c++)
                {
                    verts[0] = new Vector3(x - cellWidth / 2 + borderSize / 2, y - cellHeight / 2 + borderSize / 2, z);
                    verts[1] = new Vector3(x + cellWidth / 2 - borderSize / 2, y - cellHeight / 2 + borderSize / 2, z);
                    verts[2] = new Vector3(x + cellWidth / 2 - borderSize / 2, y + cellHeight / 2 - borderSize / 2, z);
                    verts[3] = new Vector3(x - cellWidth / 2 + borderSize / 2, y + cellHeight / 2 - borderSize / 2, z);

                    var color = controller.GetNodeColor(c, r);

                    Handles.DrawSolidRectangleWithOutline(verts, color * 0.5f, color * 0.5f);

                    x += cellWidth;
                }

                y += cellHeight;
            }
        }

        void DrawBoundsPositionHandles(GraphController controller)
        {
            Handles.color = Color.white;

            var center = controller.transform.position;
            var widthHalf = controller.width / 2;
            var heightHalf = controller.height / 2;
            var z = 0;

            var leftHandlePosition = new Vector3(center.x - widthHalf, center.y, z);
            var topHandlePosition = new Vector3(center.x, center.y + heightHalf, z);
            var rightHandlePosition = new Vector3(center.x + widthHalf, center.y, z);
            var bottomHandlePosition = new Vector3(center.x, center.y - heightHalf, z);

            Vector3 newPosition;

            EditorGUI.BeginChangeCheck();
            var fmh_199_70_638595283233889511 = Quaternion.identity; newPosition = Handles.FreeMoveHandle(leftHandlePosition, 0.2f, Vector3.one * 0.5f, Handles.DotHandleCap);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(controller, "Change size of graph");
                controller.width -= (newPosition.x - leftHandlePosition.x) / 2;
            }

            EditorGUI.BeginChangeCheck();
            var fmh_206_71_638595283233995644 = Quaternion.identity; newPosition = Handles.FreeMoveHandle(rightHandlePosition, 0.2f, Vector3.one * 0.5f, Handles.DotHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(controller, "Change size of graph");
                controller.width += (newPosition.x - rightHandlePosition.x) / 2;
            }

            EditorGUI.BeginChangeCheck();
            var fmh_214_69_638595283234007001 = Quaternion.identity; newPosition = Handles.FreeMoveHandle(topHandlePosition, 0.2f, Vector3.one * 0.5f, Handles.DotHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(controller, "Change size of graph");
                controller.height += (newPosition.y - topHandlePosition.y) / 2;
            }

            EditorGUI.BeginChangeCheck();
            var fmh_222_72_638595283234014565 = Quaternion.identity; newPosition = Handles.FreeMoveHandle(bottomHandlePosition, 0.2f, Vector3.one * 0.5f, Handles.DotHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(controller, "Change size of graph");
                controller.height -= (newPosition.y - bottomHandlePosition.y) / 2;
            }
        }

        void DrawOutline(GraphController controller)
        {
            Handles.color = Color.blue;

            var center = controller.transform.position;
            var left = center.x - controller.width / 2;
            var right = center.x + controller.width / 2;
            var top = center.y + controller.height / 2;
            var bottom = center.y - controller.height / 2;
            var z = 0;

            Vector3[] lines = new Vector3[8];

            lines[0] = new Vector3(left, top, z);
            lines[1] = new Vector3(right, top, z);

            lines[2] = new Vector3(right, top, z);
            lines[3] = new Vector3(right, bottom, z);

            lines[4] = new Vector3(right, bottom, z);
            lines[5] = new Vector3(left, bottom, z);

            lines[6] = new Vector3(left, bottom, z);
            lines[7] = new Vector3(left, top, z);

            Handles.DrawLines(lines);
        }
    }
#endif

}
