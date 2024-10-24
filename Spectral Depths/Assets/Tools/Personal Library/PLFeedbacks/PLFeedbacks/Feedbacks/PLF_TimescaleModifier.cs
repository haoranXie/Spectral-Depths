﻿using System.Collections;
using System.Collections.Generic;
using SpectralDepths.Tools;
using UnityEngine;

namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// This feedback changes the timescale by sending a TimeScale event on play
	/// </summary>
	[AddComponentMenu("")]
	[FeedbackHelp("This feedback triggers a PLTimeScaleEvent, which, if you have a PLTimeManager object in your scene, will be caught and used to modify the timescale according to the specified settings. These settings are the new timescale (0.5 will be twice slower than normal, 2 twice faster, etc), the duration of the timescale modification, and the optional speed at which to transition between normal and altered time scale.")]
	[FeedbackPath("Time/Timescale Modifier")]
	public class PLF_TimescaleModifier : PLF_Feedback
	{
		/// a static bool used to disable all feedbacks of this type at once
		public static bool FeedbackTypeAuthorized = true;
		/// <summary>
		/// The possible modes for this feedback :
		/// - shake : changes the timescale for a certain duration
		/// - change : sets the timescale to a new value, forever (until you change it again)
		/// - reset : resets the timescale to its previous value
		/// </summary>
		public enum Modes { Shake, Change, Reset }

		/// sets the inspector color for this feedback
		#if UNITY_EDITOR
		public override Color FeedbackColor { get { return PLFeedbacksInspectorColors.TimeColor; } }
		public override string RequiredTargetText { get { return Mode.ToString() + " x" + TimeScale ;  } }
		public override bool HasCustomInspectors { get { return true; } }
		#endif

		[PLFInspectorGroup("Timescale Modifier", true, 63)]
		/// the selected mode
		[Tooltip("the selected mode : shake : changes the timescale for a certain duration" +
		         "- change : sets the timescale to a new value, forever (until you change it again)" +
		         "- reset : resets the timescale to its previous value")]
		public Modes Mode = Modes.Shake;

		/// the new timescale to apply
		[Tooltip("the new timescale to apply")]
		public float TimeScale = 0.5f;
		/// the duration of the timescale modification
		[Tooltip("the duration of the timescale modification")]
		[PLFEnumCondition("Mode", (int)Modes.Shake)]
		public float TimeScaleDuration = 1f;
		/// whether to reset the timescale on Stop or not
		[Tooltip("whether to reset the timescale on Stop or not")]
		public bool ResetTimescaleOnStop = false;
		
		[PLFInspectorGroup("Interpolation", true, 63)]
		/// whether or not we should lerp the timescale
		[Tooltip("whether or not we should lerp the timescale")]
		public bool TimeScaleLerp = false;
		/// whether to lerp over a set duration, or at a certain speed
		[Tooltip("whether to lerp over a set duration, or at a certain speed")]
		public PLTimeScaleLerpModes TimescaleLerpMode = PLTimeScaleLerpModes.Speed;
		/// in Speed mode, the speed at which to lerp the timescale
		[Tooltip("in Speed mode, the speed at which to lerp the timescale")]
		[PLFEnumCondition("TimescaleLerpMode", (int)PLTimeScaleLerpModes.Speed)]
		public float TimeScaleLerpSpeed = 1f;
		/// in Duration mode, the curve to use to lerp the timescale
		[Tooltip("in Duration mode, the curve to use to lerp the timescale")]
		[PLFEnumCondition("TimescaleLerpMode", (int)PLTimeScaleLerpModes.Duration)]
		public PLTweenType TimescaleLerpCurve = new PLTweenType( new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1))); 
		/// in Duration mode, the duration of the timescale interpolation, in unscaled time seconds
		[Tooltip("in Duration mode, the duration of the timescale interpolation, in unscaled time seconds")]
		[PLFEnumCondition("TimescaleLerpMode", (int)PLTimeScaleLerpModes.Duration)]
		public float TimescaleLerpDuration = 1f;
		/// whether or not we should lerp the timescale as it goes back to normal afterwards
		[Tooltip("whether or not we should lerp the timescale as it goes back to normal afterwards")]
		[PLFEnumCondition("TimescaleLerpMode", (int)PLTimeScaleLerpModes.Duration)]
		public bool TimeScaleLerpOnReset = false;
		/// in Duration mode, the curve to use to lerp the timescale
		[Tooltip("in Duration mode, the curve to use to lerp the timescale")]
		[PLFEnumCondition("TimescaleLerpMode", (int)PLTimeScaleLerpModes.Duration)]
		public PLTweenType TimescaleLerpCurveOnReset = new PLTweenType( new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1)));
		/// in Duration mode, the duration of the timescale interpolation, in unscaled time seconds
		[Tooltip("in Duration mode, the duration of the timescale interpolation, in unscaled time seconds")]
		[PLFEnumCondition("TimescaleLerpMode", (int)PLTimeScaleLerpModes.Duration)]
		public float TimescaleLerpDurationOnReset = 1f;

		/// the duration of this feedback is the duration of the time modification
		public override float FeedbackDuration { get { return ApplyTimeMultiplier(TimeScaleDuration); } set { TimeScaleDuration = value; } }

		/// <summary>
		/// On Play, triggers a time scale event
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
				case Modes.Shake:
					PLTimeScaleEvent.Trigger(PLTimeScaleMethods.For, TimeScale, FeedbackDuration, TimeScaleLerp, TimeScaleLerpSpeed, false, TimescaleLerpMode, TimescaleLerpCurve, TimescaleLerpDuration, TimeScaleLerpOnReset, TimescaleLerpCurveOnReset, TimescaleLerpDurationOnReset);
					break;
				case Modes.Change:
					PLTimeScaleEvent.Trigger(PLTimeScaleMethods.For, TimeScale, 0f, TimeScaleLerp, TimeScaleLerpSpeed, true, TimescaleLerpMode, TimescaleLerpCurve, TimescaleLerpDuration, TimeScaleLerpOnReset, TimescaleLerpCurveOnReset, TimescaleLerpDurationOnReset);
					break;
				case Modes.Reset:
					PLTimeScaleEvent.Trigger(PLTimeScaleMethods.Reset, TimeScale, 0f, false, 0f, true);
					break;
			}     
		}

		/// <summary>
		/// On stop, we reset timescale if needed
		/// </summary>
		/// <param name="position"></param>
		/// <param name="feedbacksIntensity"></param>
		protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
		{
			if (!Active || !FeedbackTypeAuthorized || !ResetTimescaleOnStop)
			{
				return;
			}
			PLTimeScaleEvent.Trigger(PLTimeScaleMethods.Reset, TimeScale, 0f, false, 0f, true);
		}
		
		/// <summary>
		/// On restore, we restore our initial state
		/// </summary>
		protected override void CustomRestoreInitialValues()
		{
			if (!Active || !FeedbackTypeAuthorized)
			{
				return;
			}
			PLTimeScaleEvent.Trigger(PLTimeScaleMethods.Reset, TimeScale, 0f, false, 0f, true);
		}
	}
}