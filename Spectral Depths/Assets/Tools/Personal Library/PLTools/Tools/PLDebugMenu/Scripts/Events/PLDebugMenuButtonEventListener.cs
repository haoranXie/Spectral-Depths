using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// An event fired when a button gets pressed in a PLDebugMenu
	/// </summary>
	[Serializable]
	public class PLDButtonPressedEvent : UnityEvent
	{
	}

	/// <summary>
	/// A class used to listen to button events from a PLDebugMenu
	/// </summary>
	public class PLDebugMenuButtonEventListener : MonoBehaviour
	{
		[Header("Event")]
		/// the name of the event to listen to
		public string ButtonEventName = "Button";
		/// an event to fire when the event is heard
		public PLDButtonPressedEvent PLDEvent;

		[Header("Test")]
		public bool TestValue = true;
		[PLInspectorButton("TestSetValue")]
		public bool TestSetValueButton;

		/// <summary>
		/// This test methods will send a set event to all buttons bound to the ButtonEventName
		/// </summary>
		protected virtual void TestSetValue()
		{
			PLDebugMenuButtonEvent.Trigger(ButtonEventName, TestValue, PLDebugMenuButtonEvent.EventModes.SetButton);
		}

		/// <summary>
		/// When we get a menu button event, we invoke
		/// </summary>
		/// <param name="buttonEventName"></param>
		protected virtual void OnMMDebugMenuButtonEvent(string buttonEventName, bool value, PLDebugMenuButtonEvent.EventModes eventMode)
		{
			if ((eventMode == PLDebugMenuButtonEvent.EventModes.FromButton) && (buttonEventName == ButtonEventName))
			{
				if (PLDEvent != null)
				{
					PLDEvent.Invoke();
				}
			}
		}

		/// <summary>
		/// Starts listening for events
		/// </summary>
		public virtual void OnEnable()
		{
			PLDebugMenuButtonEvent.Register(OnMMDebugMenuButtonEvent);
		}

		/// <summary>
		/// Stops listening for events
		/// </summary>
		public virtual void OnDisable()
		{
			PLDebugMenuButtonEvent.Unregister(OnMMDebugMenuButtonEvent);
		}
	}
}