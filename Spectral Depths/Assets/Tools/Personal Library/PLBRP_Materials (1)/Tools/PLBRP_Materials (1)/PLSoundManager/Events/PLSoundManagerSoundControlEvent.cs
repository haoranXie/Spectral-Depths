using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.Tools
{
	public enum PLSoundManagerSoundControlEventTypes
	{
		Pause,
		Resume,
		Stop,
		Free
	}
    
	/// <summary>
	/// An event used to control a specific sound on the PLSoundManager.
	/// You can either search for it by ID, or directly pass an audiosource if you have it.
	///
	/// Example : PLSoundManagerSoundControlEvent.Trigger(PLSoundManagerSoundControlEventTypes.Stop, 33);
	/// will cause the sound(s) with an ID of 33 to stop playing
	/// </summary>
	public struct PLSoundManagerSoundControlEvent
	{
		/// the ID of the sound to control (has to match the one used to play it)
		public int SoundID;
		/// the control mode
		public PLSoundManagerSoundControlEventTypes PLSoundManagerSoundControlEventType;
		/// the audiosource to control (if specified)
		public AudioSource TargetSource;
        
		public PLSoundManagerSoundControlEvent(PLSoundManagerSoundControlEventTypes eventType, int soundID, AudioSource source = null)
		{
			SoundID = soundID;
			TargetSource = source;
			PLSoundManagerSoundControlEventType = eventType;
		}

		static PLSoundManagerSoundControlEvent e;
		public static void Trigger(PLSoundManagerSoundControlEventTypes eventType, int soundID, AudioSource source = null)
		{
			e.SoundID = soundID;
			e.TargetSource = source;
			e.PLSoundManagerSoundControlEventType = eventType;
			PLEventManager.TriggerEvent(e);
		}
	}
}