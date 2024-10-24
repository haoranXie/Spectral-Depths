﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpectralDepths.Tools
{
	[CustomPropertyDrawer(typeof(PLColorAttribute))]
	public class PLColorAttributeDrawer : PropertyDrawer
	{
        
		#if  UNITY_EDITOR
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Color color = (attribute as PLColorAttribute).color;
			Color prev = GUI.color;
			GUI.color = color;
			EditorGUI.PropertyField(position, property, label, true);
			GUI.color = prev;
		}
		#endif
	}
}