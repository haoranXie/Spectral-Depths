using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using UnityEditor;

namespace SpectralDepths.Tools
{	
	public static class PLAchievementMenu 
	{
		[MenuItem("Tools/Spectral Depths/Reset all achievements", false,21)]
		/// <summary>
		/// Adds a menu item to enable help
		/// </summary>
		private static void EnableHelpInInspectors()
		{
			PLAchievementManager.ResetAllAchievements ();
		}
	}
}