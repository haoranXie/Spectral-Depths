using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// Add this class to an empty object in your scene and it'll act as a point of control to enable or disable logs and debug draws
	/// </summary>
	[AddComponentMenu("Spectral Depths/Tools/Utilities/PLDebugController")]
	public class PLDebugController : MonoBehaviour
	{
		/// whether or not debug logs (PLDebug.DebugLogTime, PLDebug.DebugOnScreen) should be displayed
		public bool DebugLogsEnabled = true;
		/// whether or not debug draws should be executed
		public bool DebugDrawEnabled = true;

		/// <summary>
		/// On Awake we turn our static debug checks on or off
		/// </summary>
		protected virtual void Awake()
		{
			PLDebug.SetDebugLogsEnabled(DebugLogsEnabled);
			PLDebug.SetDebugDrawEnabled(DebugDrawEnabled);
		}
	}
}