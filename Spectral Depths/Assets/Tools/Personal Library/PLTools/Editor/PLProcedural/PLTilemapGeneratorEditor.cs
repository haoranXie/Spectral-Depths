using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// Custom editor for the PLTilemapGenerator, handles generate button and reorderable layers
	/// </summary>
	[CustomEditor(typeof(PLTilemapGenerator), true)]
	[CanEditMultipleObjects]
	public class PLTilemapGeneratorEditor : Editor
	{
    
		protected PLReorderableList _list;

		protected virtual void OnEnable()
		{
			_list = new PLReorderableList(serializedObject.FindProperty("Layers"));
			_list.elementNameProperty = "Layer";
			_list.elementDisplayType = PLReorderableList.ElementDisplayType.Expandable;
		}
        
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
            
			DrawPropertiesExcluding(serializedObject,  "Layers");
			EditorGUILayout.Space(10);
			_list.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
            
			if (GUILayout.Button("Generate"))
			{
				(target as PLTilemapGenerator).Generate();
			}
		}
	}
}