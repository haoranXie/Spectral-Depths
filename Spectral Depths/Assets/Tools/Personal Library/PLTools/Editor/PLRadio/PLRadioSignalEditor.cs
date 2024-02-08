using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpectralDepths.Tools
{
	[CustomEditor(typeof(PLRadioSignal), true)]
	[CanEditMultipleObjects]
	public class PLRadioSignalEditor : Editor
	{
		protected PLRadioSignal _radioSignal;

		protected float _inspectorWidth;
        
		protected SerializedProperty _duration;
		protected SerializedProperty _currentLevel;

		public override bool RequiresConstantRepaint()
		{
			return true;
		}

		protected virtual void OnEnable()
		{
			_radioSignal = target as PLRadioSignal;
			_duration = serializedObject.FindProperty("Duration");
			_currentLevel = serializedObject.FindProperty("CurrentLevel");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			_inspectorWidth = EditorGUIUtility.currentViewWidth - 24;

			DrawProperties();

			serializedObject.ApplyModifiedProperties();
		}

		protected virtual void DrawProperties()
		{
			DrawPropertiesExcluding(serializedObject, "AnimatedPreview", "CurrentLevel");
		}

        
	}
}