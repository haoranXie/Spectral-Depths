using UnityEngine;
#if UNITY_EDITOR
using System.Collections;
using SpectralDepths.Tools;
using UnityEditor;

namespace SpectralDepths.InventoryEngine
{	
	/// <summary>
	/// Adds a dedicated InventoryEngine menu into the top bar Spectral Depths entry
	/// </summary>
	public static class InventoryEngineMenu 
	{
		const string _saveFolderName = "InventoryEngine"; 

		[MenuItem("Tools/Spectral Depths/Reset all saved inventories",false,31)]
		/// <summary>
		/// Adds a menu item to reset all saved inventories directly from Unity. 
		/// This will remove the whole PLData/InventoryEngine folder, use it with caution.
		/// </summary>
		private static void ResetAllSavedInventories()
		{
			PLSaveLoadManager.DeleteSaveFolder (_saveFolderName);
			Debug.LogFormat ("Inventories Save Files Reset");
		}
	}
}
#endif