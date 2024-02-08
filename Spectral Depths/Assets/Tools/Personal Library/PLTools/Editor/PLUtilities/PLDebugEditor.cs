using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using UnityEditor;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// An editor class used to display menu items 
	/// </summary>
	public class PLDebugEditor
	{
		/// <summary>
		/// Adds a menu item to enable debug logs
		/// </summary>
		[MenuItem("Tools/Spectral Depths/Enable Debug Logs", false, 100)]
		private static void EnableDebugLogs()
		{
			PLDebug.SetDebugLogsEnabled(true);
		}

		/// <summary>
		/// Conditional method to determine if the "enable debug log" entry should be greyed or not
		/// </summary>
		[MenuItem("Tools/Spectral Depths/Enable Debug Logs", true)]
		private static bool EnableDebugLogsValidation()
		{
			return !PLDebug.DebugLogsEnabled;
		}

		/// <summary>
		/// Adds a menu item to disable debug logs
		/// </summary>
		[MenuItem("Tools/Spectral Depths/Disable Debug Logs", false, 101)]
		private static void DisableDebugLogs()
		{
			PLDebug.SetDebugLogsEnabled(false);
		}

		/// <summary>
		/// Conditional method to determine if the "disable debug log" entry should be greyed or not
		/// </summary>
		[MenuItem("Tools/Spectral Depths/Disable Debug Logs", true)]
		private static bool DisableDebugLogsValidation()
		{
			return PLDebug.DebugLogsEnabled;
		}

		/// <summary>
		/// Adds a menu item to enable debug logs
		/// </summary>
		[MenuItem("Tools/Spectral Depths/Enable Debug Draws", false, 102)]
		private static void EnableDebugDraws()
		{
			PLDebug.SetDebugDrawEnabled(true);
		}

		[MenuItem("Tools/Spectral Depths/Enable Debug Draws", true)]
		/// <summary>
		/// Conditional method to determine if the "enable debug log" entry should be greyed or not
		/// </summary>
		private static bool EnableDebugDrawsValidation()
		{
			return !PLDebug.DebugDrawEnabled;
		}

		[MenuItem("Tools/Spectral Depths/Disable Debug Draws", false, 103)]
		/// <summary>
		/// Adds a menu item to disable debug logs
		/// </summary>
		private static void DisableDebugDraws()
		{
			PLDebug.SetDebugDrawEnabled(false);
		}

		[MenuItem("Tools/Spectral Depths/Disable Debug Draws", true)]
		/// <summary>
		/// Conditional method to determine if the "disable debug log" entry should be greyed or not
		/// </summary>
		private static bool DisableDebugDrawsValidation()
		{
			return PLDebug.DebugDrawEnabled;
		}

	}
}