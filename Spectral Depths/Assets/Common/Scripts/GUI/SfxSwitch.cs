using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using SpectralDepths.Feedbacks;

namespace SpectralDepths.TopDown
{
	[AddComponentMenu("Spectral Depths/GUI/SfxSwitch")]
	public class SfxSwitch : TopDownMonoBehaviour
	{
		public virtual void On()
		{
			PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.UnmuteTrack, PLSoundManager.PLSoundManagerTracks.Sfx);
		}

		public virtual void Off()
		{
			PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.MuteTrack, PLSoundManager.PLSoundManagerTracks.Sfx);
		}
	}
}