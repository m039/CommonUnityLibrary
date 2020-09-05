/* MIT License
 *
 * Copyright (c) 2018 Andrew Rumak
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS 
 * IN THE SOFTWARE.
 */

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;
using System.Reflection;

namespace m039.Common
{

	/// <summary>
	/// Conditionally Show/Hide field in inspector, based on some other field value 
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class ConditionalFieldAttribute : PropertyAttribute
	{
		public readonly string FieldToCheck;
		public readonly string[] CompareValues;
		public readonly bool Inverse;

		/// <param name="fieldToCheck">String name of field to check value</param>
		/// <param name="inverse">Inverse check result</param>
		/// <param name="compareValues">On which values field will be shown in inspector</param>
		public ConditionalFieldAttribute(string fieldToCheck, bool inverse = false, params object[] compareValues)
		{
			FieldToCheck = fieldToCheck;
			Inverse = inverse;
			CompareValues = compareValues.Select(c => c.ToString().ToUpper()).ToArray();
		}
	}
}

#if UNITY_EDITOR
namespace m039.Common.Internal
{
	using UnityEditor;

    public static class ConditionaFieldExtensions {

    	public static string AsStringValue(this SerializedProperty property)
		{
			switch (property.propertyType)
			{
				case SerializedPropertyType.String:
					return property.stringValue;

				case SerializedPropertyType.Character:
				case SerializedPropertyType.Integer:
					if (property.type == "char") return Convert.ToChar(property.intValue).ToString();
					return property.intValue.ToString();

				case SerializedPropertyType.ObjectReference:
					return property.objectReferenceValue != null ? property.objectReferenceValue.ToString() : "null";

				case SerializedPropertyType.Boolean:
					return property.boolValue.ToString();

				case SerializedPropertyType.Enum:
					return property.enumNames[property.enumValueIndex];

				default:
					return string.Empty;
			}
		}

        /// <summary>
		/// Is list null or empty
		/// </summary>
		public static bool IsNullOrEmpty<T>(this IList<T> collection)
		{
			if (collection == null) return true;

			return collection.Count == 0;
		}
    }

