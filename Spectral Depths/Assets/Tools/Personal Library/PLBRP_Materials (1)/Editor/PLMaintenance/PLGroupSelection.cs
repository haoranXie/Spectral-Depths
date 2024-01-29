using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A class used to add a menu item and a shortcut to group objects together under a parent game object
	/// </summary>
	public class PLGroupSelection 
	{
		/// <summary>
		/// Creates a parent object and puts all selected transforms under it
		/// </summary>
		[MenuItem("Tools/Spectral Depths/Group Selection %g")]
		public static void GroupSelection()
		{
			if (!Selection.activeTransform)
			{
				return;
			}

			GameObject groupObject = new GameObject();
			groupObject.name = "Group";

			Undo.RegisterCreatedObjectUndo(groupObject, "Group Selection");

			groupObject.transform.SetParent(Selection.activeTransform.parent, false);

			foreach (Transform selectedTransform in Selection.transforms)
			{
				Undo.SetTransformParent(selectedTransform, groupObject.transform, "Group Selection");
			}
			Selection.activeGameObject = groupObject;
		}
	}
}