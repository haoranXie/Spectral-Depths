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
	/// This feedback lets you trigger fades on a specific sound via the PLSoundManager. You will need a PLSoundManager in your scene for this to work.
	/// </summary>
	[AddComponentMenu("")]
	[FeedbackPath("Audio/PLSoundManager Sound Fade")]
	[FeedbackHelp("This feedback lets you trigger fades on a specific sound via the PLSoundManager. You will need a PLSoundManager in your scene for this to work.")]
	public class PLF_MMSoundManagerSoundFade : PLF_Feedback
	{
		/// a static bool used to disable all feedbacks of this type at once
		public static bool FeedbackTypeAuthorized = true;
		/// sets the inspector color for this feedback
		#if UNITY_EDITOR
		public override Color FeedbackColor { get { return PLFeedbacksInspectorColors.SoundsColor; } }
		public override string RequiredTargetText { get { return "ID "+SoundID;  } }
		#endif

		[PLFInspectorGroup("PLSoundManager Sound Fade", true, 30)]
		/// the ID of the sound you want to fade. Has to match the ID you specified when playing the sound initially
		[Tooltip("the ID of the sound you want to fade. Has to match the ID you specified when playing the sound initially")]
		public int SoundID = 0;
		/// the duration of the fade, in seconds
		[Tooltip("the duration of the fade, in seconds")]
		public float FadeDuration = 1f;
		/// the volume towards which to fade
		[Tooltip("the volume towards which to fade")]
		[Range(PLSoundManagerSettings._minimalVolume,PLSoundManagerSettings._maxVolume)]
		public float FinalVolume = PLSoundManagerSettings._minimalVolume;
		/// the tween to apply over the fade
		[Tooltip("the tween to apply over the fade")]
		public PLTweenType FadeTween = new PLTweenType(PLTween.PLTweenCurve.EaseInOutQuartic);
        
		protected AudioSource _targetAudioSource;
        
		/// <summary>
		/// On play, we start our fade via a fade event
		/// </summary>
		/// <param name="position"></param>
		/// <param name="feedbacksIntensity"></param>
		protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
		{
			if (!Active || !FeedbackTypeAuthorized)
			{
				return;
			}
            
			PLSoundManagerSoundFadeEvent.Trigger(PLSoundManagerSoundFadeEvent.Modes.PlayFade, SoundID, FadeDuration, FinalVolume, FadeTween);
		}
        
		/// <summary>
		/// On stop, we stop our fade via a fade event
		/// </summary>
		/// <param name="position"></param>
		/// <param name="feedbacksIntensity"></param>
		protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
		{
			if (!Active || !FeedbackTypeAuthorized)
			{
				return;
			}
            
			PLSoundManagerSoundFadeEvent.Trigger(PLSoundManagerSoundFadeEvent.Modes.StopFade, SoundID, FadeDuration, FinalVolume, FadeTween);
		}
	}
}