using UnityEngine;
using SpectralDepths.Feedbacks;
#if PL_CINEMACHINE
using Cinemachine;
#endif

namespace SpectralDepths.FeedbacksForThirdParty
{
	[AddComponentMenu("")]
	#if PL_CINEMACHINE
	[FeedbackPath("Camera/Cinemachine Impulse Source")]
	#endif
	[FeedbackHelp("This feedback lets you generate an impulse on a Cinemachine Impulse source. You'll need a Cinemachine Impulse Listener on your camera for this to work.")]
	public class PLF_CinemachineImpulseSource : PLF_Feedback
	{
		/// a static bool used to disable all feedbacks of this type at once
		public static bool FeedbackTypeAuthorized = true;
		/// sets the inspector color for this feedback
		#if UNITY_EDITOR
			public override Color FeedbackColor { get { return PLFeedbacksInspectorColors.CameraColor; } }
			#if PL_CINEMACHINE
				public override bool EvaluateRequiresSetup() { return (ImpulseSource == null); }
				public override string RequiredTargetText { get { return ImpulseSource != null ? ImpulseSource.name : "";  } }
			#endif
			public override string RequiresSetupText { get { return "This feedback requires that an ImpulseSource be set to be able to work properly. You can set one below."; } }
		#endif
		

		[PLFInspectorGroup("Cinemachine Impulse Source", true, 28)]

		/// the velocity to apply to the impulse shake
		[Tooltip("the velocity to apply to the impulse shake")]
		public Vector3 Velocity = new Vector3(1f,1f,1f);
		#if PL_CINEMACHINE
			/// the impulse definition to broadcast
			[Tooltip("the impulse definition to broadcast")]
			public CinemachineImpulseSource ImpulseSource;
			
			public override bool HasAutomatedTargetAcquisition => true;
			protected override void AutomateTargetAcquisition() => ImpulseSource = FindAutomatedTarget<CinemachineImpulseSource>();
		#endif
		/// whether or not to clear impulses (stopping camera shakes) when the Stop method is called on that feedback
		[Tooltip("whether or not to clear impulses (stopping camera shakes) when the Stop method is called on that feedback")]
		public bool ClearImpulseOnStop = false;
        
		protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
		{
			if (!Active || !FeedbackTypeAuthorized)
			{
				return;
			}

			#if PL_CINEMACHINE
			if (ImpulseSource != null)
			{
				ImpulseSource.GenerateImpulse(Velocity);
			}
			#endif
		}

		/// <summary>
		/// Stops the animation if needed
		/// </summary>
		/// <param name="position"></param>
		/// <param name="feedbacksIntensity"></param>
		protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
		{
			if (!Active || !FeedbackTypeAuthorized || !ClearImpulseOnStop)
			{
				return;
			}
			base.CustomStopFeedback(position, feedbacksIntensity);
            
			#if PL_CINEMACHINE
			CinemachineImpulseManager.Instance.Clear();
			#endif
		}
		
		/// <summary>
		/// On restore, we put our object back at its initial position
		/// </summary>
		protected override void CustomRestoreInitialValues()
		{
			if (!Active || !FeedbackTypeAuthorized)
			{
				return;
			}
            
			#if PL_CINEMACHINE
			CinemachineImpulseManager.Instance.Clear();
			#endif
		}
	}
}