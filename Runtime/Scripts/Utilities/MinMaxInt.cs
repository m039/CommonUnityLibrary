using UnityEditor;
using UnityEngine;

namespace m039.Common
{
    [System.Serializable]
    public struct MinMaxInt
    {
        public int min;
        public int max;

        public MinMaxInt(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public readonly int Random()
        {
            if (min > max)
                return max;

            return UnityEngine.Random.Range(min, max + 1);
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(MinMaxInt))]
    public class MinMaxIntPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            const float LabelWidth = 40;
            const float Gap = 5;
                        
            var minProperty = property.FindPropertyRelative("min");
            var maxProperty = property.FindPropertyRelative("max");

            var minRect = new Rect(position.x, position.y, position.width / 2 - Gap, position.height);
            var maxRect = new Rect(minRect.xMax + Gap, position.y, position.width / 2, position.height);

            float labelWidthTmp = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = LabelWidth;
            EditorGUI.PropertyField(minRect, minProperty, new GUIContent("MIN"));
            EditorGUI.PropertyField(maxRect, maxProperty, new GUIContent("MAX"));

            EditorGUIUtility.labelWidth = labelWidthTmp;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
#endif
}

