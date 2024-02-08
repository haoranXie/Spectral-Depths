using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Tools;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// An Action that will set the target to null, resetting it
	/// </summary>
	[AddComponentMenu("Spectral Depths/Character/AI/Actions/AIActionResetTarget")]
	public class AIActionResetTarget : AIAction
	{
		/// <summary>
		/// we reset our target
		/// </summary>
		public override void PerformAction()
		{
			_brain.Target = null;
		}
	}
}