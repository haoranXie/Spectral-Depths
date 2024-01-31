using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A test class used to demonstrate the PLObservable pattern in the PLObservableDemo scene
	/// This one disables itself on Awake, and passively listens for changes, even when disabled
	/// </summary>
	public class PLObservableDemoObserverAutoSleep : MonoBehaviour
	{
		public PLObservableDemoSubject TargetSubject;

		protected virtual void OnSpeedChange()
		{
			this.transform.position = this.transform.position.PLSetY(TargetSubject.PositionX.Value);
		}

		/// <summary>
		/// On awake we start listening for changes
		/// </summary>
		protected virtual void Awake()
		{
			TargetSubject.PositionX.OnValueChanged += OnSpeedChange;
			this.enabled = false;
		}

		/// <summary>
		/// On destroy we stop listening for changes
		/// </summary>
		protected virtual void OnDestroy()
		{
			TargetSubject.PositionX.OnValueChanged -= OnSpeedChange;
		}

		/// <summary>
		/// On enable we do nothing
		/// </summary>
		protected virtual void OnEnable()
		{

		}

		/// <summary>
		/// On disable we do nothing
		/// </summary>
		protected virtual void OnDisable()
		{

		}
	}
}