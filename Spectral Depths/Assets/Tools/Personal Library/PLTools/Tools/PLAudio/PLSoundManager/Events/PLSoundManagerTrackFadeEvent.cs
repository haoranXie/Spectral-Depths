using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// This event will let you order the PLSoundManager to fade an entire track's sounds' volume towards the specified FinalVolume
	///
	/// Example : PLSoundManagerTrackFadeEvent.Trigger(PLSoundManager.PLSoundManagerTracks.Music, 2f, 0.5f, new PLTweenType(PLTween.PLTweenCurve.EaseInCubic));
	/// will fade the volume of the music track towards 0.5, over 2 seconds, using an ease in cubic tween 
	/// </summary>
	public struct PLSoundManagerTrackFadeEvent
	{
		public enum Modes { PlayFade, StopFade }

		/// whether we are fading a sound, or stopping an existing fade
		public Modes Mode;
		/// the track to fade the volume of
		public PLSoundManager.PLSoundManagerTracks Track;
		/// the duration of the fade, in seconds
		public float FadeDuration;
		/// the final volume to fade towards
		public float FinalVolume;
		/// the tween to use when fading
		public PLTweenType FadeTween;
        
		public PLSoundManagerTrackFadeEvent(Modes mode, PLSoundManager.PLSoundManagerTracks track, float fadeDuration, float finalVolume, PLTweenType fadeTween)
		{
			Mode = mode;
			Track = track;
			FadeDuration = fadeDuration;
			FinalVolume = finalVolume;
			FadeTween = fadeTween;
		}

		static PLSoundManagerTrackFadeEvent e;
		public static void Trigger(Modes mode, PLSoundManager.PLSoundManagerTracks track, float fadeDuration, float finalVolume, PLTweenType fadeTween)
		{
			e.Mode = mode;
			e.Track = track;
			e.FadeDuration = fadeDuration;
			e.FinalVolume = finalVolume;
			e.FadeTween = fadeTween;
			PLEventManager.TriggerEvent(e);
		}
	}
}