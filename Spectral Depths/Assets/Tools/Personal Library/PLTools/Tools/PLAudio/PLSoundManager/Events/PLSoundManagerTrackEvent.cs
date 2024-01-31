using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.Tools
{
	public enum PLSoundManagerTrackEventTypes
	{
		MuteTrack,
		UnmuteTrack,
		SetVolumeTrack,
		PlayTrack,
		PauseTrack,
		StopTrack,
		FreeTrack
	}
    
	/// <summary>
	/// This feedback will let you mute, unmute, play, pause, stop, free or set the volume of a selected track
	///
	/// Example :  PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.PauseTrack,PLSoundManager.PLSoundManagerTracks.UI);
	/// will pause the entire UI track
	/// </summary>
	public struct PLSoundManagerTrackEvent
	{
		/// the order to pass to the track
		public PLSoundManagerTrackEventTypes TrackEventType;
		/// the track to pass the order to
		public PLSoundManager.PLSoundManagerTracks Track;
		/// if in SetVolume mode, the volume to which to set the track to
		public float Volume;
        
		public PLSoundManagerTrackEvent(PLSoundManagerTrackEventTypes trackEventType, PLSoundManager.PLSoundManagerTracks track = PLSoundManager.PLSoundManagerTracks.Master, float volume = 1f)
		{
			TrackEventType = trackEventType;
			Track = track;
			Volume = volume;
		}

		static PLSoundManagerTrackEvent e;
		public static void Trigger(PLSoundManagerTrackEventTypes trackEventType, PLSoundManager.PLSoundManagerTracks track = PLSoundManager.PLSoundManagerTracks.Master, float volume = 1f)
		{
			e.TrackEventType = trackEventType;
			e.Track = track;
			e.Volume = volume;
			PLEventManager.TriggerEvent(e);
		}
	}
}