	[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
	public class ConditionalFieldAttributeDrawer : PropertyDrawer
	{
		private ConditionalFieldAttribute Conditional => _conditional ?? (_conditional = attribute as ConditionalFieldAttribute);
		private ConditionalFieldAttribute _conditional;
		
		private bool _customDrawersCached;
		private static IEnumerable<Type> _allPropertyDrawerAttributeTypes;
		private bool _multipleAttributes;
		private bool _specialType;
		private PropertyAttribute _genericAttribute;
		private PropertyDrawer _genericAttributeDrawerInstance;
		private Type _genericAttributeDrawerType;
		private Type _genericType;
		private PropertyDrawer _genericTypeDrawerInstance;
		private Type _genericTypeDrawerType;


		/// <summary>
		/// If conditional is part of type in collection, we need to link properties as in collection
		/// </summary>
		private readonly Dictionary<SerializedProperty, SerializedProperty> _conditionalToTarget =
			new Dictionary<SerializedProperty, SerializedProperty>();

		private bool _toShow = true;


		private void Initialize(SerializedProperty property)
		{
			if (!_conditionalToTarget.ContainsKey(property))
				_conditionalToTarget.Add(property, ConditionalFieldUtility.FindRelativeProperty(property, Conditional.FieldToCheck));

			if (_customDrawersCached) return;
			if (_allPropertyDrawerAttributeTypes == null)
			{
				_allPropertyDrawerAttributeTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
					.Where(x => typeof(PropertyDrawer).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
			}

			if (HaveMultipleAttributes())
			{
				_multipleAttributes = true;
				GetPropertyDrawerType(property);
			}
			else if (fieldInfo != null && !fieldInfo.FieldType.Module.ScopeName.Equals(typeof(int).Module.ScopeName))
			{
				_specialType = true;
				GetTypeDrawerType(property);
			}

			_customDrawersCached = true;
		}

		private bool HaveMultipleAttributes()
		{
			if (fieldInfo == null) return false;
			var genericAttributeType = typeof(PropertyAttribute);
			var attributes = fieldInfo.GetCustomAttributes(genericAttributeType, false);
			if (attributes.IsNullOrEmpty()) return false;
			return attributes.Length > 1;
		}


		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			Initialize(property);

			_toShow = ConditionalFieldUtility.PropertyIsVisible(_conditionalToTarget[property], Conditional.Inverse, Conditional.CompareValues);
			if (!_toShow) return 0;

			if (_genericAttributeDrawerInstance != null)
				return _genericAttributeDrawerInstance.GetPropertyHeight(property, label);

			if (_genericTypeDrawerInstance != null)
				return _genericTypeDrawerInstance.GetPropertyHeight(property, label);
			
			return EditorGUI.GetPropertyHeight(property);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!_toShow) return;

			if (_multipleAttributes && _genericAttributeDrawerInstance != null)
			{
				try
				{
					_genericAttributeDrawerInstance.OnGUI(position, property, label);
				}
				catch (Exception e)
				{
					EditorGUI.PropertyField(position, property, label);
					LogWarning("Unable to instantiate " + _genericAttribute.GetType() + " : " + e, property);
				}
			}
			else if (_specialType && _genericTypeDrawerInstance != null)
			{
				try
				{
					_genericTypeDrawerInstance.OnGUI(position, property, label);
				}
				catch (Exception e)
				{
					EditorGUI.PropertyField(position, property, label);
					LogWarning("Unable to instantiate " + _genericType + " : " + e, property);
				}
			}
			else
			{
				EditorGUI.PropertyField(position, property, label, true);
			}
		}

		private void LogWarning(string log, SerializedProperty property)
		{
			var warning = "Property <color=brown>" + fieldInfo.Name + "</color>";
			if (fieldInfo != null && fieldInfo.DeclaringType != null)
				warning += " on behaviour <color=brown>" + fieldInfo.DeclaringType.Name + "</color>";
			warning += " caused: " + log;

#if UNUSED
			WarningsPool.LogWarning(warning, property.serializedObject.targetObject);
#endif
		}

#region Get Custom Property/Type drawers

		private void GetPropertyDrawerType(SerializedProperty property)
		{
			if (_genericAttributeDrawerInstance != null) return;

			//Get the second attribute flag
			try
			{
				_genericAttribute = (PropertyAttribute) fieldInfo.GetCustomAttributes(typeof(PropertyAttribute), false)
					.FirstOrDefault(a => !(a is ConditionalFieldAttribute));

				//TODO: wtf man
				if (_genericAttribute is ContextMenuItemAttribute
#if UNUSED
                || _genericAttribute is SeparatorAttribute | _genericAttribute is AutoPropertyAttribute
#endif
                )
				{
					LogWarning("[ConditionalField] does not work with " + _genericAttribute.GetType(), property);
					return;
				}

				if (_genericAttribute is TooltipAttribute) return;
			}
			catch (Exception e)
			{
				LogWarning("Can't find stacked propertyAttribute after ConditionalProperty: " + e, property);
				return;
			}

			//Get the associated attribute drawer
			try
			{
				_genericAttributeDrawerType = _allPropertyDrawerAttributeTypes.First(x =>
					(Type) CustomAttributeData.GetCustomAttributes(x).First().ConstructorArguments.First().Value == _genericAttribute.GetType());
			}
			catch (Exception e)
			{
				LogWarning("Can't find property drawer from CustomPropertyAttribute of " + _genericAttribute.GetType() + " : " + e, property);
				return;
			}

			//Create instances of each (including the arguments)
			try
			{
				_genericAttributeDrawerInstance = (PropertyDrawer) Activator.CreateInstance(_genericAttributeDrawerType);
				//Get arguments
				IList<CustomAttributeTypedArgument> attributeParams = fieldInfo.GetCustomAttributesData()
				.First(a => a.AttributeType == _genericAttribute.GetType()).ConstructorArguments;
				IList<CustomAttributeTypedArgument> unpackedParams = new List<CustomAttributeTypedArgument>();
				//Unpack any params object[] args
				foreach (CustomAttributeTypedArgument singleParam in attributeParams)
				{
					if (singleParam.Value.GetType() == typeof(ReadOnlyCollection<CustomAttributeTypedArgument>))
					{
						foreach (CustomAttributeTypedArgument unpackedSingleParam in (ReadOnlyCollection<CustomAttributeTypedArgument>) singleParam
							.Value)
						{
							unpackedParams.Add(unpackedSingleParam);
						}
					}
					else
					{
						unpackedParams.Add(singleParam);
					}
				}

				object[] attributeParamsObj = unpackedParams.Select(x => x.Value).ToArray();

				if (attributeParamsObj.Any())
				{
					_genericAttribute = (PropertyAttribute) Activator.CreateInstance(_genericAttribute.GetType(), attributeParamsObj);
				}
				else
				{
					_genericAttribute = (PropertyAttribute) Activator.CreateInstance(_genericAttribute.GetType());
				}
			}
			catch (Exception e)
			{
				LogWarning("No constructor available in " + _genericAttribute.GetType() + " : " + e, property);
				return;
			}

			//Reassign the attribute field in the drawer so it can access the argument values
			try
			{
				var genericDrawerAttributeField = _genericAttributeDrawerType.GetField("m_Attribute", BindingFlags.Instance | BindingFlags.NonPublic);
				genericDrawerAttributeField.SetValue(_genericAttributeDrawerInstance, _genericAttribute);
			}
			catch (Exception e)
			{
				LogWarning("Unable to assign attribute to " + _genericAttributeDrawerInstance.GetType() + " : " + e, property);
			}
		}


		private void GetTypeDrawerType(SerializedProperty property)
		{
			if (_genericTypeDrawerInstance != null) return;

			//Get the associated attribute drawer
			try
			{
				// Of all property drawers in the assembly we need to find one that affects target type
				// or one of the base types of target type
				foreach (Type propertyDrawerType in _allPropertyDrawerAttributeTypes)
				{
					_genericType = fieldInfo.FieldType;
					var affectedType = (Type) CustomAttributeData.GetCustomAttributes(propertyDrawerType).First().ConstructorArguments.First().Value;
					while (_genericType != null)
					{
						if (_genericTypeDrawerType != null) break;
						if (affectedType == _genericType) _genericTypeDrawerType = propertyDrawerType;
						else _genericType = _genericType.BaseType;
					}
					if (_genericTypeDrawerType != null) break;
				}
			}
			catch (Exception)
			{
				// Commented out because of multiple false warnings on Behaviour types
				//LogWarning("[ConditionalField] does not work with "+_genericType+". Unable to find property drawer from the Type", property);
				return;
			}
			if (_genericTypeDrawerType == null) return;
			
			//Create instances of each (including the arguments)
			try
			{
				_genericTypeDrawerInstance = (PropertyDrawer) Activator.CreateInstance(_genericTypeDrawerType);
			}
			catch (Exception e)
			{
				LogWarning("no constructor available in " + _genericType + " : " + e, property);
				return;
			}

			//Reassign the attribute field in the drawer so it can access the argument values
			try
			{
				_genericTypeDrawerType.GetField("m_Attribute", BindingFlags.Instance | BindingFlags.NonPublic)
					.SetValue(_genericTypeDrawerInstance, fieldInfo);
			}
			catch (Exception)
			{
				//LogWarning("Unable to assign attribute to " + _genericTypeDrawerInstance.GetType() + " : " + e, property);
			}
		}

#endregion
	}

