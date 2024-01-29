using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Tools;

namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// This feedback will let you pilot a PLPlaylist
	/// </summary>
	[AddComponentMenu("")]
	[FeedbackHelp("This feedback will let you pilot a PLPlaylist")]
	[FeedbackPath("Audio/PLPlaylist")]
	public class PLFeedbackPlaylist : PLFeedback
	{
		/// a static bool used to disable all feedbacks of this type at once
		public static bool FeedbackTypeAuthorized = true;
		/// sets the inspector color for this feedback
		#if UNITY_EDITOR
		public override Color FeedbackColor { get { return PLFeedbacksInspectorColors.SoundsColor; } }
		#endif
		
		public enum Modes { Play, PlayNext, PlayPrevious, Stop, Pause, PlaySongAt }

		[Header("PLPlaylist")] 
		/// the channel of the target PLPlaylist
		[Tooltip("the channel of the target PLPlaylist")]
		public int Channel = 0;
		/// the action to call on the playlist
		[Tooltip("the action to call on the playlist")]
		public Modes Mode = Modes.PlayNext;
		/// the index of the song to play
		[Tooltip("the index of the song to play")]
		[PLEnumCondition("Mode", (int)Modes.PlaySongAt)]
		public int SongIndex = 0;
        
		protected Coroutine _coroutine;

		/// <summary>
		/// On Play we change the values of our fog
		/// </summary>
		/// <param name="position"></param>
		/// <param name="feedbacksIntensity"></param>
		protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
		{
			if (!Active || !FeedbackTypeAuthorized)
			{
				return;
			}

			switch (Mode)
			{
				case Modes.Play:
					PLPlaylistPlayEvent.Trigger(Channel);
					break;
				case Modes.PlayNext:
					PLPlaylistPlayNextEvent.Trigger(Channel);
					break;
				case Modes.PlayPrevious:
					PLPlaylistPlayPreviousEvent.Trigger(Channel);
					break;
				case Modes.Stop:
					PLPlaylistStopEvent.Trigger(Channel);
					break;
				case Modes.Pause:
					PLPlaylistPauseEvent.Trigger(Channel);
					break;
				case Modes.PlaySongAt:
					PLPlaylistPlayIndexEvent.Trigger(Channel, SongIndex);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
            
		}
	}
}