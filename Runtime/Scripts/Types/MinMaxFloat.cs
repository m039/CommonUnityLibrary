using UnityEditor;
using UnityEngine;

namespace m039.Common
{
    [System.Serializable]
    public struct MinMaxFloat
    {
        public float min;
        public float max;

        public MinMaxFloat(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        public readonly float Random()
        {
            if (min > max)
                return max;

            return UnityEngine.Random.Range(min, max + 1);
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(MinMaxFloat))]
    public class MinMaxFloatPropertyDrawer : PropertyDrawer
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

