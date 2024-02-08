using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Feedbacks;

//#if MOREMOUNTAINS_NICEVIBRATIONS
//using SpectralDepths.NiceVibrations;

namespace SpectralDepths.FeedbacksForThirdParty
{
	/// <summary>
	/// Add this feedback to be able to trigger haptic feedbacks via the NiceVibration library.
	/// It'll let you create transient or continuous vibrations, play presets or advanced patterns via AHAP files, and stop any vibration at any time
	/// This feedback has been deprecated, and is just here to avoid errors in case you were to update from an old version. Use the new haptic feedbacks instead.
	/// </summary>
	[AddComponentMenu("")]
	[FeedbackPath("Haptics/Haptics DEPRECATED!")]
	[FeedbackHelp("This feedback has been deprecated, and is just here to avoid errors in case you were to update from an old version. Use the new haptic feedbacks instead.")]
	public class PLFeedbackHaptics : PLFeedback
	{
		/// a static bool used to disable all feedbacks of this type at once
		public static bool FeedbackTypeAuthorized = true;
		#if UNITY_EDITOR
		public override Color FeedbackColor { get { return PLFeedbacksInspectorColors.HapticsColor; } }
		#endif
        
		public enum HapticTypes { Selection, Success, Warning, Failure, LightImpact, MediumImpact, HeavyImpact, RigidImpact, SoftImpact, None }
        
		/// the possible haptic methods for this feedback
		public enum HapticMethods { NativePreset, Transient, Continuous, AdvancedPattern, Stop, AdvancedTransient, AdvancedContinuous }
		/// the timescale to operate on
		public enum Timescales { ScaledTime, UnscaledTime }

		// NATIVE PRESET -----------------------------------------------------------------------------------------------------
		[Header("Haptics")]
		/// the method to use when triggering this haptic feedback
		[Tooltip("the method to use when triggering this haptic feedback")]
		public HapticMethods HapticMethod = HapticMethods.NativePreset;
		/// the type of native preset to use
		[Tooltip("the type of native preset to use")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.NativePreset)]
		public HapticTypes HapticType = HapticTypes.None;

