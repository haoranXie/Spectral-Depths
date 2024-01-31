using SpectralDepths.Tools;
using UnityEngine;

namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// This feedback lets you control the position of a RectTransform's pivot over time
	/// </summary>
	[AddComponentMenu("")]
	[FeedbackHelp("This feedback lets you control the position of a RectTransform's pivot over time")]
	[FeedbackPath("UI/RectTransform Pivot")]
	public class PLFeedbackRectTransformPivot : PLFeedbackBase
	{
		/// sets the inspector color for this feedback
		#if UNITY_EDITOR
		public override Color FeedbackColor { get { return PLFeedbacksInspectorColors.UIColor; } }
		#endif

		[Header("Target")]
		/// the RectTransform whose position you want to control over time 
		[Tooltip("the RectTransform whose position you want to control over time")]
		public RectTransform TargetRectTransform;
        
		[Header("Pivot")]
		/// The curve along which to evaluate the position of the RectTransform's pivot
		[Tooltip("The curve along which to evaluate the position of the RectTransform's pivot")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.OverTime)]
		public PLTweenType SpeedCurve = new PLTweenType(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1)));
		/// the position to remap the curve's 0 to 
		[Tooltip("the position to remap the curve's 0 to")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.OverTime)]
		public Vector2 RemapZero = Vector2.zero;
		/// the position to remap the curve's 1 to
		[Tooltip("the position to remap the curve's 1 to")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.OverTime, (int)PLFeedbackBase.Modes.Instant)]
		public Vector2 RemapOne = Vector2.one;
        
		protected override void FillTargets()
		{
			if (TargetRectTransform == null)
			{
				return;
			}
            
			PLFeedbackBaseTarget target = new PLFeedbackBaseTarget();
			PLPropertyReceiver receiver = new PLPropertyReceiver();
			receiver.TargetObject = TargetRectTransform.gameObject;
			receiver.TargetComponent = TargetRectTransform;
			receiver.TargetPropertyName = "pivot";
			receiver.RelativeValue = RelativeValues;
			receiver.Vector2RemapZero = RemapZero;
			receiver.Vector2RemapOne = RemapOne;
			target.Target = receiver;
			target.LevelCurve = SpeedCurve;
			target.RemapLevelZero = 0f;
			target.RemapLevelOne = 1f;
			target.InstantLevel = 1f;

			_targets.Add(target);
		}
	}
}