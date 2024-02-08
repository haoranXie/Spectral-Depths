using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// Add this class to a UI object to have it act as a raycast target without needing an Image component
	/// </summary>
	[AddComponentMenu("Spectral Depths/Tools/GUI/PLRaycastTarget")]
	public class PLRaycastTarget : Graphic
	{
		public override void SetVerticesDirty() { return; }
		public override void SetMaterialDirty() { return; }

		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			return;
		}
	}
}