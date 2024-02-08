using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.Tools
{
	public enum PLSoundManagerAllSoundsControlEventTypes
	{
		Pause, Play, Stop, Free, FreeAllButPersistent, FreeAllLooping
	}
    
	/// <summary>
	/// This event will let you pause/play/stop/free all sounds playing through the PLSoundManager at once
	///
	/// Example : PLSoundManagerAllSoundsControlEvent.Trigger(PLSoundManagerAllSoundsControlEventTypes.Stop);
	/// will stop all sounds playing at once
	/// </summary>
	public struct PLSoundManagerAllSoundsControlEvent
	{
		public PLSoundManagerAllSoundsControlEventTypes EventType;
        
		public PLSoundManagerAllSoundsControlEvent(PLSoundManagerAllSoundsControlEventTypes eventType)
		{
			EventType = eventType;
		}

		static PLSoundManagerAllSoundsControlEvent e;
		public static void Trigger(PLSoundManagerAllSoundsControlEventTypes eventType)
		{
			e.EventType = eventType;
			PLEventManager.TriggerEvent(e);
		}
	}
}