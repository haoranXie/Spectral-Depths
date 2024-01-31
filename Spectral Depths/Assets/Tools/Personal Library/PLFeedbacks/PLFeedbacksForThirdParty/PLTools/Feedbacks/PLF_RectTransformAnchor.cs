using SpectralDepths.Tools;
using UnityEngine;

namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// This feedback lets you control the min and max anchors of a RectTransform over time. That's the normalized position in the parent RectTransform that the lower left and upper right corners are anchored to.
	/// </summary>
	[AddComponentMenu("")]
	[FeedbackHelp("This feedback lets you control the min and max anchors of a RectTransform over time. That's the normalized position in the parent RectTransform that the lower left and upper right corners are anchored to.")]
	[FeedbackPath("UI/RectTransform Anchor")]
	public class PLF_RectTransformAnchor : PLF_FeedbackBase
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
		/// the target RectTransform to control
		[Tooltip("the target RectTransform to control")]
		public RectTransform TargetRectTransform;

		[PLFInspectorGroup("Anchor Min", true, 43)] 
		/// whether or not to modify the min anchor
		[Tooltip("whether or not to modify the min anchor")]
		public bool ModifyAnchorMin = true;
		/// the curve to animate the min anchor on
		[Tooltip("the curve to animate the min anchor on")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.OverTime)]
		public PLTweenType AnchorMinCurve = new PLTweenType(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1)));
		/// the value to remap the min anchor curve's 0 on
		[Tooltip("the value to remap the min anchor curve's 0 on")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.OverTime)]
		public Vector2 AnchorMinRemapZero = Vector2.zero;
		/// the value to remap the min anchor curve's 1 on
		[Tooltip("the value to remap the min anchor curve's 1 on")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.OverTime, (int)PLFeedbackBase.Modes.Instant)]
		public Vector2 AnchorMinRemapOne = Vector2.one;
        
		[PLFInspectorGroup("Anchor Max", true, 44)]
		/// whether or not to modify the max anchor
		[Tooltip("whether or not to modify the max anchor")]
		public bool ModifyAnchorMax = true;
		/// the curve to animate the max anchor on
		[Tooltip("the curve to animate the max anchor on")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.OverTime)]
		public PLTweenType AnchorMaxCurve = new PLTweenType(new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1)));
		/// the value to remap the max anchor curve's 0 on
		[Tooltip("the value to remap the max anchor curve's 0 on")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.OverTime)]
		public Vector2 AnchorMaxRemapZero = Vector2.zero;
		/// the value to remap the max anchor curve's 1 on
		[Tooltip("the value to remap the max anchor curve's 1 on")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.OverTime, (int)PLFeedbackBase.Modes.Instant)]
		public Vector2 AnchorMaxRemapOne = Vector2.one;
        
		protected override void FillTargets()
		{
			if (TargetRectTransform == null)
			{
				return;
			}
            
			PLF_FeedbackBaseTarget targetMin = new PLF_FeedbackBaseTarget();
			PLPropertyReceiver receiverMin = new PLPropertyReceiver();
			receiverMin.TargetObject = TargetRectTransform.gameObject;
			receiverMin.TargetComponent = TargetRectTransform;
			receiverMin.TargetPropertyName = "anchorMin";
			receiverMin.RelativeValue = RelativeValues;
			receiverMin.Vector2RemapZero = AnchorMinRemapZero;
			receiverMin.Vector2RemapOne = AnchorMinRemapOne;
			receiverMin.ShouldModifyValue = ModifyAnchorMin;
			targetMin.Target = receiverMin;
			targetMin.LevelCurve = AnchorMinCurve;
			targetMin.RemapLevelZero = 0f;
			targetMin.RemapLevelOne = 1f;
			targetMin.InstantLevel = 1f;

			_targets.Add(targetMin);
            
			PLF_FeedbackBaseTarget targetMax = new PLF_FeedbackBaseTarget();
			PLPropertyReceiver receiverMax = new PLPropertyReceiver();
			receiverMax.TargetObject = TargetRectTransform.gameObject;
			receiverMax.TargetComponent = TargetRectTransform;
			receiverMax.TargetPropertyName = "anchorMax";
			receiverMax.RelativeValue = RelativeValues;
			receiverMax.Vector2RemapZero = AnchorMaxRemapZero;
			receiverMax.Vector2RemapOne = AnchorMaxRemapOne;
			receiverMax.ShouldModifyValue = ModifyAnchorMax;
			targetMax.Target = receiverMax;
			targetMax.LevelCurve = AnchorMaxCurve;
			targetMax.RemapLevelZero = 0f;
			targetMax.RemapLevelOne = 1f;
			targetMax.InstantLevel = 1f;

			_targets.Add(targetMax);
		}
	}
}