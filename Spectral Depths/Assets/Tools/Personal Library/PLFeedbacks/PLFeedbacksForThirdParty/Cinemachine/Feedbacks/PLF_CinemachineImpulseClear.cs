using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Feedbacks;
#if PL_CINEMACHINE
using Cinemachine;
#endif

namespace SpectralDepths.FeedbacksForThirdParty
{
	[AddComponentMenu("")]
	#if PL_CINEMACHINE
	[FeedbackPath("Camera/Cinemachine Impulse Clear")]
	#endif
	[FeedbackHelp("This feedback lets you trigger a Cinemachine Impulse clear, stopping instantly any impulse that may be playing.")]
	public class PLF_CinemachineImpulseClear : PLF_Feedback
	{
		/// a static bool used to disable all feedbacks of this type at once
		public static bool FeedbackTypeAuthorized = true;
		/// sets the inspector color for this feedback
		#if UNITY_EDITOR
		public override Color FeedbackColor { get { return PLFeedbacksInspectorColors.CameraColor; } }
		#endif

		protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
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