using SpectralDepths.Tools;
using UnityEngine;

namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// This feedback lets you control the size delta property (the size of this RectTransform relative to the distances between the anchors) of a RectTransform, over time 
	/// </summary>
	[AddComponentMenu("")]
	[FeedbackHelp("This feedback lets you control the size delta property (the size of this RectTransform relative to the distances between the anchors) of a RectTransform, over time")]
	[FeedbackPath("UI/RectTransformSizeDelta")]
	public class PLF_RectTransformSizeDelta : PLF_FeedbackBase
	{
		/// sets the inspector color for this feedback
		#if UNITY_EDITOR
		public override Color FeedbackColor { get { return PLFeedbacksInspectorColors.UIColor; } }
		public override bool EvaluateRequiresSetup() { return (TargetRectTransform == null); }
		public override string RequiredTargetText { get { return TargetRectTransform != null ? TargetRectTransform.name : "";  } }
		public override string RequiresSetupText { get { return "This feedback requires that a TargetRectTransform be set to be able to work properly. You can set one below."; } }
		#endif
		public override bool HasAutomatedTargetAcquisition => true;
		protected override void AutomateTargetAcquisition() => TargetRectTransform = FindAutomatedTarget<RectTransform>();

		[PLFInspectorGroup("Target RectTransform", true, 37, true)]
		/// the rect transform we want to impact
		[Tooltip("the rect transform we want to impact")]
		public RectTransform TargetRectTransform;
        
		[PLFInspectorGroup("Size Delta", true, 38)] 
		/// the speed at which we should animate the size delta
		[Tooltip("the speed at which we should animate the size delta")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.OverTime)]
		public PLTweenType SpeedCurve = new PLTweenType(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1)));
		/// the value to remap the curve's 0 to
		[Tooltip("the value to remap the curve's 0 to")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.OverTime)]
		public Vector2 RemapZero = Vector2.zero;
		/// the value to remap the curve's 1 to
		[Tooltip("the value to remap the curve's 1 to")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.OverTime, (int)PLFeedbackBase.Modes.Instant)]
		public Vector2 RemapOne = Vector2.one;
        
		protected override void FillTargets()
		{
			if (TargetRectTransform == null)
			{
				return;
			}
            
			PLF_FeedbackBaseTarget target = new PLF_FeedbackBaseTarget();
			PLPropertyReceiver receiver = new PLPropertyReceiver();
			receiver.TargetObject = TargetRectTransform.gameObject;
			receiver.TargetComponent = TargetRectTransform;
			receiver.TargetPropertyName = "sizeDelta";
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