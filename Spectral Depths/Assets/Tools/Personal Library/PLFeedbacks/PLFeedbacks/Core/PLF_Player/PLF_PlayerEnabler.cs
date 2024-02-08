using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// A helper class added automatically by PLFeedbacks if they're in AutoPlayOnEnable mode
	/// This lets them play again should their parent game object be disabled/enabled
	/// </summary>
	[AddComponentMenu("")]
	public class PLF_PlayerEnabler : MonoBehaviour
	{
		/// the PLFeedbacks to pilot
		public PLF_Player TargetMmfPlayer { get; set; }
        
		/// <summary>
		/// On enable, we re-enable (and thus play) our PLFeedbacks if needed
		/// </summary>
		protected virtual void OnEnable()
		{
			if ((TargetMmfPlayer != null) && !TargetMmfPlayer.enabled && TargetMmfPlayer.AutoPlayOnEnable)
			{
				TargetMmfPlayer.enabled = true;
			}
		}
	}    
}