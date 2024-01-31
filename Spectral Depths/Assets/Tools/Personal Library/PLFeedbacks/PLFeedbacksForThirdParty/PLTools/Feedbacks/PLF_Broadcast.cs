using SpectralDepths.Tools;
using UnityEngine;

namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// This feedback lets you broadcast a float value to the PLRadio system
	/// </summary>
	[AddComponentMenu("")]
	[FeedbackHelp("This feedback lets you broadcast a float value to the PLRadio system.")]
	[FeedbackPath("GameObject/Broadcast")]
	public class PLF_Broadcast : PLF_FeedbackBase
	{
		/// sets the inspector color for this feedback
		#if UNITY_EDITOR
		public override Color FeedbackColor { get { return PLFeedbacksInspectorColors.UIColor; } }
		#endif
		public override bool HasChannel => true;

		[Header("Level")]
		/// the curve to tween the intensity on
		[Tooltip("the curve to tween the intensity on")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.OverTime)]
		public PLTweenType Curve = new PLTweenType(new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.3f, 1f), new Keyframe(1, 0)));
		/// the value to remap the intensity curve's 0 to
		[Tooltip("the value to remap the intensity curve's 0 to")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.OverTime)]
		public float RemapZero = 0f;
		/// the value to remap the intensity curve's 1 to
		[Tooltip("the value to remap the intensity curve's 1 to")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.OverTime)]
		public float RemapOne = 1f;
		/// the value to move the intensity to in instant mode
		[Tooltip("the value to move the intensity to in instant mode")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.Instant)]
		public float InstantChange;

		protected PLF_BroadcastProxy _proxy;
        
		/// <summary>
		/// On init we store our initial alpha
		/// </summary>
		/// <param name="owner"></param>
		protected override void CustomInitialization(PLF_Player owner)
		{
			base.CustomInitialization(owner);

			_proxy = Owner.gameObject.AddComponent<PLF_BroadcastProxy>();
			_proxy.Channel = Channel;
			PrepareTargets();
		}

		/// <summary>
		/// We setup our target with this object
		/// </summary>
		protected override void FillTargets()
		{
			PLF_FeedbackBaseTarget target = new PLF_FeedbackBaseTarget();
			PLPropertyReceiver receiver = new PLPropertyReceiver();
			receiver.TargetObject = Owner.gameObject;
			receiver.TargetComponent = _proxy;
			receiver.TargetPropertyName = "ThisLevel";
			receiver.RelativeValue = RelativeValues;
			target.Target = receiver;
			target.LevelCurve = Curve;
			target.RemapLevelZero = RemapZero;
			target.RemapLevelOne = RemapOne;
			target.InstantLevel = InstantChange;

			_targets.Add(target);
		}
	}
}