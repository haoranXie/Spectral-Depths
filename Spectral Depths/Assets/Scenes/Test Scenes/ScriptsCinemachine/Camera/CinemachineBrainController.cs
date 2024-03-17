using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
#if PL_CINEMACHINE
using Cinemachine;
#endif

namespace SpectralDepths.TopDown
{
	public enum PLCinemachineBrainEventTypes { ChangeBlendDuration }

	/// <summary>
	/// An event used to interact with camera brains
	/// </summary>
	public struct PLCinemachineBrainEvent
	{
		public PLCinemachineBrainEventTypes EventType;
		public float Duration;

		public PLCinemachineBrainEvent(PLCinemachineBrainEventTypes eventType, float duration)
		{
			EventType = eventType;
			Duration = duration;
		}

		static PLCinemachineBrainEvent e;
		public static void Trigger(PLCinemachineBrainEventTypes eventType, float duration)
		{
			e.EventType = eventType;
			e.Duration = duration;
			PLEventManager.TriggerEvent(e);
		}
	}

	/// <summary>
	/// This class is designed to control CinemachineBrains, letting you control their default blend values via events from any class
	/// </summary>
	#if PL_CINEMACHINE
	[RequireComponent(typeof(CinemachineBrain))]
	#endif
	public class CinemachineBrainController : TopDownMonoBehaviour, PLEventListener<PLCinemachineBrainEvent>
	{
		#if PL_CINEMACHINE
		protected CinemachineBrain _brain;
		#endif

		/// <summary>
		/// On Awake we store our brain reference
		/// </summary>
		protected virtual void Awake()
		{
			#if PL_CINEMACHINE
			_brain = this.gameObject.GetComponent<CinemachineBrain>();
			#endif
		}

		/// <summary>
		/// Changes the default blend duration for this brain to the one set in parameters
		/// </summary>
		/// <param name="newDuration"></param>
		public virtual void SetDefaultBlendDuration(float newDuration)
		{
			#if PL_CINEMACHINE
			_brain.m_DefaultBlend.m_Time = newDuration;
			#endif
		}

		/// <summary>
		/// When we get a brain event, we treat it
		/// </summary>
		/// <param name="cinemachineBrainEvent"></param>
		public virtual void OnMMEvent(PLCinemachineBrainEvent cinemachineBrainEvent)
		{
			switch (cinemachineBrainEvent.EventType)
			{
				case PLCinemachineBrainEventTypes.ChangeBlendDuration:
					SetDefaultBlendDuration(cinemachineBrainEvent.Duration);
					break;
			}
		}

		/// <summary>
		/// On enable we start listening for events
		/// </summary>
		protected virtual void OnEnable()
		{
			this.PLEventStartListening<PLCinemachineBrainEvent>();
		}

		/// <summary>
		/// On disable we stop listening for events
		/// </summary>
		protected virtual void OnDisable()
		{
			this.PLEventStopListening<PLCinemachineBrainEvent>();
		}
	}
}