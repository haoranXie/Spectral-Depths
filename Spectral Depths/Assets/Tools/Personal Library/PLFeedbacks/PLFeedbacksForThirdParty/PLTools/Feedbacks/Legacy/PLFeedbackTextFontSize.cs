using SpectralDepths.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// This feedback lets you control the font size of a target Text over time
	/// </summary>
	[AddComponentMenu("")]
	[FeedbackHelp("This feedback lets you control the font size of a target Text over time.")]
	[FeedbackPath("UI/Text Font Size")]
	public class PLFeedbackTextFontSize : PLFeedbackBase
	{
		/// sets the inspector color for this feedback
		#if UNITY_EDITOR
		public override Color FeedbackColor { get { return PLFeedbacksInspectorColors.TMPColor; } }
		#endif

		[Header("Target")]
		/// the TMP_Text component to control
		[Tooltip("the TMP_Text component to control")]
		public Text TargetText;

		[Header("Font Size")]
		/// the curve to tween on
		[Tooltip("the curve to tween on")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.OverTime)]
		public PLTweenType FontSizeCurve = new PLTweenType(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0)));
		/// the value to remap the curve's 0 to
		[Tooltip("the value to remap the curve's 0 to")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.OverTime)]
		public float RemapZero = 0f;
		/// the value to remap the curve's 1 to
		[Tooltip("the value to remap the curve's 1 to")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.OverTime)]
		public float RemapOne = 1f;
		/// the value to move to in instant mode
		[Tooltip("the value to move to in instant mode")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.Instant)]
		public float InstantFontSize;
        
		protected override void FillTargets()
		{
			if (TargetText == null)
			{
				return;
			}

			PLFeedbackBaseTarget target = new PLFeedbackBaseTarget();
			PLPropertyReceiver receiver = new PLPropertyReceiver();
			receiver.TargetObject = TargetText.gameObject;
			receiver.TargetComponent = TargetText;
			receiver.TargetPropertyName = "fontSize";
			receiver.RelativeValue = RelativeValues;
			target.Target = receiver;
			target.LevelCurve = FontSizeCurve;
			target.RemapLevelZero = RemapZero;
			target.RemapLevelOne = RemapOne;
			target.InstantLevel = InstantFontSize;

			_targets.Add(target);
		}

	}
}