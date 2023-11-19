using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// Coin manager
	/// </summary>
	[AddComponentMenu("Spectral Depths/Items/Coin")]
	public class Coin : PickableItem
	{
		/// The amount of points to add when collected
		[Tooltip("The amount of points to add when collected")]
		public int PointsToAdd = 10;

		/// <summary>
		/// Triggered when something collides with the coin
		/// </summary>
		/// <param name="collider">Other.</param>
		protected override void Pick(GameObject picker) 
		{
			// we send a new points event for the GameManager to catch (and other classes that may listen to it too)
			TopDownEnginePointEvent.Trigger(PointsMethods.Add, PointsToAdd);
		}
	}
}