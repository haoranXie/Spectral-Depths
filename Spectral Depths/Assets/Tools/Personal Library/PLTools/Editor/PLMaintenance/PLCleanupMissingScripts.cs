using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// This class lets you clean all missing scripts on a selection of gameobjects
	/// </summary>
	public class PLCleanupMissingScripts : MonoBehaviour
	{
		/// <summary>
		/// Processes the cleaning of gameobjects for all missing scripts on them
		/// </summary>
		[MenuItem("Tools/Spectral Depths/Cleanup missing scripts on selected GameObjects", false, 504)]
		protected static void CleanupMissingScripts()
		{
			Object[] collectedDeepHierarchy = EditorUtility.CollectDeepHierarchy(Selection.gameObjects);
			int removedComponentsCounter = 0;
			int gameobjectsAffectedCounter = 0;
			foreach (Object targetObject in collectedDeepHierarchy)
			{
				if (targetObject is GameObject gameObject)
				{
					int amountOfMissingScripts = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(gameObject);
					if (amountOfMissingScripts > 0)
					{
						Undo.RegisterCompleteObjectUndo(gameObject, "Removing missing scripts");
						GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);
						removedComponentsCounter += amountOfMissingScripts;
						gameobjectsAffectedCounter++;
					}
				}
			}
			Debug.Log("[PLCleanupMissingScripts] Removed " + removedComponentsCounter + " missing scripts from " + gameobjectsAffectedCounter + " GameObjects");
		}
	}
}