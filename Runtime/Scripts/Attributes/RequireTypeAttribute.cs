using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace m039.Common
{

    //
    // The code is borrowed from:
    //
    // - https://www.patrykgalach.com/2020/01/27/assigning-interface-in-unity-inspector/?cn-reloaded=1
    // - https://bitbucket.org/gaello/interface-in-inspector/src/master/
    //

    public class RequireTypeAttribute : PropertyAttribute
    {
        public readonly System.Type requiredType;

        public RequireTypeAttribute(System.Type type)
        {
            requiredType = type;
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(RequireTypeAttribute))]
    public class RequireTypeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.ObjectReference)
            {
                var requiredAttribute = this.attribute as RequireTypeAttribute;

                EditorGUI.BeginProperty(position, label, property);
                property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, requiredAttribute.requiredType, true);
                EditorGUI.EndProperty();
            }
            else
            {
                var previousColor = GUI.color;

                GUI.color = Color.red;
                EditorGUI.LabelField(position, label, new GUIContent("Property is not a reference type"));
                GUI.color = previousColor;
            }
        }
    }

#endif

}
