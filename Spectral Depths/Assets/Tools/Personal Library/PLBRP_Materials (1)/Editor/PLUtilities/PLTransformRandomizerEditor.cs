using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// Custom editor for the PLTransformRandomizer class
	/// </summary>
	[CustomEditor(typeof(PLTransformRandomizer), true)]
	[CanEditMultipleObjects]
	public class PLTransformRandomizerEditor : Editor
	{
		/// <summary>
		/// On inspector we handle undo and display a test button
		/// </summary>
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			Undo.RecordObject(target, "Modified PLTransformRandomizer");
			DrawDefaultInspector();

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Test", EditorStyles.boldLabel);

			if (GUILayout.Button("Randomize"))
			{
				foreach (PLTransformRandomizer randomizer in targets)
				{
					randomizer.Randomize();
				}
			}
		}
	}
}