using UnityEngine;
using System;
using SpectralDepths.Tools;

namespace SpectralDepths.Feedbacks
{
	[Serializable]
	/// <summary>
	/// Camera shake properties
	/// </summary>
	public struct PLCameraShakeProperties
	{
		public float Duration;
		public float Amplitude;
		public float Frequency;
		public float AmplitudeX;
		public float AmplitudeY;
		public float AmplitudeZ;

		public PLCameraShakeProperties(float duration, float amplitude, float frequency, float amplitudeX = 0f, float amplitudeY = 0f, float amplitudeZ = 0f)
		{
			Duration = duration;
			Amplitude = amplitude;
			Frequency = frequency;
			AmplitudeX = amplitudeX;
			AmplitudeY = amplitudeY;
			AmplitudeZ = amplitudeZ;
		}
	}

	public enum PLCameraZoomModes { For, Set, Reset }

	public struct PLCameraZoomEvent
	{
		static private event Delegate OnEvent;
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] private static void RuntimeInitialization() { OnEvent = null; }
		static public void Register(Delegate callback) { OnEvent += callback; }
		static public void Unregister(Delegate callback) { OnEvent -= callback; }

		public delegate void Delegate(PLCameraZoomModes mode, float newFieldOfView, float transitionDuration, float duration, PLChannelData channelData, bool useUnscaledTime = false, bool stop = false, bool relative = false, bool restore = false, PLTweenType tweenType = null);

