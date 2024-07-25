using UnityEditor;
using UnityEngine;

namespace m039.Common
{
    [CustomEditor(typeof(InjectorMonoBehaviour))]
    public class InjectorMonoBehaviourEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var injector = (InjectorMonoBehaviour)target;

            if (GUILayout.Button("Validate Dependencies"))
            {
                injector.ValidateDependencies();
            }

            if (GUILayout.Button("Clear All Injectable Fields"))
            {
                injector.ClearDependencies();
                EditorUtility.SetDirty(injector);
            }
        }
    }
}
