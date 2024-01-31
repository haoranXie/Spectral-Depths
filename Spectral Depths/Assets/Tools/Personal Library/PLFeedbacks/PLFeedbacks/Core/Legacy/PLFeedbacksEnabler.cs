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
	public class PLFeedbacksEnabler : MonoBehaviour
	{
		/// the PLFeedbacks to pilot
		public PLFeedbacks TargetMMFeedbacks { get; set; }
        
		/// <summary>
		/// On enable, we re-enable (and thus play) our PLFeedbacks if needed
		/// </summary>
		protected virtual void OnEnable()
		{
			if ((TargetMMFeedbacks != null) && !TargetMMFeedbacks.enabled && TargetMMFeedbacks.AutoPlayOnEnable)
			{
				TargetMMFeedbacks.enabled = true;
			}
		}
	}    
}