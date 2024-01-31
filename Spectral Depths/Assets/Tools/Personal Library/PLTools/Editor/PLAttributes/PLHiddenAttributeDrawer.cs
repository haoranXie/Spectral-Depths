using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEditor;

namespace SpectralDepths.Tools
{	

	[CustomPropertyDrawer(typeof(PLHiddenAttribute))]

	public class PLHiddenAttributeDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return 0f;
		}
		
		#if  UNITY_EDITOR
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
	       
		}
		#endif
	}
}