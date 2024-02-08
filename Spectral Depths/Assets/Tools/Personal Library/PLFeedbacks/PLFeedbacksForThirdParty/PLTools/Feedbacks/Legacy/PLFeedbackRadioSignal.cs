using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Tools;

namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// This feedback will let you trigger a play on a target PLRadioSignal (usually used by a PLRadioBroadcaster to emit a value that can then be listened to by PLRadioReceivers. From this feedback you can also specify a duration, timescale and multiplier.
	/// </summary>
	[AddComponentMenu("")]
	[FeedbackHelp("This feedback will let you trigger a play on a target PLRadioSignal (usually used by a PLRadioBroadcaster to emit a value that can then be listened to by PLRadioReceivers. From this feedback you can also specify a duration, timescale and multiplier.")]
	[FeedbackPath("GameObject/PLRadioSignal")]
	public class PLFeedbackRadioSignal : PLFeedback
	{
		/// a static bool used to disable all feedbacks of this type at once
		public static bool FeedbackTypeAuthorized = true;
		
		/// the duration of this feedback is 0
		public override float FeedbackDuration { get { return 0f; } }

		/// The target PLRadioSignal to trigger
		[Tooltip("The target PLRadioSignal to trigger")]
		public PLRadioSignal TargetSignal;
		/// the timescale to operate on
		[Tooltip("the timescale to operate on")]
		public PLRadioSignal.TimeScales TimeScale = PLRadioSignal.TimeScales.Unscaled;
        
		/// the duration of the shake, in seconds
		[Tooltip("the duration of the shake, in seconds")]
		public float Duration = 1f;
		/// a global multiplier to apply to the end result of the combination
		[Tooltip("a global multiplier to apply to the end result of the combination")]
		public float GlobalMultiplier = 1f;

		/// sets the inspector color for this feedback
		#if UNITY_EDITOR
		public override Color FeedbackColor { get { return PLFeedbacksInspectorColors.GameObjectColor; } }
		#endif
        

		/// <summary>
		/// On Play we set the values on our target signal and make it start shaking its level
		/// </summary>
		/// <param name="position"></param>
		/// <param name="feedbacksIntensity"></param>
		protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
		{
			if (Active && FeedbackTypeAuthorized)
			{
				if (TargetSignal != null)
				{
					float intensityMultiplier = Timing.ConstantIntensity ? 1f : feedbacksIntensity;
                    
					TargetSignal.Duration = Duration;
					TargetSignal.GlobalMultiplier = GlobalMultiplier * intensityMultiplier;
					TargetSignal.TimeScale = TimeScale;
					TargetSignal.StartShaking();
				}
			}
		}

		protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			if (Active)
			{
				if (TargetSignal != null)
				{
					TargetSignal.Stop();
				}
			}
		}
	}
}