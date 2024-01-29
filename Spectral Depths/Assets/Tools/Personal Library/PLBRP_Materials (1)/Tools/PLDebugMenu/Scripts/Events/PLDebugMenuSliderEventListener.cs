using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpectralDepths.Tools
{
	[Serializable]
	public class PLDSliderValueChangedEvent : UnityEvent<float> { }

	/// <summary>
	/// A class used to listen to slider events from a PLDebugMenu
	/// </summary>
	public class PLDebugMenuSliderEventListener : MonoBehaviour
	{
		[Header("Events")]
		/// the name of the slider event to listen to
		public string SliderEventName = "SliderEventName";
		/// an event fired when the slider's value changes
		public PLDSliderValueChangedEvent PLDValueChangedEvent;

		[Header("Test")]
		[Range(0f, 1f)]
		public float TestValue = 1f;
		[PLInspectorButton("TestSetValue")]
		public bool TestSetValueButton;

		/// <summary>
		/// This test methods will send a set event to all sliders bound to the SliderEventName
		/// </summary>
		protected virtual void TestSetValue()
		{
			PLDebugMenuSliderEvent.Trigger(SliderEventName, TestValue, PLDebugMenuSliderEvent.EventModes.SetSlider);
		}

		/// <summary>
		/// When we get a slider event, we trigger an event if needed 
		/// </summary>
		/// <param name="sliderEventName"></param>
		/// <param name="value"></param>
		protected virtual void OnMMDebugMenuSliderEvent(string sliderEventName, float value, PLDebugMenuSliderEvent.EventModes eventMode)
		{
			if ( (eventMode == PLDebugMenuSliderEvent.EventModes.FromSlider) 
			     && (sliderEventName == SliderEventName))
			{
				if (PLDValueChangedEvent != null)
				{
					PLDValueChangedEvent.Invoke(value);
				}
			}
		}

		/// <summary>
		/// Starts listening for events
		/// </summary>
		public virtual void OnEnable()
		{
			PLDebugMenuSliderEvent.Register(OnMMDebugMenuSliderEvent);
		}

		/// <summary>
		/// Stops listening for events
		/// </summary>
		public virtual void OnDisable()
		{
			PLDebugMenuSliderEvent.Unregister(OnMMDebugMenuSliderEvent);
		}
	}
}