using UnityEngine;
using UnityEditor;

namespace m039.Common
{
    // The code is borrowed from <see href="https://answers.unity.com/questions/1758798/is-there-anyway-to-batch-renaming-via-editor-not-o.html">here</see>.
    public class RenameChildren : EditorWindow
    {
        static readonly Vector2Int _sSize = new Vector2Int(250, 100);

        string _childrenPrefix;

        int _startIndex;

        [MenuItem("GameObject/Rename Children")]
        public static void ShowWindow()
        {
            EditorWindow window = GetWindow<RenameChildren>();
            window.minSize = _sSize;
            window.maxSize = _sSize;
        }

        private void OnGUI()
        {
            _childrenPrefix = EditorGUILayout.TextField("Children Prefix", _childrenPrefix);
            _startIndex = EditorGUILayout.IntField("Start Index", _startIndex);

            if (GUILayout.Button("Rename Children"))
            {
                GameObject[] selectedObjects = Selection.gameObjects;
                for (int objectI = 0; objectI < selectedObjects.Length; objectI++)
                {
                    Transform selectedObjectT = selectedObjects[objectI].transform;

                    for (int childI = 0, i = _startIndex; childI < selectedObjectT.childCount; childI++)
                        selectedObjectT.GetChild(childI).name = $"{_childrenPrefix}({i++})";
                }
            }
        }
    }

}
