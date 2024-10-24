﻿using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using SpectralDepths.Tools;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// An event used to stop fades
	/// </summary>
	public struct PLFadeStopEvent
	{
		/// an ID that has to match the one on the fader
		public int ID;
		public bool Restore;
        
		public PLFadeStopEvent(int id = 0, bool restore = false)
		{
			Restore = restore;
			ID = id;
		}
		static PLFadeStopEvent e;
		public static void Trigger(int id = 0, bool restore = false)
		{
			e.ID = id;
			e.Restore = restore;
			PLEventManager.TriggerEvent(e);
		}
	}
    
	/// <summary>
	/// Events used to trigger faders on or off
	/// </summary>
	public struct PLFadeEvent
	{
		/// an ID that has to match the one on the fader
		public int ID;
		/// the duration of the fade, in seconds
		public float Duration;
		/// the alpha to aim for
		public float TargetAlpha;
		/// the curve to apply to the fade
		public PLTweenType Curve;
		/// whether or not this fade should ignore timescale
		public bool IgnoreTimeScale;
		/// a world position for a target object. Useless for regular fades, but can be useful for alt implementations (circle fade for example)
		public Vector3 WorldPosition;


		/// <summary>
		/// Initializes a new instance of the <see cref="SpectralDepths.PLInterface.PLFadeEvent"/> struct.
		/// </summary>
		/// <param name="duration">Duration, in seconds.</param>
		/// <param name="targetAlpha">Target alpha, from 0 to 1.</param>
		public PLFadeEvent(float duration, float targetAlpha, PLTweenType tween, int id=0, 
			bool ignoreTimeScale = true, Vector3 worldPosition = new Vector3())
		{
			ID = id;
			Duration = duration;
			TargetAlpha = targetAlpha;
			Curve = tween;
			IgnoreTimeScale = ignoreTimeScale;
			WorldPosition = worldPosition;
		}
		static PLFadeEvent e;
		public static void Trigger(float duration, float targetAlpha)
		{
			Trigger(duration, targetAlpha, new PLTweenType(PLTween.PLTweenCurve.EaseInCubic));
		}
		public static void Trigger(float duration, float targetAlpha, PLTweenType tween, int id = 0, 
			bool ignoreTimeScale = true, Vector3 worldPosition = new Vector3())
		{
			e.ID = id;
			e.Duration = duration;
			e.TargetAlpha = targetAlpha;
			e.Curve = tween;
			e.IgnoreTimeScale = ignoreTimeScale;
			e.WorldPosition = worldPosition;
			PLEventManager.TriggerEvent(e);
		}
	}
     
	public struct PLFadeInEvent
	{
		/// an ID that has to match the one on the fader
		public int ID;
		/// the duration of the fade, in seconds
		public float Duration;
		/// the curve to apply to the fade
		public PLTweenType Curve;
		/// whether or not this fade should ignore timescale
		public bool IgnoreTimeScale;
		/// a world position for a target object. Useless for regular fades, but can be useful for alt implementations (circle fade for example)
		public Vector3 WorldPosition;

		/// <summary>
		/// Initializes a new instance of the <see cref="SpectralDepths.PLInterface.PLFadeInEvent"/> struct.
		/// </summary>
		/// <param name="duration">Duration.</param>
		public PLFadeInEvent(float duration, PLTweenType tween, int id = 0, 
			bool ignoreTimeScale = true, Vector3 worldPosition = new Vector3())
		{
			ID = id;
			Duration = duration;
			Curve = tween;
			IgnoreTimeScale = ignoreTimeScale;
			WorldPosition = worldPosition;
		}
		static PLFadeInEvent e;
		public static void Trigger(float duration, PLTweenType tween, int id = 0, 
			bool ignoreTimeScale = true, Vector3 worldPosition = new Vector3())
		{
			e.ID = id;
			e.Duration = duration;
			e.Curve = tween;
			e.IgnoreTimeScale = ignoreTimeScale;
			e.WorldPosition = worldPosition;
			PLEventManager.TriggerEvent(e);
		}
	}

	public struct PLFadeOutEvent
	{
		/// an ID that has to match the one on the fader
		public int ID;
		/// the duration of the fade, in seconds
		public float Duration;
		/// the curve to apply to the fade
		public PLTweenType Curve;
		/// whether or not this fade should ignore timescale
		public bool IgnoreTimeScale;
		/// a world position for a target object. Useless for regular fades, but can be useful for alt implementations (circle fade for example)
		public Vector3 WorldPosition;

		/// <summary>
		/// Initializes a new instance of the <see cref="SpectralDepths.PLInterface.PLFadeOutEvent"/> struct.
		/// </summary>
		/// <param name="duration">Duration.</param>
		public PLFadeOutEvent(float duration, PLTweenType tween, int id = 0, 
			bool ignoreTimeScale = true, Vector3 worldPosition = new Vector3())
		{
			ID = id;
			Duration = duration;
			Curve = tween;
			IgnoreTimeScale = ignoreTimeScale;
			WorldPosition = worldPosition;
		}

		static PLFadeOutEvent e;
		public static void Trigger(float duration, PLTweenType tween, int id = 0, 
			bool ignoreTimeScale = true, Vector3 worldPosition = new Vector3())
		{
			e.ID = id;
			e.Duration = duration;
			e.Curve = tween;
			e.IgnoreTimeScale = ignoreTimeScale;
			e.WorldPosition = worldPosition;
			PLEventManager.TriggerEvent(e);
		}
	}

	/// <summary>
	/// The Fader class can be put on an Image, and it'll intercept PLFadeEvents and turn itself on or off accordingly.
	/// </summary>
	[RequireComponent(typeof(CanvasGroup))]
	[RequireComponent(typeof(Image))]
	[AddComponentMenu("Spectral Depths/Tools/GUI/PLFader")]
	public class PLFader : MonoBehaviour, PLEventListener<PLFadeEvent>, PLEventListener<PLFadeInEvent>, PLEventListener<PLFadeOutEvent>, PLEventListener<PLFadeStopEvent>
	{
		public enum ForcedInitStates { None, Active, Inactive }
        
		[Header("Identification")]
		/// the ID for this fader (0 is default), set more IDs if you need more than one fader
		[Tooltip("the ID for this fader (0 is default), set more IDs if you need more than one fader")]
		public int ID;
        
		[Header("Opacity")]
		/// the opacity the fader should be at when inactive
		[Tooltip("the opacity the fader should be at when inactive")]
		public float InactiveAlpha = 0f;
		/// the opacity the fader should be at when active
		[Tooltip("the opacity the fader should be at when active")]
		public float ActiveAlpha = 1f;
		/// determines whether a state should be forced on init
		[Tooltip("determines whether a state should be forced on init")]
		public ForcedInitStates ForcedInitState = ForcedInitStates.Inactive;
        
		[Header("Timing")]
		/// the default duration of the fade in/out
		[Tooltip("the default duration of the fade in/out")]
		public float DefaultDuration = 0.2f;
		/// the default curve to use for this fader
		[Tooltip("the default curve to use for this fader")]
		public PLTweenType DefaultTween = new PLTweenType(PLTween.PLTweenCurve.LinearTween);
		/// whether or not the fade should happen in unscaled time
		[Tooltip("whether or not the fade should happen in unscaled time")] 
		public bool IgnoreTimescale = true;
		/// whether or not this fader can cause a fade if the requested final alpha is the same as the current one
		[Tooltip("whether or not this fader can cause a fade if the requested final alpha is the same as the current one")] 
		public bool CanFadeToCurrentAlpha = true;

		[Header("Interaction")]
		/// whether or not the fader should block raycasts when visible
		[Tooltip("whether or not the fader should block raycasts when visible")]
		public bool ShouldBlockRaycasts = false;

		[Header("Debug")]
		[PLInspectorButton("FadeIn1Second")]
		public bool FadeIn1SecondButton;
		[PLInspectorButton("FadeOut1Second")]
		public bool FadeOut1SecondButton;
		[PLInspectorButton("DefaultFade")]
		public bool DefaultFadeButton;
		[PLInspectorButton("ResetFader")]
		public bool ResetFaderButton;

		protected CanvasGroup _canvasGroup;
		protected Image _image;

		protected float _initialAlpha;
		protected float _currentTargetAlpha;
		protected float _currentDuration;
		protected PLTweenType _currentCurve;

		protected bool _fading = false;
		protected float _fadeStartedAt;
		protected bool _frameCountOne;

		/// <summary>
		/// Test method triggered by an inspector button
		/// </summary>
		protected virtual void ResetFader()
		{
			_canvasGroup.alpha = InactiveAlpha;
		}

		/// <summary>
		/// Test method triggered by an inspector button
		/// </summary>
		protected virtual void DefaultFade()
		{
			PLFadeEvent.Trigger(DefaultDuration, ActiveAlpha, DefaultTween, ID);
		}

		/// <summary>
		/// Test method triggered by an inspector button
		/// </summary>
		protected virtual void FadeIn1Second()
		{
			PLFadeInEvent.Trigger(1f, new PLTweenType(PLTween.PLTweenCurve.LinearTween));
		}

		/// <summary>
		/// Test method triggered by an inspector button
		/// </summary>
		protected virtual void FadeOut1Second()
		{
			PLFadeOutEvent.Trigger(1f, new PLTweenType(PLTween.PLTweenCurve.LinearTween));
		}

		/// <summary>
		/// On Start, we initialize our fader
		/// </summary>
		protected virtual void Awake()
		{
			Initialization();
		}

		/// <summary>
		/// On init, we grab our components, and disable/hide everything
		/// </summary>
		protected virtual void Initialization()
		{
			_canvasGroup = GetComponent<CanvasGroup>();
			_image = GetComponent<Image>();

			if (ForcedInitState == ForcedInitStates.Inactive)
			{
				_canvasGroup.alpha = InactiveAlpha;    
				_image.enabled = false;
			}
			else if (ForcedInitState == ForcedInitStates.Active)
			{
				_canvasGroup.alpha = ActiveAlpha;    
				_image.enabled = true;
			}
		}

		/// <summary>
		/// On Update, we update our alpha 
		/// </summary>
		protected virtual void Update()
		{
			if (_canvasGroup == null) { return; }

			if (_fading)
			{
				Fade();
			}
		}

		/// <summary>
		/// Fades the canvasgroup towards its target alpha
		/// </summary>
		protected virtual void Fade()
		{
			float currentTime = IgnoreTimescale ? Time.unscaledTime : Time.time;

			if (_frameCountOne)
			{
				if (Time.frameCount <= 2)
				{
					_canvasGroup.alpha = _initialAlpha;
					return;
				}
				_fadeStartedAt = IgnoreTimescale ? Time.unscaledTime : Time.time;
				currentTime = _fadeStartedAt;
				_frameCountOne = false;
			}
                        
			float endTime = _fadeStartedAt + _currentDuration;
			if (currentTime - _fadeStartedAt < _currentDuration)
			{
				float result = PLTween.Tween(currentTime, _fadeStartedAt, endTime, _initialAlpha, _currentTargetAlpha, _currentCurve);
				_canvasGroup.alpha = result;
			}
			else
			{
				StopFading();
			}
		}

		/// <summary>
		/// Stops the fading.
		/// </summary>
		protected virtual void StopFading()
		{
			_canvasGroup.alpha = _currentTargetAlpha;
			_fading = false;
			if (_canvasGroup.alpha == InactiveAlpha)
			{
				DisableFader();
			}
		}

		/// <summary>
		/// Disables the fader.
		/// </summary>
		protected virtual void DisableFader()
		{
			_image.enabled = false;
			if (ShouldBlockRaycasts)
			{
				_canvasGroup.blocksRaycasts = false;
			}
		}

		/// <summary>
		/// Enables the fader.
		/// </summary>
		protected virtual void EnableFader()
		{
			_image.enabled = true;
			if (ShouldBlockRaycasts)
			{
				_canvasGroup.blocksRaycasts = true;
			}
		}

		/// <summary>
		/// Starts fading this fader from the specified initial alpha to the target
		/// </summary>
		/// <param name="initialAlpha"></param>
		/// <param name="endAlpha"></param>
		/// <param name="duration"></param>
		/// <param name="curve"></param>
		/// <param name="id"></param>
		/// <param name="ignoreTimeScale"></param>
		protected virtual void StartFading(float initialAlpha, float endAlpha, float duration, PLTweenType curve, int id, bool ignoreTimeScale)
		{
			if (id != ID)
			{
				return;
			}

			if ((!CanFadeToCurrentAlpha) && (_canvasGroup.alpha == endAlpha))
			{
				return;
			}
            
			IgnoreTimescale = ignoreTimeScale;
			EnableFader();
			_fading = true;
			_initialAlpha = initialAlpha;
			_currentTargetAlpha = endAlpha;
			_fadeStartedAt = IgnoreTimescale ? Time.unscaledTime : Time.time;
			_currentCurve = curve;
			_currentDuration = duration;
			if (Time.frameCount == 1)
			{
				_frameCountOne = true;
			}
		}

		/// <summary>
		/// When catching a fade event, we fade our image in or out
		/// </summary>
		/// <param name="fadeEvent">Fade event.</param>
		public virtual void OnMMEvent(PLFadeEvent fadeEvent)
		{
			_currentTargetAlpha = (fadeEvent.TargetAlpha == -1) ? ActiveAlpha : fadeEvent.TargetAlpha;
			StartFading(_canvasGroup.alpha, _currentTargetAlpha, fadeEvent.Duration, fadeEvent.Curve, fadeEvent.ID, fadeEvent.IgnoreTimeScale);
		}

		/// <summary>
		/// When catching an PLFadeInEvent, we fade our image in
		/// </summary>
		/// <param name="fadeEvent">Fade event.</param>
		public virtual void OnMMEvent(PLFadeInEvent fadeEvent)
		{
			StartFading(InactiveAlpha, ActiveAlpha, fadeEvent.Duration, fadeEvent.Curve, fadeEvent.ID, fadeEvent.IgnoreTimeScale);
		}

		/// <summary>
		/// When catching an PLFadeOutEvent, we fade our image out
		/// </summary>
		/// <param name="fadeEvent">Fade event.</param>
		public virtual void OnMMEvent(PLFadeOutEvent fadeEvent)
		{
			StartFading(ActiveAlpha, InactiveAlpha, fadeEvent.Duration, fadeEvent.Curve, fadeEvent.ID, fadeEvent.IgnoreTimeScale);
		}

		/// <summary>
		/// When catching an PLFadeStopEvent, we stop our fade
		/// </summary>
		/// <param name="fadeEvent">Fade event.</param>
		public virtual void OnMMEvent(PLFadeStopEvent fadeStopEvent)
		{
			if (fadeStopEvent.ID == ID)
			{
				_fading = false;
				if (fadeStopEvent.Restore)
				{
					_canvasGroup.alpha = _initialAlpha;
				}
			}
		}

		/// <summary>
		/// On enable, we start listening to events
		/// </summary>
		protected virtual void OnEnable()
		{
			this.PLEventStartListening<PLFadeEvent>();
			this.PLEventStartListening<PLFadeStopEvent>();
			this.PLEventStartListening<PLFadeInEvent>();
			this.PLEventStartListening<PLFadeOutEvent>();
		}

		/// <summary>
		/// On disable, we stop listening to events
		/// </summary>
		protected virtual void OnDisable()
		{
			this.PLEventStopListening<PLFadeEvent>();
			this.PLEventStopListening<PLFadeStopEvent>();
			this.PLEventStopListening<PLFadeInEvent>();
			this.PLEventStopListening<PLFadeOutEvent>();
		}
	}
}