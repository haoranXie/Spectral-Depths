using System.Collections;
using UnityEngine;
#if PL_CINEMACHINE
using Cinemachine;
#endif
using SpectralDepths.Feedbacks;

namespace SpectralDepths.FeedbacksForThirdParty
{
	/// <summary>
	/// Add this component to your Cinemachine Virtual Camera to have it shake when calling its ShakeCamera methods.
	/// </summary>
	[AddComponentMenu("Spectral Depths/Feedbacks/Shakers/Cinemachine/PLCinemachineCameraShaker")]
	#if PL_CINEMACHINE
	[RequireComponent(typeof(CinemachineVirtualCamera))]
	#endif
	public class PLCinemachineCameraShaker : MonoBehaviour
	{
		[Header("Settings")]
		/// whether to listen on a channel defined by an int or by a PLChannel scriptable object. Ints are simple to setup but can get messy and make it harder to remember what int corresponds to what.
		/// PLChannel scriptable objects require you to create them in advance, but come with a readable name and are more scalable
		[Tooltip("whether to listen on a channel defined by an int or by a PLChannel scriptable object. Ints are simple to setup but can get messy and make it harder to remember what int corresponds to what. " +
		         "PLChannel scriptable objects require you to create them in advance, but come with a readable name and are more scalable")]
		public PLChannelModes ChannelMode = PLChannelModes.Int;
		/// the channel to listen to - has to match the one on the feedback
		[Tooltip("the channel to listen to - has to match the one on the feedback")]
		[PLFEnumCondition("ChannelMode", (int)PLChannelModes.Int)]
		public int Channel = 0;
		/// the PLChannel definition asset to use to listen for events. The feedbacks targeting this shaker will have to reference that same PLChannel definition to receive events - to create a PLChannel,
		/// right click anywhere in your project (usually in a Data folder) and go SpectralDepths > PLChannel, then name it with some unique name
		[Tooltip("the PLChannel definition asset to use to listen for events. The feedbacks targeting this shaker will have to reference that same PLChannel definition to receive events - to create a PLChannel, " +
		         "right click anywhere in your project (usually in a Data folder) and go SpectralDepths > PLChannel, then name it with some unique name")]
		[PLFEnumCondition("ChannelMode", (int)PLChannelModes.PLChannel)]
		public PLChannel PLChannelDefinition = null;
		/// The default amplitude that will be applied to your shakes if you don't specify one
		[Tooltip("The default amplitude that will be applied to your shakes if you don't specify one")]
		public float DefaultShakeAmplitude = .5f;
		/// The default frequency that will be applied to your shakes if you don't specify one
		[Tooltip("The default frequency that will be applied to your shakes if you don't specify one")]
		public float DefaultShakeFrequency = 10f;
		/// the amplitude of the camera's noise when it's idle
		[Tooltip("the amplitude of the camera's noise when it's idle")]
		[PLFReadOnly]
		public float IdleAmplitude;
		/// the frequency of the camera's noise when it's idle
		[Tooltip("the frequency of the camera's noise when it's idle")]
		[PLFReadOnly]
		public float IdleFrequency = 1f;
		/// the speed at which to interpolate the shake
		[Tooltip("the speed at which to interpolate the shake")]
		public float LerpSpeed = 5f;

		[Header("Test")]
		/// a duration (in seconds) to apply when testing this shake via the TestShake button
		[Tooltip("a duration (in seconds) to apply when testing this shake via the TestShake button")]
		public float TestDuration = 0.3f;
		/// the amplitude to apply when testing this shake via the TestShake button
		[Tooltip("the amplitude to apply when testing this shake via the TestShake button")]
		public float TestAmplitude = 2f;
		/// the frequency to apply when testing this shake via the TestShake button
		[Tooltip("the frequency to apply when testing this shake via the TestShake button")]
		public float TestFrequency = 20f;

		[PLFInspectorButton("TestShake")]
		public bool TestShakeButton;

