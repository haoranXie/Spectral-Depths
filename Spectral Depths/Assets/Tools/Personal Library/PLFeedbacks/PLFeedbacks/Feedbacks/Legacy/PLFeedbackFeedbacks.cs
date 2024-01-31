using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// Turns an object active or inactive at the various stages of the feedback
	/// </summary>
	[AddComponentMenu("")]
	[FeedbackHelp("This feedback allows you to trigger any PLFeedbacks on the specified Channel within a certain range. You'll need an PLFeedbacksShaker on them.")]
	[FeedbackPath("GameObject/PLFeedbacks")]
	public class PLFeedbackFeedbacks : PLFeedback
	{
		/// a static bool used to disable all feedbacks of this type at once
		public static bool FeedbackTypeAuthorized = true;
		/// the duration of this feedback is 0
		public override float FeedbackDuration { get { return 0f; } }
		/// sets the inspector color for this feedback
		#if UNITY_EDITOR
		public override Color FeedbackColor { get { return PLFeedbacksInspectorColors.GameObjectColor; } }
		#endif

		[Header("PLFeedbacks")]
		/// the channel to broadcast on
		[Tooltip("the channel to broadcast on")]
		public int Channel = 0;
		/// whether or not to use a range
		[Tooltip("whether or not to use a range")]
		public bool UseRange = false;
		/// the range of the event, in units
		[Tooltip("the range of the event, in units")]
		public float EventRange = 100f;
		/// the transform to use to broadcast the event as origin point
		[Tooltip("the transform to use to broadcast the event as origin point")]
		public Transform EventOriginTransform;

		/// <summary>
		/// On init we turn the light off if needed
		/// </summary>
		/// <param name="owner"></param>
		protected override void CustomInitialization(GameObject owner)
		{
			base.CustomInitialization(owner);
            
			if (EventOriginTransform == null)
			{
				EventOriginTransform = this.transform;
			}
		}

		/// <summary>
		/// On Play we trigger our feedback shake event
		/// </summary>
		/// <param name="position"></param>
		/// <param name="feedbacksIntensity"></param>
		protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
		{
			if (!Active || !FeedbackTypeAuthorized)
			{
				return;
			}
			PLFeedbacksShakeEvent.Trigger(ChannelData(Channel), UseRange, EventRange, EventOriginTransform.position);
		}
	}
}