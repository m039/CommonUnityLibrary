using UnityEditor;
using UnityEngine;

namespace m039.Common.DependencyInjection
{
    [CustomPropertyDrawer(typeof(InjectAttribute))]
    public class InjectPropertyDrawer : PropertyDrawer
    {
        Texture2D icon;

        Texture2D LoadIcon()
        {
            if (icon == null)
            {
                icon = (Texture2D)EditorGUIUtility.Load("Packages/com.m039.common-unity-library/Editor/Images/DI/icon.png");
            }

            return icon;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            icon = LoadIcon();
            var iconRect = new Rect(position.x, position.y, 20, 20);
            position.xMin += 24;
            var oldWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth -= 24;

            if (icon != null)
            {
                var savedColor = GUI.color;
                if (property.propertyType == SerializedPropertyType.ObjectReference)
                {
                    GUI.color = property.objectReferenceValue == null ? savedColor : Color.green;
                }
                GUI.DrawTexture(iconRect, icon);
                GUI.color = savedColor;
            }

            EditorGUI.PropertyField(position, property, label, true);

            EditorGUIUtility.labelWidth = oldWidth;

            EditorGUI.EndProperty();
        }
    }
}
