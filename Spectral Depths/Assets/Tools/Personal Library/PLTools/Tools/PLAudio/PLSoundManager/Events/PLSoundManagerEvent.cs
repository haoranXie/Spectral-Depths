using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.Tools
{
	public enum PLSoundManagerEventTypes
	{
		SaveSettings,
		LoadSettings,
		ResetSettings,
		SettingsLoaded
	}
    
	/// <summary>
	/// This event will let you trigger a save/load/reset on the PLSoundManager settings
	///
	/// Example : PLSoundManagerEvent.Trigger(PLSoundManagerEventTypes.SaveSettings);
	/// will save settings. 
	/// </summary>
	public struct PLSoundManagerEvent
	{
		public PLSoundManagerEventTypes EventType;
        
		public PLSoundManagerEvent(PLSoundManagerEventTypes eventType)
		{
			EventType = eventType;
		}

		static PLSoundManagerEvent e;
		public static void Trigger(PLSoundManagerEventTypes eventType)
		{
			e.EventType = eventType;
			PLEventManager.TriggerEvent(e);
		}
	}
}