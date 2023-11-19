using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using MoreMountains.Feedbacks;

namespace SpectralDepths.TopDown
{
	[AddComponentMenu("Spectral Depths/GUI/MusicSwitch")]
	public class MusicSwitch : TopDownMonoBehaviour
	{
		public virtual void On()
		{
			MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.UnmuteTrack, MMSoundManager.MMSoundManagerTracks.Music);
		}

		public virtual void Off()
		{
			MMSoundManagerTrackEvent.Trigger(MMSoundManagerTrackEventTypes.MuteTrack, MMSoundManager.MMSoundManagerTracks.Music);
		}        
	}
}