using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SpectralDepths.Tools
{
	[Serializable]
	public class PLDCheckboxPressedEvent : UnityEvent<bool> { }
	[Serializable]
	public class PLDCheckboxTrueEvent : UnityEvent { }
	[Serializable]
	public class PLDCheckboxFalseEvent : UnityEvent { }

	/// <summary>
	/// A class used to listen to events from a PLDebugMenu's checkbox
	/// </summary>
	public class PLDebugMenuCheckboxEventListener : MonoBehaviour
	{
		[Header("Events")]
		/// the name of the event to listen to
		public string CheckboxEventName = "CheckboxEventName";
		/// an event fired when the checkbox gets pressed
		public PLDCheckboxPressedEvent PLDPressedEvent;
		/// an event fired when the checkbox is pressed and becomes true/checked
		public PLDCheckboxTrueEvent PLDTrueEvent;
		/// an event fired when the checkbox is pressed and becomes false/unchecked
		public PLDCheckboxFalseEvent PLDFalseEvent;

		[Header("Test")]
		public bool TestValue = true;
		[PLInspectorButton("TestSetValue")]
		public bool TestSetValueButton;

		/// <summary>
		/// This test methods will send a set event to all checkboxes bound to the CheckboxEventName
		/// </summary>
		protected virtual void TestSetValue()
		{
			PLDebugMenuCheckboxEvent.Trigger(CheckboxEventName, TestValue, PLDebugMenuCheckboxEvent.EventModes.SetCheckbox);
		}

		/// <summary>
		/// When get a checkbox event, we invoke our events if needed
		/// </summary>
		/// <param name="checkboxNameEvent"></param>
		/// <param name="value"></param>
		protected virtual void OnMMDebugMenuCheckboxEvent(string checkboxNameEvent, bool value, PLDebugMenuCheckboxEvent.EventModes eventMode)
		{
			if ((eventMode == PLDebugMenuCheckboxEvent.EventModes.FromCheckbox) && (checkboxNameEvent == CheckboxEventName))
			{
				if (PLDPressedEvent != null)
				{
					PLDPressedEvent.Invoke(value);
				}

				if (value)
				{
					if (PLDTrueEvent != null)
					{
						PLDTrueEvent.Invoke();
					}
				}
				else
				{
					if (PLDFalseEvent != null)
					{
						PLDFalseEvent.Invoke();
					}
				}
			}
		}

		/// <summary>
		/// Starts listening for events
		/// </summary>
		public virtual void OnEnable()
		{
			PLDebugMenuCheckboxEvent.Register(OnMMDebugMenuCheckboxEvent);
		}

		/// <summary>
		/// Stops listening for events
		/// </summary>
		public virtual void OnDisable()
		{
			PLDebugMenuCheckboxEvent.Unregister(OnMMDebugMenuCheckboxEvent);
		}
	}
}