using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Feedbacks;
using SpectralDepths.Tools;

namespace SpectralDepths.FeedbacksForThirdParty
{
	/// <summary>
	/// This class will allow you to trigger zooms on your camera by sending PLCameraZoomEvents from any other class
	/// </summary>
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Spectral Depths/Feedbacks/Shakers/Camera/PLCameraZoom")]
	public class PLCameraZoom : MonoBehaviour
	{
		[Header("Channel")]
		[PLFInspectorGroup("Shaker Settings", true, 3)]
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
		
		[Header("Transition Speed")]
		/// the animation curve to apply to the zoom transition
		[Tooltip("the animation curve to apply to the zoom transition")]
		public PLTweenType ZoomTween = new PLTweenType( new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f)));

		[Header("Test Zoom")]
		/// the mode to apply the zoom in when using the test button in the inspector
		[Tooltip("the mode to apply the zoom in when using the test button in the inspector")]
		public PLCameraZoomModes TestMode;
		/// the target field of view to apply the zoom in when using the test button in the inspector
		[Tooltip("the target field of view to apply the zoom in when using the test button in the inspector")]
		public float TestFieldOfView = 30f;
		/// the transition duration to apply the zoom in when using the test button in the inspector
		[Tooltip("the transition duration to apply the zoom in when using the test button in the inspector")]
		public float TestTransitionDuration = 0.1f;
		/// the duration to apply the zoom in when using the test button in the inspector
		[Tooltip("the duration to apply the zoom in when using the test button in the inspector")]
		public float TestDuration = 0.05f;

		[PLFInspectorButton("TestZoom")]
		/// an inspector button to test the zoom in play mode
		public bool TestZoomButton;
        
		public virtual float GetTime() { return (TimescaleMode == TimescaleModes.Scaled) ? Time.time : Time.unscaledTime; }
		public virtual float GetDeltaTime() { return (TimescaleMode == TimescaleModes.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime; }

		public TimescaleModes TimescaleMode { get; set; }
        
		protected Camera _camera;
		protected float _initialFieldOfView;
		protected PLCameraZoomModes _mode;
		protected bool _zooming = false;
		protected float _startFieldOfView;
		protected float _transitionDuration;
		protected float _duration;
		protected float _targetFieldOfView;
		protected int _direction = 1;
		protected float _reachedDestinationTimestamp;
		protected bool _destinationReached = false;
		protected float _elapsedTime = 0f;
		protected float _zoomStartedAt = 0f;

		/// <summary>
		/// On Awake we grab our virtual camera
		/// </summary>
		protected virtual void Awake()
		{
			_camera = this.gameObject.GetComponent<Camera>();
			_initialFieldOfView = _camera.fieldOfView;
		}	
        
		/// <summary>
		/// On Update if we're zooming we modify our field of view accordingly
		/// </summary>
		protected virtual void Update()
		{
			if (!_zooming)
			{
				return;
			}
			
			_elapsedTime = GetTime() - _zoomStartedAt;
			if (_elapsedTime <= _transitionDuration)
			{
				float t = PLMaths.Remap(_elapsedTime, 0f, _transitionDuration, 0f, 1f);
				_camera.fieldOfView = Mathf.LerpUnclamped(_startFieldOfView, _targetFieldOfView, ZoomTween.Evaluate(t));
			}
			else
			{
				if (!_destinationReached)
				{
					_reachedDestinationTimestamp = GetTime();
					_destinationReached = true;
				}
				if ((_mode == PLCameraZoomModes.For) && (_direction == 1))
				{
					if (GetTime() - _reachedDestinationTimestamp > _duration)
					{
						_direction = -1;
						_zoomStartedAt = GetTime();
						_startFieldOfView = _targetFieldOfView;
						_targetFieldOfView = _initialFieldOfView;
					}                    
				}
				else
				{
					_zooming = false;
				}   
			}
		}

		/// <summary>
		/// A method that triggers the zoom, ideally only to be called via an event, but public for convenience
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="newFieldOfView"></param>
		/// <param name="transitionDuration"></param>
		/// <param name="duration"></param>
		public virtual void Zoom(PLCameraZoomModes mode, float newFieldOfView, float transitionDuration, float duration, 
			bool useUnscaledTime, bool relative = false, PLTweenType tweenType = null)
		{
			if (_zooming)
			{
				return;
			}

			_zooming = true;
			_elapsedTime = 0f;
			_mode = mode;

			TimescaleMode = useUnscaledTime ? TimescaleModes.Unscaled : TimescaleModes.Scaled;
			_startFieldOfView = _camera.fieldOfView;
			_transitionDuration = transitionDuration;
			_duration = duration;
			_transitionDuration = transitionDuration;
			_direction = 1;
			_destinationReached = false;
			_initialFieldOfView = _camera.fieldOfView;
			_zoomStartedAt = GetTime();

			if (tweenType != null)
			{
				ZoomTween = tweenType;
			}

			switch (mode)
			{
				case PLCameraZoomModes.For:
					_targetFieldOfView = newFieldOfView;
					break;

				case PLCameraZoomModes.Set:
					_targetFieldOfView = newFieldOfView;
					break;

				case PLCameraZoomModes.Reset:
					_targetFieldOfView = _initialFieldOfView;
					break;
			}

			if (relative)
			{
				_targetFieldOfView += _initialFieldOfView;
			}

		}

		/// <summary>
		/// The method used by the test button to trigger a test zoom
		/// </summary>
		protected virtual void TestZoom()
		{
			Zoom(TestMode, TestFieldOfView, TestTransitionDuration, TestDuration, false, tweenType: ZoomTween);
		}

		/// <summary>
		/// When we get an PLCameraZoomEvent we call our zoom method 
		/// </summary>
		/// <param name="zoomEvent"></param>
		public virtual void OnCameraZoomEvent(PLCameraZoomModes mode, float newFieldOfView, float transitionDuration, float duration, PLChannelData channelData, 
			bool useUnscaledTime, bool stop = false, bool relative = false, bool restore = false, PLTweenType tweenType = null)
		{
			if (!PLChannel.Match(channelData, ChannelMode, Channel, PLChannelDefinition))
			{
				return;
			}
			if (stop)
			{
				_zooming = false;
				return;
			}
			if (restore)
			{
				_camera.fieldOfView = _initialFieldOfView;
				return;
			}
			this.Zoom(mode, newFieldOfView, transitionDuration, duration, useUnscaledTime, relative, tweenType);
		}

		/// <summary>
		/// Starts listening for PLCameraZoomEvents
		/// </summary>
		protected virtual void OnEnable()
		{
			PLCameraZoomEvent.Register(OnCameraZoomEvent);
		}

		/// <summary>
		/// Stops listening for PLCameraZoomEvents
		/// </summary>
		protected virtual void OnDisable()
		{
			PLCameraZoomEvent.Unregister(OnCameraZoomEvent);
		}
	}
}