using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A class used to pilot a PLPlaylist
	/// </summary>
	[AddComponentMenu("Spectral Depths/Tools/Audio/PLPlaylistRemote")]
	public class PLPlaylistRemote : MonoBehaviour
	{
		public int Channel = 0;
		/// The track to play when calling PlaySelectedTrack
		public int TrackNumber = 0;

		[Header("Triggers")]
		/// if this is true, the selected track will be played on trigger enter (if you have a trigger collider on this)
		public bool PlaySelectedTrackOnTriggerEnter = true;
		/// if this is true, the selected track will be played on trigger exit (if you have a trigger collider on this)
		public bool PlaySelectedTrackOnTriggerExit = false;
		/// the tag to check for on trigger stuff
		public string TriggerTag = "Player";

		[Header("Test")]
		/// a play test button
		[PLInspectorButton("Play")]
		public bool PlayButton;
		/// a pause test button
		[PLInspectorButton("Pause")]
		public bool PauseButton;
		/// a stop test button
		[PLInspectorButton("Stop")]
		public bool StopButton;
		/// a next track test button
		[PLInspectorButton("PlayNextTrack")]
		public bool NextButton;
		/// a selected track test button
		[PLInspectorButton("PlaySelectedTrack")]
		public bool SelectedTrackButton;

		/// <summary>
		/// Plays the playlist
		/// </summary>
		public virtual void Play()
		{
			PLPlaylistPlayEvent.Trigger(Channel);
		}

		/// <summary>
		/// Pauses the current track
		/// </summary>
		public virtual void Pause()
		{
			PLPlaylistPauseEvent.Trigger(Channel);
		}

		/// <summary>
		/// Stops the playlist
		/// </summary>
		public virtual void Stop()
		{
			PLPlaylistStopEvent.Trigger(Channel);
		}

		/// <summary>
		/// Plays the next track in the playlist
		/// </summary>
		public virtual void PlayNextTrack()
		{
			PLPlaylistPlayNextEvent.Trigger(Channel);
		}

		/// <summary>
		/// Plays the track selected in the inspector
		/// </summary>
		public virtual void PlaySelectedTrack()
		{
			PLPlaylistPlayIndexEvent.Trigger(Channel, TrackNumber);
		}

		/// <summary>
		/// Plays the track set in parameters
		/// </summary>
		public virtual void PlayTrack(int trackIndex)
		{
			PLPlaylistPlayIndexEvent.Trigger(Channel, trackIndex);
		}

		/// <summary>
		/// On trigger enter, we play the selected track if needed
		/// </summary>
		/// <param name="collider"></param>
		protected virtual void OnTriggerEnter(Collider collider)
		{
			if (PlaySelectedTrackOnTriggerEnter && (collider.CompareTag(TriggerTag)))
			{
				PlaySelectedTrack();
			}
		}

		/// <summary>
		/// On trigger exit, we play the selected track if needed
		/// </summary>
		protected virtual void OnTriggerExit(Collider collider)
		{
			if (PlaySelectedTrackOnTriggerExit && (collider.CompareTag(TriggerTag)))
			{
				PlaySelectedTrack();
			}
		}

		/// <summary>
		/// On trigger enter 2D, we play the selected track if needed
		/// </summary>
		/// <param name="collider"></param>
		protected virtual void OnTriggerEnter2D(Collider2D collider)
		{
			if (PlaySelectedTrackOnTriggerEnter && (collider.CompareTag(TriggerTag)))
			{
				PlaySelectedTrack();
			}
		}

		/// <summary>
		/// On trigger exit 2D, we play the selected track if needed
		/// </summary>
		protected virtual void OnTriggerExit2D(Collider2D collider)
		{
			if (PlaySelectedTrackOnTriggerExit && (collider.CompareTag(TriggerTag)))
			{
				PlaySelectedTrack();
			}
		}
	}
}