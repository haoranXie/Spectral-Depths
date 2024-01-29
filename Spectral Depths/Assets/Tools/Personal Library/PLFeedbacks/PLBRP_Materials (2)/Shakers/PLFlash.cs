using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SpectralDepths.Feedbacks;
using System;
using SpectralDepths.Tools;

namespace SpectralDepths.Feedbacks
{
	public struct PLFlashEvent
	{
		static private event Delegate OnEvent;
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] private static void RuntimeInitialization() { OnEvent = null; }
		static public void Register(Delegate callback) { OnEvent += callback; }
		static public void Unregister(Delegate callback) { OnEvent -= callback; }

		public delegate void Delegate(Color flashColor, float duration, float alpha, int flashID, PLChannelData channelData, TimescaleModes timescaleMode, bool stop = false);

		static public void Trigger(Color flashColor, float duration, float alpha, int flashID, PLChannelData channelData, TimescaleModes timescaleMode, bool stop = false)
		{
			OnEvent?.Invoke(flashColor, duration, alpha, flashID, channelData, timescaleMode, stop);
		}
	}

	[Serializable]
	public class PLFlashDebugSettings
	{
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
		/// the color of the flash
		public Color FlashColor = Color.white;
		/// the flash duration (in seconds)
		public float FlashDuration = 0.2f;
		/// the alpha of the flash
		public float FlashAlpha = 1f;
		/// the ID of the flash (usually 0). You can specify on each PLFlash object an ID, allowing you to have different flash images in one scene and call them separately (one for damage, one for health pickups, etc)
		public int FlashID = 0;
	}
    
	[RequireComponent(typeof(Image))]
	[RequireComponent(typeof(CanvasGroup))]
	[AddComponentMenu("Spectral Depths/Feedbacks/Shakers/Various/PLFlash")]
	/// <summary>
	/// Add this class to an image and it'll flash when getting a PLFlashEvent
	/// </summary>
	public class PLFlash : MonoBehaviour
	{
		[Header("Flash")]
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
		/// the ID of this PLFlash object. When triggering a PLFlashEvent you can specify an ID, and only PLFlash objects with this ID will answer the call and flash, allowing you to have more than one flash object in a scene
		[Tooltip("the ID of this PLFlash object. When triggering a PLFlashEvent you can specify an ID, and only PLFlash objects with this ID will answer the call and flash, allowing you to have more than one flash object in a scene")]
		public int FlashID = 0;
		/// if this is true, the PLFlash will stop before playing on every new event received
		[Tooltip("if this is true, the PLFlash will stop before playing on every new event received")]
		public bool Interruptable = false;
		
		[Header("Interpolation")]
		/// the animation curve to use when flashing in
		[Tooltip("the animation curve to use when flashing in")]
		public PLTweenType FlashInTween = new PLTweenType(PLTween.PLTweenCurve.LinearTween);
		/// the animation curve to use when flashing out
		[Tooltip("the animation curve to use when flashing out")]
		public PLTweenType FlashOutTween = new PLTweenType(PLTween.PLTweenCurve.LinearTween);

		[Header("Debug")]
		/// the set of test settings to use when pressing the DebugTest button
		[Tooltip("the set of test settings to use when pressing the DebugTest button")]
		public PLFlashDebugSettings DebugSettings;
		/// a test button that calls the DebugTest method
		[Tooltip("a test button that calls the DebugTest method")]
		[PLFInspectorButton("DebugTest")]
		public bool DebugTestButton;

		public virtual float GetTime() { return (_timescaleMode == TimescaleModes.Scaled) ? Time.time : Time.unscaledTime; }
		public virtual float GetDeltaTime() { return (_timescaleMode == TimescaleModes.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime; }

		protected Image _image;
		protected CanvasGroup _canvasGroup;
		protected bool _flashing = false;
		protected float _targetAlpha;
		protected Color _initialColor;
		protected float _delta;
		protected float _flashStartedTimestamp;
		protected int _direction = 1;
		protected float _duration;
		protected TimescaleModes _timescaleMode;
		protected PLTweenType _currentTween;

		/// <summary>
		/// On start we grab our image component
		/// </summary>
		protected virtual void Start()
		{
			_image = GetComponent<Image>();
			_canvasGroup = GetComponent<CanvasGroup>();
			_initialColor = _image.color;
		}

		/// <summary>
		/// On update we flash our image if needed
		/// </summary>
		protected virtual void Update()
		{
			if (_flashing)
			{
				_image.enabled = true;

				_currentTween = FlashInTween;
				if (GetTime() - _flashStartedTimestamp > _duration / 2f)
				{
					_direction = -1;
					_currentTween = FlashOutTween;
				}
				
				if (_direction == 1)
				{
					_delta += GetDeltaTime() / (_duration / 2f);
				}
				else
				{
					_delta -= GetDeltaTime() / (_duration / 2f);
				}
                
				if (GetTime() - _flashStartedTimestamp > _duration)
				{
					_flashing = false;
				}
				
				float percent = PLMaths.Remap(_delta, 0f, _duration/2f, 0f, 1f);
				float tweenValue = _currentTween.Evaluate(percent);

				_canvasGroup.alpha = Mathf.Lerp(0f, _targetAlpha, tweenValue);
			}
			else
			{
				_image.enabled = false;
			}
		}

		public virtual void DebugTest()
		{
			PLFlashEvent.Trigger(DebugSettings.FlashColor, DebugSettings.FlashDuration, DebugSettings.FlashAlpha, DebugSettings.FlashID, new PLChannelData(DebugSettings.ChannelMode, DebugSettings.Channel, DebugSettings.PLChannelDefinition), TimescaleModes.Unscaled);
		}

		/// <summary>
		/// When getting a flash event, we turn our image on
		/// </summary>
		public virtual void OnMMFlashEvent(Color flashColor, float duration, float alpha, int flashID, PLChannelData channelData, TimescaleModes timescaleMode, bool stop = false)
		{
			if (flashID != FlashID) 
			{
				return;
			}
            
			if (stop)
			{
				_flashing = false;
				return;
			}

			if (!PLChannel.Match(channelData, ChannelMode, Channel, PLChannelDefinition))
			{
				return;
			}

			if (_flashing && Interruptable)
			{
				_flashing = false;
			}

			if (!_flashing)
			{
				_flashing = true;
				_direction = 1;
				_canvasGroup.alpha = 0;
				_targetAlpha = alpha;
				_delta = 0f;
				_image.color = flashColor;
				_duration = duration;
				_timescaleMode = timescaleMode;
				_flashStartedTimestamp = GetTime();
			}
		} 

		/// <summary>
		/// On enable we start listening for events
		/// </summary>
		protected virtual void OnEnable()
		{
			PLFlashEvent.Register(OnMMFlashEvent);
		}

		/// <summary>
		/// On disable we stop listening for events
		/// </summary>
		protected virtual void OnDisable()
		{
			PLFlashEvent.Unregister(OnMMFlashEvent);
		}		
	}
}