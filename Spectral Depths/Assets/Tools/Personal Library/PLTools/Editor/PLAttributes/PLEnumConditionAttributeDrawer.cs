using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpectralDepths.Tools
{
	// original implementation by http://www.brechtos.com/hiding-or-disabling-inspector-properties-using-propertydrawers-within-unity-5/
	[CustomPropertyDrawer(typeof(PLEnumConditionAttribute))]
	public class PLEnumConditionAttributeDrawer : PropertyDrawer
	{
		#if  UNITY_EDITOR
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			PLEnumConditionAttribute enumConditionAttribute = (PLEnumConditionAttribute)attribute;
			bool enabled = GetConditionAttributeResult(enumConditionAttribute, property);
			bool previouslyEnabled = GUI.enabled;
			GUI.enabled = enabled;
			if (!enumConditionAttribute.Hidden || enabled)
			{
				EditorGUI.PropertyField(position, property, label, true);
			}
			GUI.enabled = previouslyEnabled;
		}
		#endif

		private static Dictionary<string, string> cachedPaths = new Dictionary<string, string>();

		private bool GetConditionAttributeResult(PLEnumConditionAttribute enumConditionAttribute, SerializedProperty property)
		{
			bool enabled = true;

			SerializedProperty enumProp;
			string enumPropPath = string.Empty;
			string propertyPath = property.propertyPath;

			if (!cachedPaths.TryGetValue(propertyPath, out enumPropPath))
			{
				enumPropPath = propertyPath.Replace(property.name, enumConditionAttribute.ConditionEnum);
				cachedPaths.Add(propertyPath, enumPropPath);
			}

			enumProp = property.serializedObject.FindProperty(enumPropPath);

			if (enumProp != null)
			{
				int currentEnum = enumProp.enumValueIndex;
				enabled = enumConditionAttribute.ContainsBitFlag(currentEnum);
			}
			else
			{
				Debug.LogWarning("No matching boolean found for ConditionAttribute in object: " + enumConditionAttribute.ConditionEnum);
			}

			return enabled;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			PLEnumConditionAttribute enumConditionAttribute = (PLEnumConditionAttribute)attribute;
			bool enabled = GetConditionAttributeResult(enumConditionAttribute, property);

			if (!enumConditionAttribute.Hidden || enabled)
			{
				return EditorGUI.GetPropertyHeight(property, label);
			}
			else
			{
				return -EditorGUIUtility.standardVerticalSpacing;
			}
		}
	}
}