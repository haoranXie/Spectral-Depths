using SpectralDepths.Tools;
using UnityEngine;
using System.Collections;
#if PL_TEXTMESHPRO
using TMPro;
#endif

namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// This feedback lets you tweak the softness of a TMP text over time
	/// </summary>
	[AddComponentMenu("")]
	[FeedbackHelp("This feedback lets you tweak the softness of a TMP text over time.")]
	[FeedbackPath("TextMesh Pro/TMP Softness")]
	public class PLFeedbackTMPSoftness : PLFeedback
	{
		/// a static bool used to disable all feedbacks of this type at once
		public static bool FeedbackTypeAuthorized = true;
		
		/// the duration of this feedback is the duration of the transition, or 0 if instant
		public override float FeedbackDuration { get { return (Mode == PLFeedbackBase.Modes.Instant) ? 0f : ApplyTimeMultiplier(Duration); } set { Duration = value; } }

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

		[Header("Softness")]
		/// whether or not values should be relative
		[Tooltip("whether or not values should be relative")]
		public bool RelativeValues = true;
		/// the selected mode
		[Tooltip("the selected mode")]
		public PLFeedbackBase.Modes Mode = PLFeedbackBase.Modes.OverTime;
		/// the duration of the feedback, in seconds
		[Tooltip("the duration of the feedback, in seconds")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.OverTime)]
		public float Duration = 0.5f;
		/// the curve to tween on
		[Tooltip("the curve to tween on")]
		[PLFEnumCondition("Mode", (int)PLFeedbackBase.Modes.OverTime)]
		public PLTweenType SoftnessCurve = new PLTweenType(new AnimationCurve(new Keyframe(0, 0f), new Keyframe(0.3f, 1f), new Keyframe(1, 0f)));
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
		public float InstantSoftness;
		/// if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over
		[Tooltip("if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over")] 
		public bool AllowAdditivePlays = false;

		protected float _initialSoftness;
		protected Coroutine _coroutine;

		/// <summary>
		/// On init we grab our initial softness
		/// </summary>
		/// <param name="owner"></param>
		protected override void CustomInitialization(GameObject owner)
		{
			base.CustomInitialization(owner);

			if (!Active)
			{
				return;
			}

			#if PL_TEXTMESHPRO
			_initialSoftness = TargetTMPText.fontMaterial.GetFloat(ShaderUtilities.ID_FaceDilate);
			#endif
		}

		/// <summary>
		/// On Play we animate our softness
		/// </summary>
		/// <param name="position"></param>
		/// <param name="feedbacksIntensity"></param>
		protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
		{
			#if PL_TEXTMESHPRO
			if (TargetTMPText == null)
			{
				return;
			}

			if (Active && FeedbackTypeAuthorized)
			{
				switch (Mode)
				{
					case PLFeedbackBase.Modes.Instant:
						TargetTMPText.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineSoftness, InstantSoftness);
						TargetTMPText.UpdateMeshPadding();
						break;
					case PLFeedbackBase.Modes.OverTime:
						if (!AllowAdditivePlays && (_coroutine != null))
						{
							return;
						}
						_coroutine = StartCoroutine(ApplyValueOverTime());
						break;
				}
			}
			#endif
		}

		protected virtual IEnumerator ApplyValueOverTime()
		{
			float journey = NormalPlayDirection ? 0f : FeedbackDuration;
			IsPlaying = true;
			while ((journey >= 0) && (journey <= FeedbackDuration) && (FeedbackDuration > 0))
			{
				float remappedTime = PLFeedbacksHelpers.Remap(journey, 0f, FeedbackDuration, 0f, 1f);

				SetValue(remappedTime);

				journey += NormalPlayDirection ? FeedbackDeltaTime : -FeedbackDeltaTime;
				yield return null;
			}
			SetValue(FinalNormalizedTime);
			_coroutine = null;
			IsPlaying = false;
			yield return null;
		}

		protected virtual void SetValue(float time)
		{
			#if PL_TEXTMESHPRO
			float intensity = PLTween.Tween(time, 0f, 1f, RemapZero, RemapOne, SoftnessCurve);
			float newValue = intensity;
			if (RelativeValues)
			{
				newValue += _initialSoftness;
			}
			TargetTMPText.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineSoftness, newValue);
			TargetTMPText.UpdateMeshPadding();
			#endif
		}
	}
}