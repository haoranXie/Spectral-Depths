using SpectralDepths.Tools;
using UnityEngine;
#if PL_TEXTMESHPRO
using TMPro;
#endif


namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// This feedback lets you control the outline width of a target TMP over time
	/// </summary>
	[AddComponentMenu("")]
	[FeedbackHelp("This feedback lets you control the outline width of a target TMP over time.")]
	[FeedbackPath("TextMesh Pro/TMP Outline Width")]
	public class PLFeedbackTMPOutlineWidth : PLFeedbackBase
	{
		/// sets the inspector color for this feedback
		#if UNITY_EDITOR
		public override Color FeedbackColor { get { return PLFeedbacksInspectorColors.TMPColor; } }
		#endif

		#if PL_TEXTMESHPRO
		[Header("Target")]
		/// the TMP_Text component to control
		[Tooltip("the TMP_Text component to control")]
		public TMP_Text TargetTMPText;
		#endif

		[Header("Outline Width")]
		/// the curve to tween on
		[Tooltip("the curve to tween on")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.OverTime)]
		public PLTweenType OutlineWidthCurve = new PLTweenType(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0)));
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
			#if PL_TEXTMESHPRO
			if (TargetTMPText == null)
			{
				return;
			}

			PLFeedbackBaseTarget target = new PLFeedbackBaseTarget();
			PLPropertyReceiver receiver = new PLPropertyReceiver();
			receiver.TargetObject = TargetTMPText.gameObject;
			receiver.TargetComponent = TargetTMPText;
			receiver.TargetPropertyName = "outlineWidth";
			receiver.RelativeValue = RelativeValues;
			target.Target = receiver;
			target.LevelCurve = OutlineWidthCurve;
			target.RemapLevelZero = RemapZero;
			target.RemapLevelOne = RemapOne;
			target.InstantLevel = InstantFontSize;

			_targets.Add(target);
			#endif
		}
	}
}