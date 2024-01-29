using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// A class used on the PLFeedback's demo ghost
	/// </summary>
	public class DemoGhost : MonoBehaviour
	{
		/// <summary>
		/// Called via animation event, disables the object
		/// </summary>
		public virtual void OnAnimationEnd()
		{
			this.gameObject.SetActive(false);
		}
	}
}