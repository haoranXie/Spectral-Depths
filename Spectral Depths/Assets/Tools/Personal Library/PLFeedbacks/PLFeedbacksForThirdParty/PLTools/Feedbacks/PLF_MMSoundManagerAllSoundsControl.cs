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
	/// A feedback used to control all sounds playing on the PLSoundManager at once. It'll let you pause, play, stop and free (stop and returns the audiosource to the pool) sounds.  You will need a PLSoundManager in your scene for this to work.
	/// </summary>
	[AddComponentMenu("")]
	[FeedbackPath("Audio/PLSoundManager All Sounds Control")]
	[FeedbackHelp("A feedback used to control all sounds playing on the PLSoundManager at once. It'll let you pause, play, stop and free (stop and returns the audiosource to the pool) sounds. You will need a PLSoundManager in your scene for this to work.")]
	public class PLF_MMSoundManagerAllSoundsControl : PLF_Feedback
	{
		/// a static bool used to disable all feedbacks of this type at once
		public static bool FeedbackTypeAuthorized = true;
		/// sets the inspector color for this feedback
		#if UNITY_EDITOR
		public override Color FeedbackColor { get { return PLFeedbacksInspectorColors.SoundsColor; } }
		public override string RequiredTargetText { get { return ControlMode.ToString();  } }
		#endif
        
		[PLFInspectorGroup("PLSoundManager All Sounds Control", true, 30)]
		/// The selected control mode. 
		[Tooltip("The selected control mode")]
		public PLSoundManagerAllSoundsControlEventTypes ControlMode = PLSoundManagerAllSoundsControlEventTypes.Pause;

		/// <summary>
		/// On Play, we call the specified event, to be caught by the PLSoundManager
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
				case PLSoundManagerAllSoundsControlEventTypes.Pause:
					PLSoundManagerAllSoundsControlEvent.Trigger(PLSoundManagerAllSoundsControlEventTypes.Pause);
					break;
				case PLSoundManagerAllSoundsControlEventTypes.Play:
					PLSoundManagerAllSoundsControlEvent.Trigger(PLSoundManagerAllSoundsControlEventTypes.Play);
					break;
				case PLSoundManagerAllSoundsControlEventTypes.Stop:
					PLSoundManagerAllSoundsControlEvent.Trigger(PLSoundManagerAllSoundsControlEventTypes.Stop);
					break;
				case PLSoundManagerAllSoundsControlEventTypes.Free:
					PLSoundManagerAllSoundsControlEvent.Trigger(PLSoundManagerAllSoundsControlEventTypes.Free);
					break;
				case PLSoundManagerAllSoundsControlEventTypes.FreeAllButPersistent:
					PLSoundManagerAllSoundsControlEvent.Trigger(PLSoundManagerAllSoundsControlEventTypes.FreeAllButPersistent);
					break;
				case PLSoundManagerAllSoundsControlEventTypes.FreeAllLooping:
					PLSoundManagerAllSoundsControlEvent.Trigger(PLSoundManagerAllSoundsControlEventTypes.FreeAllLooping);
					break;
			}
		}
	}
}