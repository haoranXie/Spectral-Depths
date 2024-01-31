using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using SpectralDepths.Tools;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// Add this class to an object and it'll make sure that the cursor is either visible or invisible
	/// </summary>
	[AddComponentMenu("Spectral Depths/Tools/GUI/PLCursorVisible")]
	public class PLCursorVisible : MonoBehaviour
	{
		/// The possible states of the cursor
		public enum CursorVisibilities { Visible, Invisible }
		/// Whether that cursor should be visible or invisible
		public CursorVisibilities CursorVisibility = CursorVisibilities.Visible;

		/// <summary>
		/// On Update we change the status of our cursor accordingly
		/// </summary>
		protected virtual void Update()
		{
			if (CursorVisibility == CursorVisibilities.Visible)
			{
				Cursor.visible = true;
			}
			else
			{
				Cursor.visible = false;
			}
		}
	}
}