		#if PL_CINEMACHINE
		public virtual float GetTime() { return (_timescaleMode == TimescaleModes.Scaled) ? Time.time : Time.unscaledTime; }
		public virtual float GetDeltaTime() { return (_timescaleMode == TimescaleModes.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime; }

		protected TimescaleModes _timescaleMode;
		protected Vector3 _initialPosition;
		protected Quaternion _initialRotation;
		protected Cinemachine.CinemachineBasicMultiChannelPerlin _perlin;
		protected Cinemachine.CinemachineVirtualCamera _virtualCamera;
		protected float _targetAmplitude;
		protected float _targetFrequency;
		private Coroutine _shakeCoroutine;

		/// <summary>
		/// On awake we grab our components
		/// </summary>
		protected virtual void Awake()
		{
			_virtualCamera = this.gameObject.GetComponent<CinemachineVirtualCamera>();
			_perlin = _virtualCamera.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
		}

		/// <summary>
		/// On Start we reset our camera to apply our base amplitude and frequency
		/// </summary>
		protected virtual void Start()
		{
			if (_perlin != null)
			{
				IdleAmplitude = _perlin.m_AmplitudeGain;
				IdleFrequency = _perlin.m_FrequencyGain;
			}            

			_targetAmplitude = IdleAmplitude;
			_targetFrequency = IdleFrequency;
		}

		protected virtual void Update()
		{
			if (_perlin != null)
			{
				_perlin.m_AmplitudeGain = _targetAmplitude;
				_perlin.m_FrequencyGain = Mathf.Lerp(_perlin.m_FrequencyGain, _targetFrequency, GetDeltaTime() * LerpSpeed);
			}
		}

		/// <summary>
		/// Use this method to shake the camera for the specified duration (in seconds) with the default amplitude and frequency
		/// </summary>
		/// <param name="duration">Duration.</param>
		public virtual void ShakeCamera(float duration, bool infinite, bool useUnscaledTime = false)
		{
			StartCoroutine(ShakeCameraCo(duration, DefaultShakeAmplitude, DefaultShakeFrequency, infinite, useUnscaledTime));
		}

		/// <summary>
		/// Use this method to shake the camera for the specified duration (in seconds), amplitude and frequency
		/// </summary>
		/// <param name="duration">Duration.</param>
		/// <param name="amplitude">Amplitude.</param>
		/// <param name="frequency">Frequency.</param>
		public virtual void ShakeCamera(float duration, float amplitude, float frequency, bool infinite, bool useUnscaledTime = false)
		{
			if (_shakeCoroutine != null)
			{
				StopCoroutine(_shakeCoroutine);
			}
			_shakeCoroutine = StartCoroutine(ShakeCameraCo(duration, amplitude, frequency, infinite, useUnscaledTime));
		}

		/// <summary>
		/// This coroutine will shake the 
		/// </summary>
		/// <returns>The camera co.</returns>
		/// <param name="duration">Duration.</param>
		/// <param name="amplitude">Amplitude.</param>
		/// <param name="frequency">Frequency.</param>
		protected virtual IEnumerator ShakeCameraCo(float duration, float amplitude, float frequency, bool infinite, bool useUnscaledTime)
		{
			_targetAmplitude  = amplitude;
			_targetFrequency = frequency;
			_timescaleMode = useUnscaledTime ? TimescaleModes.Unscaled : TimescaleModes.Scaled;
			if (!infinite)
			{
				yield return new WaitForSeconds(duration);
				CameraReset();
			}                        
		}

		/// <summary>
		/// Resets the camera's noise values to their idle values
		/// </summary>
		public virtual void CameraReset()
		{
			_targetAmplitude = IdleAmplitude;
			_targetFrequency = IdleFrequency;
		}

		public virtual void OnCameraShakeEvent(float duration, float amplitude, float frequency, float amplitudeX, float amplitudeY, float amplitudeZ, bool infinite, PLChannelData channelData, bool useUnscaledTime)
		{
			if (!PLChannel.Match(channelData, ChannelMode, Channel, PLChannelDefinition))
			{
				return;
			}
			this.ShakeCamera(duration, amplitude, frequency, infinite, useUnscaledTime);
		}

		public virtual void OnCameraShakeStopEvent(PLChannelData channelData)
		{
			if (!PLChannel.Match(channelData, ChannelMode, Channel, PLChannelDefinition))
			{
				return;
			}
			if (_shakeCoroutine != null)
			{
				StopCoroutine(_shakeCoroutine);
			}            
			CameraReset();
		}

		protected virtual void OnEnable()
		{
			PLCameraShakeEvent.Register(OnCameraShakeEvent);
			PLCameraShakeStopEvent.Register(OnCameraShakeStopEvent);
		}

		protected virtual void OnDisable()
		{
			PLCameraShakeEvent.Unregister(OnCameraShakeEvent);
			PLCameraShakeStopEvent.Unregister(OnCameraShakeStopEvent);
		}

		protected virtual void TestShake()
		{
			PLCameraShakeEvent.Trigger(TestDuration, TestAmplitude, TestFrequency, 0f, 0f, 0f, false, new PLChannelData(ChannelMode, Channel, PLChannelDefinition));
		}
		#endif
	}
}