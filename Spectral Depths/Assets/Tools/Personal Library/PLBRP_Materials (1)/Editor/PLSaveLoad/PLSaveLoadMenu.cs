using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using UnityEditor;

namespace SpectralDepths.Tools
{	
	/// <summary>
	/// Adds a dedicated Tools menu into the top bar Spectral Depths entry to delete all saved data
	/// </summary>
	public static class PLSaveLoadMenu 
	{
		[MenuItem("Tools/Spectral Depths/Delete all saved data",false,31)]
		/// <summary>
		/// Adds a menu item to reset all data saved by the PLSaveLoadManager. No turning back.
		/// </summary>
		private static void ResetAllSavedInventories()
		{
			PLSaveLoadManager.DeleteAllSaveFiles();
			Debug.LogFormat ("All Save Files Deleted");
		}
	}
}