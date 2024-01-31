using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace  SpectralDepths.Feedbacks
{
	/// <summary>
	/// Events triggered by a PLFeedbacks when playing a series of feedbacks
	/// - play : when a PLFeedbacks starts playing
	/// - pause : when a holding pause is met
	/// - resume : after a holding pause resumes
	/// - revert : when a PLFeedbacks reverts its play direction
	/// - complete : when a PLFeedbacks has played its last feedback
	///
	/// to listen to these events :
	///
	/// public virtual void OnMMFeedbacksEvent(PLFeedbacks source, EventTypes type)
	/// {
	///     // do something
	/// }
	/// 
	/// protected virtual void OnEnable()
	/// {
	///     PLFeedbacksEvent.Register(OnMMFeedbacksEvent);
	/// }
	/// 
	/// protected virtual void OnDisable()
	/// {
	///     PLFeedbacksEvent.Unregister(OnMMFeedbacksEvent);
	/// }
	/// 
	/// </summary>
	public struct PLF_PlayerEvent
	{
		static private event Delegate OnEvent;
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] private static void RuntimeInitialization() { OnEvent = null; }
		static public void Register(Delegate callback) { OnEvent += callback; }
		static public void Unregister(Delegate callback) { OnEvent -= callback; }

		public enum EventTypes { Play, Pause, Resume, Revert, Complete }
		public delegate void Delegate(PLF_Player source, EventTypes type);
		static public void Trigger(PLF_Player source, EventTypes type)
		{
			OnEvent?.Invoke(source, type);
		}
	}

	/// <summary>
	/// A subclass of PLFeedbacks, contains UnityEvents that can be played, 
	/// </summary>
	[Serializable]
	public class PLF_PlayerEvents
	{
		/// whether or not this PLFeedbacks should fire PLFeedbacksEvents
		[Tooltip("whether or not this PLFeedbacks should fire PLFeedbacksEvents")] 
		public bool TriggerMMFeedbacksEvents = false; 
		/// whether or not this PLFeedbacks should fire Unity Events
		[Tooltip("whether or not this PLFeedbacks should fire Unity Events")] 
		public bool TriggerUnityEvents = true;
		/// This event will fire every time this PLFeedbacks gets played
		[Tooltip("This event will fire every time this PLFeedbacks gets played")]
		public UnityEvent OnPlay;
		/// This event will fire every time this PLFeedbacks starts a holding pause
		[Tooltip("This event will fire every time this PLFeedbacks starts a holding pause")]
		public UnityEvent OnPause;
		/// This event will fire every time this PLFeedbacks resumes after a holding pause
		[Tooltip("This event will fire every time this PLFeedbacks resumes after a holding pause")]
		public UnityEvent OnResume;
		/// This event will fire every time this PLFeedbacks reverts its play direction
		[Tooltip("This event will fire every time this PLFeedbacks reverts its play direction")]
		public UnityEvent OnRevert;
		/// This event will fire every time this PLFeedbacks plays its last PLFeedback
		[Tooltip("This event will fire every time this PLFeedbacks plays its last PLFeedback")]
		public UnityEvent OnComplete;

		public bool OnPlayIsNull { get; protected set; }
		public bool OnPauseIsNull { get; protected set; }
		public bool OnResumeIsNull { get; protected set; }
		public bool OnRevertIsNull { get; protected set; }
		public bool OnCompleteIsNull { get; protected set; }

		/// <summary>
		/// On init we store for each event whether or not we have one to invoke
		/// </summary>
		public virtual void Initialization()
		{
			OnPlayIsNull = OnPlay == null;
			OnPauseIsNull = OnPause == null;
			OnResumeIsNull = OnResume == null;
			OnRevertIsNull = OnRevert == null;
			OnCompleteIsNull = OnComplete == null;
		}

		/// <summary>
		/// Fires Play events if needed
		/// </summary>
		/// <param name="source"></param>
		public virtual void TriggerOnPlay(PLF_Player source)
		{
			if (!OnPlayIsNull && TriggerUnityEvents)
			{
				OnPlay.Invoke();
			}

			if (TriggerMMFeedbacksEvents)
			{
				PLF_PlayerEvent.Trigger(source, PLF_PlayerEvent.EventTypes.Play);
			}
		}

		/// <summary>
		/// Fires pause events if needed
		/// </summary>
		/// <param name="source"></param>
		public virtual void TriggerOnPause(PLF_Player source)
		{
			if (!OnPauseIsNull && TriggerUnityEvents)
			{
				OnPause.Invoke();
			}

			if (TriggerMMFeedbacksEvents)
			{
				PLF_PlayerEvent.Trigger(source, PLF_PlayerEvent.EventTypes.Pause);
			}
		}

		/// <summary>
		/// Fires resume events if needed
		/// </summary>
		/// <param name="source"></param>
		public virtual void TriggerOnResume(PLF_Player source)
		{
			if (!OnResumeIsNull && TriggerUnityEvents)
			{
				OnResume.Invoke();
			}

			if (TriggerMMFeedbacksEvents)
			{
				PLF_PlayerEvent.Trigger(source, PLF_PlayerEvent.EventTypes.Resume);
			}
		}

		/// <summary>
		/// Fires revert events if needed
		/// </summary>
		/// <param name="source"></param>
		public virtual void TriggerOnRevert(PLF_Player source)
		{
			if (!OnRevertIsNull && TriggerUnityEvents)
			{
				OnRevert.Invoke();
			}

			if (TriggerMMFeedbacksEvents)
			{
				PLF_PlayerEvent.Trigger(source, PLF_PlayerEvent.EventTypes.Revert);
			}
		}

		/// <summary>
		/// Fires complete events if needed
		/// </summary>
		/// <param name="source"></param>
		public virtual void TriggerOnComplete(PLF_Player source)
		{
			if (!OnCompleteIsNull && TriggerUnityEvents)
			{
				OnComplete.Invoke();
			}

			if (TriggerMMFeedbacksEvents)
			{
				PLF_PlayerEvent.Trigger(source, PLF_PlayerEvent.EventTypes.Complete);
			}
		}
	}
   
}