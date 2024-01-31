using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A custom editor displaying a foldable list of PLFeedbacks, a dropdown to add more, as well as test buttons to test your feedbacks at runtime
	/// </summary>
	[CanEditMultipleObjects]
	[CustomEditor(typeof(PLPlotter), true)]
	public class PLPlotterEditor : Editor
	{
		protected string[] _typeDisplays;
		protected string[] _excludedProperties = new string[] { "TweenMethod", "m_Script" }; 

		protected PLPlotter _mmPlotter;

		protected virtual void OnEnable()
		{
			_mmPlotter = target as PLPlotter;
			_typeDisplays = _mmPlotter.GetMethodsList();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			Undo.RecordObject(target, "Modified Plotter");

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Tween Method", EditorStyles.boldLabel);

			_mmPlotter.TweenMethodIndex = EditorGUILayout.Popup("Tween Method", _mmPlotter.TweenMethodIndex, _typeDisplays, EditorStyles.popup);

			//int newItem = EditorGUILayout.Popup(0, _typeDisplays) - 1;
			//DrawDefaultInspector();
			DrawPropertiesExcluding(serializedObject, _excludedProperties);

			if (GUILayout.Button("Draw Graph"))
			{
				_mmPlotter.DrawGraph();
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}