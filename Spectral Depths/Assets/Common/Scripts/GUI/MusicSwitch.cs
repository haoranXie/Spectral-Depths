using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using SpectralDepths.Feedbacks;

namespace SpectralDepths.TopDown
{
	[AddComponentMenu("Spectral Depths/GUI/MusicSwitch")]
	public class MusicSwitch : TopDownMonoBehaviour
	{
		public virtual void On()
		{
			PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.UnmuteTrack, PLSoundManager.PLSoundManagerTracks.Music);
		}

		public virtual void Off()
		{
			PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.MuteTrack, PLSoundManager.PLSoundManagerTracks.Music);
		}        
	}
}