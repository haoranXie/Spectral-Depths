using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A class used to bind a checkbox to a PLDebugMenu
	/// </summary>
	public class PLDebugMenuItemCheckbox : MonoBehaviour
	{
		[Header("Bindings")]
		/// the switch used to display the checkbox
		public PLDebugMenuSwitch Switch;
		/// the text used to display the checkbox's text
		public Text SwitchText;
		/// the name of the checkbox event
		public string CheckboxEventName = "Checkbox";

		protected bool _valueSetThisFrame = false;
		protected bool _listening = false;

		/// <summary>
		/// Triggers an event when the checkbox gets pressed
		/// </summary>
		public virtual void TriggerCheckboxEvent()
		{
			if (_valueSetThisFrame)
			{
				_valueSetThisFrame = false;
				return;
			}
			PLDebugMenuCheckboxEvent.Trigger(CheckboxEventName, Switch.SwitchState, PLDebugMenuCheckboxEvent.EventModes.FromCheckbox);
		}

		/// <summary>
		/// Triggers an event when the checkbox gets checked and becomes true
		/// </summary>
		public virtual void TriggerCheckboxEventTrue()
		{
			if (_valueSetThisFrame)
			{
				_valueSetThisFrame = false;
				return;
			}
			PLDebugMenuCheckboxEvent.Trigger(CheckboxEventName, true, PLDebugMenuCheckboxEvent.EventModes.FromCheckbox);
		}

		/// <summary>
		/// Triggers an event when the checkbox gets unchecked and becomes false
		/// </summary>
		public virtual void TriggerCheckboxEventFalse()
		{
			if (_valueSetThisFrame)
			{
				_valueSetThisFrame = false;
				return;
			}
			PLDebugMenuCheckboxEvent.Trigger(CheckboxEventName, false, PLDebugMenuCheckboxEvent.EventModes.FromCheckbox);
		}

		protected virtual void OnMMDebugMenuCheckboxEvent(string checkboxEventName, bool value, PLDebugMenuCheckboxEvent.EventModes eventMode)
		{
			if ((eventMode == PLDebugMenuCheckboxEvent.EventModes.SetCheckbox)
			    && (checkboxEventName == CheckboxEventName))
			{
				_valueSetThisFrame = true;
				if (value)
				{
					Switch.SetTrue();
				}
				else
				{
					Switch.SetFalse();
				}
			}
		}

		/// <summary>
		/// Starts listening for events
		/// </summary>
		public virtual void OnEnable()
		{
			if (!_listening)
			{
				_listening = true;
				PLDebugMenuCheckboxEvent.Register(OnMMDebugMenuCheckboxEvent);
			}            
		}

		/// <summary>
		/// Stops listening for events
		/// </summary>
		public virtual void OnDestroy()
		{
			_listening = false;
			PLDebugMenuCheckboxEvent.Unregister(OnMMDebugMenuCheckboxEvent);
		}
	}
}