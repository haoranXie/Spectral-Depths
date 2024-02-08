using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using UnityEditor;

namespace SpectralDepths.Tools
{	
	/// <summary>
	/// This class adds a SpectralDepths entry in Unity's top menu, allowing to enable/disable the help texts from the engine's inspectors
	/// </summary>
	public static class PLMenuHelp
	{
		[MenuItem("Tools/Spectral Depths/Enable Help in Inspectors", false,0)]
		/// <summary>
		/// Adds a menu item to enable help
		/// </summary>
		private static void EnableHelpInInspectors()
		{
			SetHelpEnabled(true);
		}

		[MenuItem("Tools/Spectral Depths/Enable Help in Inspectors", true)]
		/// <summary>
		/// Conditional method to determine if the "enable help" entry should be greyed or not
		/// </summary>
		private static bool EnableHelpInInspectorsValidation()
		{
			return !HelpEnabled();
		}

		[MenuItem("Tools/Spectral Depths/Disable Help in Inspectors", false,1)]
		/// <summary>
		/// Adds a menu item to disable help
		/// </summary>
		private static void DisableHelpInInspectors()
		{
			SetHelpEnabled(false);
		}
		 
		[MenuItem("Tools/Spectral Depths/Disable Help in Inspectors", true)]
		/// <summary>
		/// Conditional method to determine if the "disable help" entry should be greyed or not
		/// </summary>
		private static bool DisableHelpInInspectorsValidation()
		{
			return HelpEnabled();
		}

		/// <summary>
		/// Checks editor prefs to see if help is enabled or not
		/// </summary>
		/// <returns><c>true</c>, if enabled was helped, <c>false</c> otherwise.</returns>
		private static bool HelpEnabled()
		{
			if (EditorPrefs.HasKey("PLShowHelpInInspectors"))
			{
				return EditorPrefs.GetBool("PLShowHelpInInspectors");
			}
			else
			{
				EditorPrefs.SetBool("PLShowHelpInInspectors",true);
				return true;
			}
		}

		/// <summary>
		/// Sets the help enabled editor pref.
		/// </summary>
		/// <param name="status">If set to <c>true</c> status.</param>
		private static void SetHelpEnabled(bool status)
		{
			EditorPrefs.SetBool("PLShowHelpInInspectors",status);
			SceneView.RepaintAll();

		}
	}
}