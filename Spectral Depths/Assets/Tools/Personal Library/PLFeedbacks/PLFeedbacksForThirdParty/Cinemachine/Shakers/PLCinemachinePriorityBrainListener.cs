using System.Collections;
using UnityEngine;
#if PL_CINEMACHINE
using Cinemachine;
#endif
using SpectralDepths.Feedbacks;

namespace SpectralDepths.FeedbacksForThirdParty
{
	/// <summary>
	/// Add this to a Cinemachine brain and it'll be able to accept custom blend transitions (used with PLFeedbackCinemachineTransition)
	/// </summary>
	[AddComponentMenu("Spectral Depths/Feedbacks/Shakers/Cinemachine/PLCinemachinePriorityBrainListener")]
	#if PL_CINEMACHINE
	[RequireComponent(typeof(CinemachineBrain))]
	#endif
	public class PLCinemachinePriorityBrainListener : MonoBehaviour
	{
        
		[HideInInspector] 
		public TimescaleModes TimescaleMode = TimescaleModes.Scaled;
        
        
		public virtual float GetTime() { return (TimescaleMode == TimescaleModes.Scaled) ? Time.time : Time.unscaledTime; }
		public virtual float GetDeltaTime() { return (TimescaleMode == TimescaleModes.Scaled) ? Time.deltaTime : Time.unscaledDeltaTime; }
    
		#if PL_CINEMACHINE    
		protected CinemachineBrain _brain;
		protected CinemachineBlendDefinition _initialDefinition;
		protected Coroutine _coroutine;

		/// <summary>
		/// On Awake we grab our brain
		/// </summary>
		protected virtual void Awake()
		{
			_brain = this.gameObject.GetComponent<CinemachineBrain>();
		}

		/// <summary>
		/// When getting an event we change our default transition if needed
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="forceMaxPriority"></param>
		/// <param name="newPriority"></param>
		/// <param name="forceTransition"></param>
		/// <param name="blendDefinition"></param>
		/// <param name="resetValuesAfterTransition"></param>
		public virtual void OnMMCinemachinePriorityEvent(PLChannelData channelData, bool forceMaxPriority, int newPriority, bool forceTransition, CinemachineBlendDefinition blendDefinition, bool resetValuesAfterTransition, TimescaleModes timescaleMode, bool restore = false)
		{
			if (forceTransition)
			{
				if (_coroutine != null)
				{
					StopCoroutine(_coroutine);
				}
				else
				{
					_initialDefinition = _brain.m_DefaultBlend;
				}
				_brain.m_DefaultBlend = blendDefinition;
				TimescaleMode = timescaleMode;
				_coroutine = StartCoroutine(ResetBlendDefinition(blendDefinition.m_Time));                
			}
		}

		/// <summary>
		/// a coroutine used to reset the default transition to its initial value
		/// </summary>
		/// <param name="delay"></param>
		/// <returns></returns>
		protected virtual IEnumerator ResetBlendDefinition(float delay)
		{
			for (float timer = 0; timer < delay; timer += GetDeltaTime())
			{
				yield return null;
			}
			_brain.m_DefaultBlend = _initialDefinition;
			_coroutine = null;
		}

		/// <summary>
		/// On enable we start listening for events
		/// </summary>
		protected virtual void OnEnable()
		{
			_coroutine = null;
			PLCinemachinePriorityEvent.Register(OnMMCinemachinePriorityEvent);
		}

		/// <summary>
		/// Stops listening for events
		/// </summary>
		protected virtual void OnDisable()
		{
			if (_coroutine != null)
			{
				StopCoroutine(_coroutine);
			}
			_coroutine = null;
			PLCinemachinePriorityEvent.Unregister(OnMMCinemachinePriorityEvent);
		}
		#endif
	}
}