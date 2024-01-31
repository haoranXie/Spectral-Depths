using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpectralDepths.Tools
{
	public class PLReadOnlyWhenPlayingAttribute : PropertyAttribute { }


	#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(PLReadOnlyWhenPlayingAttribute))]
	public class ReadOnlyWhenPlayingAttributeDrawer : PropertyDrawer
	{        
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			GUI.enabled = !Application.isPlaying;
			EditorGUI.PropertyField(position, property, label, true);
			GUI.enabled = true;
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}
	}
	#endif
}