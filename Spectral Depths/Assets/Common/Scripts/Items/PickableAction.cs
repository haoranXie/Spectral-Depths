using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using UnityEngine.Events;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// Add this class to an object and it'll trigger the specified actions on pick
	/// </summary>
	[AddComponentMenu("Spectral Depths/Items/Pickable Action")]
	public class PickableAction : PickableItem
	{
		/// the action(s) to trigger when picked
		[Tooltip("the action(s) to trigger when picked")]
		public UnityEvent PickEvent;

		/// <summary>
		/// Triggered when something collides with the object
		/// </summary>
		/// <param name="collider">Other.</param>
		protected override void Pick(GameObject picker)
		{
			base.Pick(picker);
			if (PickEvent != null)
			{
				PickEvent.Invoke();
			}
		}
	}
}