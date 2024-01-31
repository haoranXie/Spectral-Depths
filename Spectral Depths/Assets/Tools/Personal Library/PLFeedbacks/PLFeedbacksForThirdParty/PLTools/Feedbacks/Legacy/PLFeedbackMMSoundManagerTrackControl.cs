using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using SpectralDepths.Tools;
using UnityEngine.Audio;

namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// This feedback will let you control all sounds playing on a specific track (master, UI, music, sfx), and play, pause, mute, unmute, resume, stop, free them all at once. You will need a PLSoundManager in your scene for this to work.
	/// </summary>
	[AddComponentMenu("")]
	[FeedbackPath("Audio/PLSoundManager Track Control")]
	[FeedbackHelp("This feedback will let you control all sounds playing on a specific track (master, UI, music, sfx), and play, pause, mute, unmute, resume, stop, free them all at once. You will need a PLSoundManager in your scene for this to work.")]
	public class PLFeedbackMMSoundManagerTrackControl : PLFeedback
	{
		/// a static bool used to disable all feedbacks of this type at once
		public static bool FeedbackTypeAuthorized = true;
		/// sets the inspector color for this feedback
		#if UNITY_EDITOR
		public override Color FeedbackColor { get { return PLFeedbacksInspectorColors.SoundsColor; } }
		#endif
        
		/// the possible modes you can use to interact with the track. Free will stop all sounds and return them to the pool
		public enum ControlModes { Mute, UnMute, SetVolume, Pause, Play, Stop, Free }
        
		[Header("PLSoundManager Track Control")]
		/// the track to mute/unmute/pause/play/stop/free/etc
		[Tooltip("the track to mute/unmute/pause/play/stop/free/etc")]
		public PLSoundManager.PLSoundManagerTracks Track;
		/// the selected control mode to interact with the track. Free will stop all sounds and return them to the pool
		[Tooltip("the selected control mode to interact with the track. Free will stop all sounds and return them to the pool")]
		public ControlModes ControlMode = ControlModes.Pause;
		/// if setting the volume, the volume to assign to the track 
		[Tooltip("if setting the volume, the volume to assign to the track")]
		[PLEnumCondition("ControlMode", (int) ControlModes.SetVolume)]
		public float Volume = 0.5f;

		/// <summary>
		/// On play, orders the entire track to follow the specific command, via a PLSoundManager event
		/// </summary>
		/// <param name="position"></param>
		/// <param name="feedbacksIntensity"></param>
		protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
		{
			if (!Active || !FeedbackTypeAuthorized)
			{
				return;
			}
            
			switch (ControlMode)
			{
				case ControlModes.Mute:
					PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.MuteTrack, Track);
					break;
				case ControlModes.UnMute:
					PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.UnmuteTrack, Track);
					break;
				case ControlModes.SetVolume:
					PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.SetVolumeTrack, Track, Volume);
					break;
				case ControlModes.Pause:
					PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.PauseTrack, Track);
					break;
				case ControlModes.Play:
					PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.PlayTrack, Track);
					break;
				case ControlModes.Stop:
					PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.StopTrack, Track);
					break;
				case ControlModes.Free:
					PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.FreeTrack, Track);
					break;
			}
		}
	}
}