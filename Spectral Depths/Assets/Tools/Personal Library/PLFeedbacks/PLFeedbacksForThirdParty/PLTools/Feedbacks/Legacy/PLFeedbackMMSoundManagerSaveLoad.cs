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
	/// This feedback will let you trigger save, load, and reset on PLSoundManager settings. You will need a PLSoundManager in your scene for this to work.
	/// </summary>
	[AddComponentMenu("")]
	[FeedbackPath("Audio/PLSoundManager Save and Load")]
	[FeedbackHelp("This feedback will let you trigger save, load, and reset on PLSoundManager settings. You will need a PLSoundManager in your scene for this to work.")]
	public class PLFeedbackMMSoundManagerSaveLoad : PLFeedback
	{
		/// a static bool used to disable all feedbacks of this type at once
		public static bool FeedbackTypeAuthorized = true;
		/// sets the inspector color for this feedback
		#if UNITY_EDITOR
		public override Color FeedbackColor { get { return PLFeedbacksInspectorColors.SoundsColor; } }
		#endif

		/// the possible modes you can use to interact with save settings
		public enum Modes { Save, Load, Reset }

		[Header("PLSoundManager Save and Load")] 
		/// the selected mode to interact with save settings on the PLSoundManager
		[Tooltip("the selected mode to interact with save settings on the PLSoundManager")]
		public Modes Mode = Modes.Save;
        
		/// <summary>
		/// On Play, saves, loads or resets settings
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
				case Modes.Save:
					PLSoundManagerEvent.Trigger(PLSoundManagerEventTypes.SaveSettings);
					break;
				case Modes.Load:
					PLSoundManagerEvent.Trigger(PLSoundManagerEventTypes.LoadSettings);
					break;
				case Modes.Reset:
					PLSoundManagerEvent.Trigger(PLSoundManagerEventTypes.ResetSettings);
					break;
			}
		}
	}
}