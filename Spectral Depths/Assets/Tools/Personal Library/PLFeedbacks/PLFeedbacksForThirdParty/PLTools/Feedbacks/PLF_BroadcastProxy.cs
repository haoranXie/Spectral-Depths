using SpectralDepths.Tools;
using UnityEngine;

namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// This component will be automatically added by the PLF_Broadcast feedback
	/// </summary>
	public class PLF_BroadcastProxy : MonoBehaviour
	{
		/// the channel on which to broadcast
		[Tooltip("the channel on which to broadcast")]
		[PLReadOnly]
		public int Channel;
		/// a debug view of the current level being broadcasted
		[Tooltip("a debug view of the current level being broadcasted")]
		[PLReadOnly]
		public float DebugLevel;
		/// whether or not a broadcast is in progress (will be false while the value is not changing, and thus not broadcasting)
		[Tooltip("whether or not a broadcast is in progress (will be false while the value is not changing, and thus not broadcasting)")]
		[PLReadOnly]
		public bool BroadcastInProgress = false;

		public float ThisLevel { get; set; }
		protected float _levelLastFrame;

		/// <summary>
		/// On Update we process our broadcast
		/// </summary>
		protected virtual void Update()
		{
			ProcessBroadcast();
		}

		/// <summary>
		/// Broadcasts the value if needed
		/// </summary>
		protected virtual void ProcessBroadcast()
		{
			BroadcastInProgress = false;
			if (ThisLevel != _levelLastFrame)
			{
				PLRadioLevelEvent.Trigger(Channel, ThisLevel);
				BroadcastInProgress = true;
			}
			DebugLevel = ThisLevel;
			_levelLastFrame = ThisLevel;
		}
	}    
}