using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A test class used to demonstrate how PLObservable works in the PLObservableTest demo scene  
	/// </summary>
	public class PLObservableDemoSubject : MonoBehaviour
	{
		/// a public float we expose, outputting the x position of our object
		public PLObservable<float> PositionX = new PLObservable<float>();

		/// <summary>
		/// On Update we update our x position
		/// </summary>
		protected virtual void Update()
		{
			PositionX.Value = this.transform.position.x;
		}
	}
}