		static public void Trigger(PLCameraZoomModes mode, float newFieldOfView, float transitionDuration, float duration, PLChannelData channelData, bool useUnscaledTime = false, bool stop = false, bool relative = false, bool restore = false, PLTweenType tweenType = null)
		{
			OnEvent?.Invoke(mode, newFieldOfView, transitionDuration, duration, channelData, useUnscaledTime, stop, relative, restore, tweenType);
		}
	}

	public struct PLCameraShakeEvent
	{
		static private event Delegate OnEvent;
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] private static void RuntimeInitialization() { OnEvent = null; }
		static public void Register(Delegate callback) { OnEvent += callback; }
		static public void Unregister(Delegate callback) { OnEvent -= callback; }

		public delegate void Delegate(float duration, float amplitude, float frequency, float amplitudeX, float amplitudeY, float amplitudeZ, bool infinite = false, PLChannelData channelData = null, bool useUnscaledTime = false);

		static public void Trigger(float duration, float amplitude, float frequency, float amplitudeX, float amplitudeY, float amplitudeZ, bool infinite = false, PLChannelData channelData = null, bool useUnscaledTime = false)
		{
			OnEvent?.Invoke(duration, amplitude, frequency, amplitudeX, amplitudeY, amplitudeZ, infinite, channelData, useUnscaledTime);
		}
	}

	public struct PLCameraShakeStopEvent
	{
		static private event Delegate OnEvent;
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] private static void RuntimeInitialization() { OnEvent = null; }
		static public void Register(Delegate callback) { OnEvent += callback; }
		static public void Unregister(Delegate callback) { OnEvent -= callback; }

		public delegate void Delegate(PLChannelData channelData);

		static public void Trigger(PLChannelData channelData)
		{
			OnEvent?.Invoke(channelData);
		}
	}

	[RequireComponent(typeof(PLWiggle))]
	[AddComponentMenu("Spectral Depths/Feedbacks/Shakers/Camera/PLCameraShaker")]
	/// <summary>
	/// A class to add to your camera. It'll listen to PLCameraShakeEvents and will shake your camera accordingly
	/// </summary>
	public class PLCameraShaker : MonoBehaviour
	{
		/// whether to listen on a channel defined by an int or by a PLChannel scriptable object. Ints are simple to setup but can get messy and make it harder to remember what int corresponds to what.
		/// PLChannel scriptable objects require you to create them in advance, but come with a readable name and are more scalable
		[Tooltip("whether to listen on a channel defined by an int or by a PLChannel scriptable object. Ints are simple to setup but can get messy and make it harder to remember what int corresponds to what. " +
		         "PLChannel scriptable objects require you to create them in advance, but come with a readable name and are more scalable")]
		public PLChannelModes ChannelMode = PLChannelModes.Int;
		/// the channel to listen to - has to match the one on the feedback
		[Tooltip("the channel to listen to - has to match the one on the feedback")]
		[PLEnumCondition("ChannelMode", (int)PLChannelModes.Int)]
		public int Channel = 0;
		/// the PLChannel definition asset to use to listen for events. The feedbacks targeting this shaker will have to reference that same PLChannel definition to receive events - to create a PLChannel,
		/// right click anywhere in your project (usually in a Data folder) and go SpectralDepths > PLChannel, then name it with some unique name
		[Tooltip("the PLChannel definition asset to use to listen for events. The feedbacks targeting this shaker will have to reference that same PLChannel definition to receive events - to create a PLChannel, " +
		         "right click anywhere in your project (usually in a Data folder) and go SpectralDepths > PLChannel, then name it with some unique name")]
		[PLEnumCondition("ChannelMode", (int)PLChannelModes.PLChannel)]
		public PLChannel PLChannelDefinition = null;
		/// a cooldown, in seconds, after a shake, during which no other shake can start
		[Tooltip("a cooldown, in seconds, after a shake, during which no other shake can start")]
		public float CooldownBetweenShakes = 0f;
	    
		protected PLWiggle _wiggle;
		protected float _shakeStartedTimestamp = -Single.MaxValue;

		/// <summary>
		/// On Awake, grabs the PLShaker component
		/// </summary>
		protected virtual void Awake()
		{
			_wiggle = GetComponent<PLWiggle>();
		}

		/// <summary>
		/// Shakes the camera for Duration seconds, by the desired amplitude and frequency
		/// </summary>
		/// <param name="duration">Duration.</param>
		/// <param name="amplitude">Amplitude.</param>
		/// <param name="frequency">Frequency.</param>
		public virtual void ShakeCamera(float duration, float amplitude, float frequency, float amplitudeX, float amplitudeY, float amplitudeZ, bool useUnscaledTime)
		{
			if (Time.unscaledTime - _shakeStartedTimestamp < CooldownBetweenShakes)
			{
				return;
			}
			
			if ((amplitudeX != 0f) || (amplitudeY != 0f) || (amplitudeZ != 0f))
			{
				_wiggle.PositionWiggleProperties.AmplitudeMin.x = -amplitudeX;
				_wiggle.PositionWiggleProperties.AmplitudeMin.y = -amplitudeY;
				_wiggle.PositionWiggleProperties.AmplitudeMin.z = -amplitudeZ;
                
				_wiggle.PositionWiggleProperties.AmplitudeMax.x = amplitudeX;
				_wiggle.PositionWiggleProperties.AmplitudeMax.y = amplitudeY;
				_wiggle.PositionWiggleProperties.AmplitudeMax.z = amplitudeZ;
			}
			else
			{
				_wiggle.PositionWiggleProperties.AmplitudeMin = Vector3.one * -amplitude;
				_wiggle.PositionWiggleProperties.AmplitudeMax = Vector3.one * amplitude;
			}

			_shakeStartedTimestamp = Time.time;
			_wiggle.PositionWiggleProperties.UseUnscaledTime = useUnscaledTime;
			_wiggle.PositionWiggleProperties.FrequencyMin = frequency;
			_wiggle.PositionWiggleProperties.FrequencyMax = frequency;
			_wiggle.PositionWiggleProperties.NoiseFrequencyMin = frequency * Vector3.one;
			_wiggle.PositionWiggleProperties.NoiseFrequencyMax = frequency * Vector3.one;
			_wiggle.WigglePosition(duration);
		}

		/// <summary>
		/// When a PLCameraShakeEvent is caught, shakes the camera
		/// </summary>
		/// <param name="shakeEvent">Shake event.</param>
		public virtual void OnCameraShakeEvent(float duration, float amplitude, float frequency, float amplitudeX, float amplitudeY, float amplitudeZ, bool infinite, PLChannelData channelData, bool useUnscaledTime)
		{
			if (!PLChannel.Match(channelData, ChannelMode, Channel, PLChannelDefinition))
			{
				return;
			}
			this.ShakeCamera (duration, amplitude, frequency, amplitudeX, amplitudeY, amplitudeZ, useUnscaledTime);
		}

		/// <summary>
		/// On enable, starts listening for events
		/// </summary>
		protected virtual void OnEnable()
		{
			PLCameraShakeEvent.Register(OnCameraShakeEvent);
		}

		/// <summary>
		/// On disable, stops listening to events
		/// </summary>
		protected virtual void OnDisable()
		{
			PLCameraShakeEvent.Unregister(OnCameraShakeEvent);
		}

	}
}