using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A test class used to demonstrate the PLObservable in the PLObservableTest demo scene    
	/// </summary>
	public class PLObservableDemoObserver : MonoBehaviour
	{
		/// the subject to look at
		public PLObservableDemoSubject TargetSubject;    

		/// <summary>
		/// When the position changes, we move our object accordingly on the y axis
		/// </summary>
		protected virtual void OnPositionChange()
		{
			this.transform.position = this.transform.position.PLSetY(TargetSubject.PositionX.Value);
		}
        
		/// <summary>
		/// On enable we start listening for changes
		/// </summary>
		protected virtual void OnEnable()
		{
			TargetSubject.PositionX.OnValueChanged += OnPositionChange;
		}

		/// <summary>
		/// On enable we stop listening for changes
		/// </summary>
		protected virtual void OnDisable()
		{
			TargetSubject.PositionX.OnValueChanged -= OnPositionChange;
		}
	}
}