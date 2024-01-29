using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Tools;

namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// This feedback will trigger a PLGameEvent of the specified name when played
	/// </summary>
	[AddComponentMenu("")]
	[FeedbackHelp("This feedback will trigger a PLGameEvent of the specified name when played")]
	[FeedbackPath("Events/PLGameEvent")]
	public class PLF_MMGameEvent : PLF_Feedback
	{
		/// a static bool used to disable all feedbacks of this type at once
		public static bool FeedbackTypeAuthorized = true;
		/// sets the inspector color for this feedback
		#if UNITY_EDITOR
		public override Color FeedbackColor { get { return PLFeedbacksInspectorColors.EventsColor; } }
		public override bool EvaluateRequiresSetup() { return (PLGameEventName == ""); }
		public override string RequiredTargetText { get { return PLGameEventName;  } }
		public override string RequiresSetupText { get { return "This feedback requires that you specify a PLGameEventName below."; } }
		#endif

		[PLFInspectorGroup("PLGameEvent", true, 57, true)]
		public string PLGameEventName;

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
			PLGameEvent.Trigger(PLGameEventName);
		}
	}
}