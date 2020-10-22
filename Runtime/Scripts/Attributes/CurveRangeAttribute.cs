using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace m039.Common
{

	public class CurveRangeAttribute : PropertyAttribute
	{
		internal readonly Rect range;

		public CurveRangeAttribute(float x = 0, float y = 0, float width = 1, float height = 1)
		{
			this.range = new Rect(x, y, width, height);
		}
	}

#if UNITY_EDITOR

	[CustomPropertyDrawer(typeof(CurveRangeAttribute))]
	public class CurveRangePropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.AnimationCurve)
			{
				EditorGUI.LabelField(position, label.text, "Use CurveRangeAttribute only with animation curves.");
				return;
			}

			CurveRangeAttribute attr = (CurveRangeAttribute)attribute;

			property.animationCurveValue = EditorGUI.CurveField(position, label, property.animationCurveValue, Color.green, attr.range);
		}
	}

#endif

}