		// TRANSIENT ---------------------------------------------------------------------------------------------------------
		/// the intensity of the transient haptic
		[Tooltip("the intensity of the transient haptic")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.Transient)]
		public float TransientIntensity = 1f;
		/// the sharpness of the transient haptic
		[Tooltip("the sharpness of the transient haptic")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.Transient)]
		public float TransientSharpness = 1f;

		// ADV TRANSIENT ---------------------------------------------------------------------------------------------------------
		/// whether or not to vibrate on iOS when in AdvancedTransient mode
		[Tooltip("whether or not to vibrate on iOS when in AdvancedTransient mode")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedTransient)]
		public bool ATVibrateIOS = true;
		/// the intensity on iOS when in AdvancedTransient mode
		[Tooltip("the intensity on iOS when in AdvancedTransient mode")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedTransient)]
		public float ATIOSIntensity = 1f;
		/// the sharpness on iOS when in AdvancedTransient mode
		[Tooltip("the sharpness on iOS when in AdvancedTransient mode")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedTransient)]
		public float ATIOSSharpness = 1f;
		/// whether or not to vibrate on android when in AdvancedTransient mode
		[Tooltip("whether or not to vibrate on android when in AdvancedTransient mode")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedTransient)]
		public bool ATVibrateAndroid = true;
		/// whether or not to vibrate on android if no support for advanced vibrations when in AdvancedTransient mode
		[Tooltip("whether or not to vibrate on android if no support for advanced vibrations when in AdvancedTransient mode")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedTransient)]
		public bool ATVibrateAndroidIfNoSupport = false;
		/// the intensity on android when in AdvancedTransient mode
		[Tooltip("the intensity on android when in AdvancedTransient mode")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedTransient)]
		public float ATAndroidIntensity = 1f;
		/// the sharpness on android when in AdvancedTransient mode
		[Tooltip("the sharpness on android when in AdvancedTransient mode")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedTransient)]
		public float ATAndroidSharpness = 1f;
		/// whether or not to rumble when in AdvancedTransient mode
		[Tooltip("whether or not to rumble when in AdvancedTransient mode")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedTransient)]
		public bool ATRumble = true;
		/// the rumble intensity when in AdvancedTransient mode
		[Tooltip("the rumble intensity when in AdvancedTransient mode")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedTransient)]
		public float ATRumbleIntensity = 1f;
		/// the rumble sharpness when in AdvancedTransient mode
		[Tooltip("the rumble sharpness when in AdvancedTransient mode")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedTransient)]
		public float ATRumbleSharpness = 1f;
		/// the controllerID when in AdvancedTransient mode
		[Tooltip("the controllerID when in AdvancedTransient mode")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedTransient)]
		public int ATRumbleControllerID = -1;

		// CONTINUOUS ---------------------------------------------------------------------------------------------------------
		/// the intensity that should be used to initialize the continuous haptic
		[Tooltip("the intensity that should be used to initialize the continuous haptic")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.Continuous)]
		public float InitialContinuousIntensity = 1f;
		/// the curve used to tween the continuous intensity
		[Tooltip("the curve used to tween the continuous intensity")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.Continuous)]
		public AnimationCurve ContinuousIntensityCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1f, 1f));
		/// the sharpness that should be used to initialize the continuous haptic
		[Tooltip("the sharpness that should be used to initialize the continuous haptic")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.Continuous)]
		public float InitialContinuousSharpness = 1f;
		/// the curve used to tween the continuous sharpness
		[Tooltip("the curve used to tween the continuous sharpness")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.Continuous)]
		public AnimationCurve ContinuousSharpnessCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1f, 1f));
		/// the duration of the continuous haptic
		[Tooltip("the duration of the continuous haptic")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.Continuous)]
		public float ContinuousDuration = 1f;

		// ADV PATTERN ---------------------------------------------------------------------------------------------------------

		/// whether or not to trigger advanced patterns on iOS
		[Tooltip("whether or not to trigger advanced patterns on iOS")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedPattern)]
		public bool APVibrateIOS = true;
		/// the AHAP file to use to trigger a pattern on iOS
		[Tooltip("the AHAP file to use to trigger a pattern on iOS")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedPattern)]
		public TextAsset AHAPFileForIOS;

		/// whether or not to trigger advanced patterns on Android
		[Tooltip("whether or not to trigger advanced patterns on Android")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedPattern)]
		public bool APVibrateAndroid = true;
		/// whether or not to vibrate if there's no haptics support
		[Tooltip("whether or not to vibrate if there's no haptics support")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedPattern)]
		public bool APVibrateAndroidIfNoSupport = false;
		/// the WaveFormFile to use to trigger a pattern on Android
		//[Tooltip("the WaveFormFile to use to trigger a pattern on Android")]
		//[PLFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedPattern)]
		//public PLNVAndroidWaveFormAsset AndroidWaveFormFile;

		/// whether or not to trigger advanced patterns on rumble
		[Tooltip("whether or not to trigger advanced patterns on rumble")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedPattern)]
		public bool APRumble = true;
		/// the file to use to trigger a rumble on gamepad
		//[Tooltip("the file to use to trigger a rumble on gamepad")]
		//[PLNVEnumCondition("HapticMethod", (int)HapticMethods.AdvancedPattern)]
		//public PLNVRumbleWaveFormAsset RumbleWaveFormFile;
		/// the amount of times this should repeat on Android (-1 : zero, 0 : infinite, 1 : one time, 2 : twice, etc)
		[Tooltip("the amount of times this should repeat on Android (-1 : zero, 0 : infinite, 1 : one time, 2 : twice, etc)")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedPattern)]
		public int AndroidRepeat = -1;
		/// the amount of times this should repeat on gamepad
		//[Tooltip("the amount of times this should repeat on gamepad")]
		//[PLNVEnumCondition("HapticMethod", (int)HapticMethods.AdvancedPattern)]
		public int RumbleRepeat = -1;
		/// a haptic type to play on older iOS APIs (prior to iOS 13)
		[Tooltip("a haptic type to play on older iOS APIs (prior to iOS 13)")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedPattern)]
		public HapticTypes OldIOSFallback;
		/// whether to run this on scaled or unscaled time
		[Tooltip("whether to run this on scaled or unscaled time")]
		[PLFEnumCondition("HapticMethod", (int)HapticMethods.AdvancedPattern)]
		public Timescales Timescale = Timescales.UnscaledTime;
        
		// RUMBLE -------------------------------------------------------------------------------------------------------------
		[Header("Rumble")]
		/// whether or not this feedback should trigger a rumble on gamepad
		[Tooltip("whether or not this feedback should trigger a rumble on gamepad")]
		public bool AllowRumble = true;
		/// the ID of the controller to rumble (-1 : auto/current, 0 : first controller, 1 : second controller, etc)
		[Tooltip("the ID of the controller to rumble (-1 : auto/current, 0 : first controller, 1 : second controller, etc)")]
		public int ControllerID = -1;

		[Header("Deprecated Feedback")] 
		/// if this is true, this feedback will output a warning when played
		public bool OutputDeprecationWarning = true;

		protected static bool _continuousPlaying = false;
		protected static float _continuousStartedAt = 0f;

		/// <summary>
		/// When this feedback gets played
		/// </summary>
		/// <param name="position"></param>
		/// <param name="feedbacksIntensity"></param>
		protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
		{
			if (!Active || !FeedbackTypeAuthorized)
			{
				return;
			}

			if (OutputDeprecationWarning)
			{
				Debug.LogWarning(this.name + " : the haptic feedback on this object is using the old version of Nice Vibrations, and won't work anymore. Replace it with any of the new haptic feedbacks.");
			}

			/*switch (HapticMethod)
			{
			    case HapticMethods.AdvancedPattern:

			        string iOSString = (AHAPFileForIOS == null) ? "" : AHAPFileForIOS.text;

			        long[] androidPattern = (AndroidWaveFormFile == null) ? null : AndroidWaveFormFile.WaveForm.Pattern;
			        int[] androidAmplitude = (AndroidWaveFormFile == null) ? null : AndroidWaveFormFile.WaveForm.Amplitudes;

			        long[] rumblePattern = (RumbleWaveFormFile == null) ? null : RumbleWaveFormFile.WaveForm.Pattern;
			        int[] lowFreqAmplitude = (RumbleWaveFormFile == null) ? null : RumbleWaveFormFile.WaveForm.LowFrequencyAmplitudes;
			        int[] highFreqAmplitude = (RumbleWaveFormFile == null) ? null : RumbleWaveFormFile.WaveForm.HighFrequencyAmplitudes;

			        PLVibrationManager.AdvancedHapticPattern(APVibrateIOS, iOSString, APVibrateAndroid, androidPattern, androidAmplitude, 
			            AndroidRepeat, APVibrateAndroidIfNoSupport, APRumble, 
			                                                            rumblePattern, lowFreqAmplitude, highFreqAmplitude, RumbleRepeat,
			                                                    OldIOSFallback, this, ControllerID);
			        break;

			    case HapticMethods.Continuous:
			        StartCoroutine(ContinuousHapticsCoroutine());
			        break;

			    case HapticMethods.NativePreset:
			        PLVibrationManager.Haptic(HapticType, false, AllowRumble, this, ControllerID);
			        break;

			    case HapticMethods.Transient:
			        PLVibrationManager.TransientHaptic(TransientIntensity, TransientSharpness, AllowRumble, this, ControllerID);
			        break;

			    case HapticMethods.AdvancedTransient:
			        PLVibrationManager.TransientHaptic(ATVibrateIOS, ATIOSIntensity, ATIOSSharpness, ATVibrateAndroid,
			            ATAndroidIntensity, ATAndroidSharpness, ATVibrateAndroidIfNoSupport, ATRumble,
			            ATRumbleIntensity, ATRumbleSharpness, ATRumbleControllerID, this);
			        break;

			    case HapticMethods.AdvancedContinuous:

			        break;

			    case HapticMethods.Stop:
			        if (_continuousPlaying)
			        {
			            PLVibrationManager.StopContinuousHaptic(AllowRumble);
			            _continuousPlaying = false;
			        }                    
			        break;
			}*/
		}

		/// <summary>
		/// A coroutine used to update continuous haptics as they're playing
		/// </summary>
		/// <returns></returns>
		/*protected virtual IEnumerator ContinuousHapticsCoroutine()
		{
		    _continuousStartedAt = (Timescale == Timescales.ScaledTime) ? Time.time : Time.unscaledTime;
		    _continuousPlaying = true;
		    float elapsedTime = ComputeElapsedTime();

		    PLVibrationManager.ContinuousHaptic(InitialContinuousIntensity, InitialContinuousSharpness, ContinuousDuration, HapticTypes.Success, this);

		    while (_continuousPlaying && (elapsedTime < ContinuousDuration))
		    {
		        elapsedTime = ComputeElapsedTime();
		        float remappedTime = Remap(elapsedTime, 0f, ContinuousDuration, 0f, 1f);
		        float intensity = ContinuousIntensityCurve.Evaluate(remappedTime);
		        float sharpness = ContinuousSharpnessCurve.Evaluate(remappedTime);
		        PLVibrationManager.UpdateContinuousHaptic(intensity, sharpness, true);
		        if (AllowRumble)
		        {
		            #if MOREMOUNTAINS_NICEVIBRATIONS_RUMBLE
		                PLNVRumble.RumbleContinuous(intensity, sharpness, ControllerID);
		            #endif
		        }
		        yield return null;
		    }
		    if (_continuousPlaying)
		    {
		        _continuousPlaying = false;
		        PLVibrationManager.StopContinuousHaptic(AllowRumble);
		    }
		}*/

		/// <summary>
		/// This methods computes and returns the elapsed time since the start of the last played continuous haptic
		/// </summary>
		/// <returns></returns>
		/*protected virtual float ComputeElapsedTime()
		{
		    float elapsedTime = (Timescale == Timescales.ScaledTime) ? Time.time - _continuousStartedAt : Time.unscaledTime - _continuousStartedAt;
		    return elapsedTime;
		}*/

		/// <summary>
		/// Remaps value x from AB to CD
		/// </summary>
		/// <param name="x"></param>
		/// <param name="A"></param>
		/// <param name="B"></param>
		/// <param name="C"></param>
		/// <param name="D"></param>
		/// <returns></returns>
		/*public static float Remap(float x, float A, float B, float C, float D)
		{
		    float remappedValue = C + (x - A) / (B - A) * (D - C);
		    return remappedValue;
		}*/
	}
}
//#endif