	public static class ConditionalFieldUtility
	{
#region Property Is Visible

		public static bool PropertyIsVisible(SerializedProperty property, bool inverse, string[] compareAgainst)
		{
			if (property == null) return true;

			string asString = property.AsStringValue().ToUpper();

			if (compareAgainst != null && compareAgainst.Length > 0)
			{
				var matchAny = CompareAgainstValues(asString, compareAgainst);
				if (inverse) matchAny = !matchAny;
				return matchAny;
			}

			bool someValueAssigned = asString != "FALSE" && asString != "0" && asString != "NULL";
			if (someValueAssigned) return !inverse;

			return inverse;
		}

		/// <summary>
		/// True if the property value matches any of the values in '_compareValues'
		/// </summary>
		private static bool CompareAgainstValues(string propertyValueAsString, string[] compareAgainst)
		{
			for (var i = 0; i < compareAgainst.Length; i++)
			{
				bool valueMatches = compareAgainst[i] == propertyValueAsString;

				// One of the value is equals to the property value.
				if (valueMatches) return true;
			}

			// None of the value is equals to the property value.
			return false;
		}

#endregion


#region Find Relative Property

		public static SerializedProperty FindRelativeProperty(SerializedProperty property, string propertyName)
		{
			if (property.depth == 0) return property.serializedObject.FindProperty(propertyName);

			var path = property.propertyPath.Replace(".Array.data[", "[");
			var elements = path.Split('.');

			var nestedProperty = NestedPropertyOrigin(property, elements);

			// if nested property is null = we hit an array property
			if (nestedProperty == null)
			{
				var cleanPath = path.Substring(0, path.IndexOf('['));
				var arrayProp = property.serializedObject.FindProperty(cleanPath);
				var target = arrayProp.serializedObject.targetObject;

				var who = "Property <color=brown>" + arrayProp.name + "</color> in object <color=brown>" + target.name + "</color> caused: ";
				var warning = who + "Array fields is not supported by [ConditionalFieldAttribute]";

#if UNUSED
				WarningsPool.LogWarning(warning, target);
#endif

				return null;
			}

			return nestedProperty.FindPropertyRelative(propertyName);
		}

		// For [Serialized] types with [Conditional] fields
		private static SerializedProperty NestedPropertyOrigin(SerializedProperty property, string[] elements)
		{
			SerializedProperty parent = null;

			for (int i = 0; i < elements.Length - 1; i++)
			{
				var element = elements[i];
				int index = -1;
				if (element.Contains("["))
				{
					index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal))
						.Replace("[", "").Replace("]", ""));
					element = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
				}

				parent = i == 0
					? property.serializedObject.FindProperty(element)
					: parent != null
						? parent.FindPropertyRelative(element)
						: null;

				if (index >= 0 && parent != null) parent = parent.GetArrayElementAtIndex(index);
			}

			return parent;
		}

#endregion

#region Behaviour Property Is Visible

		public static bool BehaviourPropertyIsVisible(MonoBehaviour behaviour, string propertyName, ConditionalFieldAttribute appliedAttribute)
		{
			if (string.IsNullOrEmpty(appliedAttribute.FieldToCheck)) return true;

			var so = new SerializedObject(behaviour);
			var property = so.FindProperty(propertyName);
			var targetProperty = FindRelativeProperty(property, appliedAttribute.FieldToCheck);

			return PropertyIsVisible(targetProperty, appliedAttribute.Inverse, appliedAttribute.CompareValues);
		}

#endregion
	}
}
#endif
