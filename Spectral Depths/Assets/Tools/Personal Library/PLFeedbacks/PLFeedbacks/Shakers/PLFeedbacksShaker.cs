using UnityEngine;
using SpectralDepths.Tools;

namespace SpectralDepths.Feedbacks
{
	[RequireComponent(typeof(PLFeedbacks))]
	[AddComponentMenu("Spectral Depths/Feedbacks/Shakers/Feedbacks/PLFeedbacksShaker")]
	public class PLFeedbacksShaker : PLShaker
	{
		protected PLFeedbacks _mmFeedbacks;

		/// <summary>
		/// On init we initialize our values
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			_mmFeedbacks = this.gameObject.GetComponent<PLFeedbacks>();
		}

		public virtual void OnMMFeedbacksShakeEvent(PLChannelData channelData = null, bool useRange = false, float eventRange = 0f, Vector3 eventOriginPosition = default(Vector3))
		{
			if (!CheckEventAllowed(channelData, useRange, eventRange, eventOriginPosition) || (!Interruptible && Shaking))
			{
				return;
			}
			Play();
		}

		protected override void ShakeStarts()
		{
			_mmFeedbacks.PlayFeedbacks();
		}

		/// <summary>
		/// When that shaker gets added, we initialize its shake duration
		/// </summary>
		protected virtual void Reset()
		{
			ShakeDuration = 0.01f;
		}

		/// <summary>
		/// Starts listening for events
		/// </summary>
		public override void StartListening()
		{
			base.StartListening();
			PLFeedbacksShakeEvent.Register(OnMMFeedbacksShakeEvent);
		}

		/// <summary>
		/// Stops listening for events
		/// </summary>
		public override void StopListening()
		{
			base.StopListening();
			PLFeedbacksShakeEvent.Unregister(OnMMFeedbacksShakeEvent);
		}
	}

	public struct PLFeedbacksShakeEvent
	{
		static private event Delegate OnEvent;
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)] private static void RuntimeInitialization() { OnEvent = null; }
		static public void Register(Delegate callback) { OnEvent += callback; }
		static public void Unregister(Delegate callback) { OnEvent -= callback; }

		public delegate void Delegate(PLChannelData channelData = null, bool useRange = false, float eventRange = 0f, Vector3 eventOriginPosition = default(Vector3));

		static public void Trigger(PLChannelData channelData = null, bool useRange = false, float eventRange = 0f, Vector3 eventOriginPosition = default(Vector3))
		{
			OnEvent?.Invoke(channelData, useRange, eventRange, eventOriginPosition);
		}
	}
}