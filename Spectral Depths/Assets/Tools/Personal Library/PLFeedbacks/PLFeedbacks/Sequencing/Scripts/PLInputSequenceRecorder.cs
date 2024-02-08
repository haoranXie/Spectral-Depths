using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// This class lets you record sequences via input presses
	/// </summary>
	[AddComponentMenu("Spectral Depths/Feedbacks/Sequencing/PLInputSequenceRecorder")]
	[ExecuteAlways]
	public class PLInputSequenceRecorder : MonoBehaviour
	{
		[Header("Target")]
		/// the target scriptable object to write to
		[Tooltip("the target scriptable object to write to")]
		public PLSequence SequenceScriptableObject;

		[Header("Recording")]
		/// whether this recorder is recording right now or not
		[PLFReadOnly]
		[Tooltip("whether this recorder is recording right now or not")]
		public bool Recording;
		/// whether any silence between the start of the recording and the first press should be removed or not
		[Tooltip("whether any silence between the start of the recording and the first press should be removed or not")]
		public bool RemoveInitialSilence = true;
		/// whether this recording should write on top of existing entries or not
		[Tooltip("whether this recording should write on top of existing entries or not")]
		public bool AdditiveRecording = false;
		/// whether this recorder should start recording when entering play mode
		[Tooltip("whether this recorder should start recording when entering play mode")]
		public bool StartRecordingOnGameStart = false;
		/// the offset to apply to entries
		[Tooltip("the offset to apply to entries")]
		public float RecordingStartOffset = 0f;

		[Header("Recorder Keys")]
		/// the key binding for recording start
		[Tooltip("the key binding for recording start")]
		public KeyCode StartRecordingHotkey = KeyCode.Home;
		/// the key binding for recording stop
		[Tooltip("the key binding for recording stop")]
		public KeyCode StopRecordingHotkey = KeyCode.End;

		protected PLSequenceNote _note;
		protected float _recordingStartedAt = 0f;

		/// <summary>
		/// On awake we initialize our recorder
		/// </summary>
		protected virtual void Awake()
		{
			Initialization();
		}

		/// <summary>
		/// Makes sure we have a scriptable object to record to
		/// </summary>
		public virtual void Initialization()
		{
			Recording = false;

			_note = new PLSequenceNote();

			if (SequenceScriptableObject == null)
			{
				Debug.LogError(this.name + " this input based sequencer needs a bound scriptable object to function, please create one and bind it in the inspector.");
			}
		}

		/// <summary>
		/// On Start, starts a recording if needed
		/// </summary>
		protected virtual void Start()
		{
			if (StartRecordingOnGameStart)
			{
				StartRecording();
			}
		}

		/// <summary>
		/// Clears the sequence if needed and starts recording
		/// </summary>
		public virtual void StartRecording()
		{
			Recording = true;
			if (!AdditiveRecording)
			{
				SequenceScriptableObject.OriginalSequence.Line.Clear();
			}            
			_recordingStartedAt = Time.realtimeSinceStartup;
		}

		/// <summary>
		/// Stops the recording
		/// </summary>
		public virtual void StopRecording()
		{
			Recording = false;
			SequenceScriptableObject.QuantizeOriginalSequence();
		}

		/// <summary>
		/// On update we look for key presses
		/// </summary>
		protected virtual void Update()
		{
			if (!Application.isPlaying)
			{
				return;
			}
			DetectStartAndEnd();
			DetectRecording();
		}

		/// <summary>
		/// Detects key presses for start and end recording actions
		/// </summary>
		protected virtual void DetectStartAndEnd()
		{
			#if !ENABLE_INPUT_SYSTEM || ENABLE_LEGACY_INPUT_MANAGER
			if (!Recording)
			{
				if (Input.GetKeyDown(StartRecordingHotkey))
				{
					StartRecording();
				}
			}
			else
			{
				if (Input.GetKeyDown(StartRecordingHotkey))
				{
					StopRecording();
				}
			}
			#endif
		}

		/// <summary>
		/// Look for key presses to write to the sequence
		/// </summary>
		protected virtual void DetectRecording()
		{
			if (Recording && (SequenceScriptableObject != null))
			{
				foreach (PLSequenceTrack track in SequenceScriptableObject.SequenceTracks)
				{                    
					if (Input.GetKeyDown(track.Key))
					{
						AddNoteToTrack(track);
					}                    
				}
			}
		}

		/// <summary>
		/// Adds a note to the specified track
		/// </summary>
		/// <param name="track"></param>
		public virtual void AddNoteToTrack(PLSequenceTrack track)
		{
			if ((SequenceScriptableObject.OriginalSequence.Line.Count == 0) && RemoveInitialSilence)
			{
				_recordingStartedAt = Time.realtimeSinceStartup;
			}

			_note = new PLSequenceNote();
			_note.ID = track.ID;
			_note.Timestamp = Time.realtimeSinceStartup + RecordingStartOffset - _recordingStartedAt;
			SequenceScriptableObject.OriginalSequence.Line.Add(_note);
		}
	}
}