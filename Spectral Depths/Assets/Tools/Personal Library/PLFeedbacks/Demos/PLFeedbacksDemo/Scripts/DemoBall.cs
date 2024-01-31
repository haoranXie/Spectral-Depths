using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Tools;

namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// A class handling the lifecycle of the balls included in the PLFeedbacks demo
	/// It waits for 2 seconds after the spawn of the ball, and destroys it, playing a PLFeedbacks while it does so
	/// </summary>
	public class DemoBall : MonoBehaviour
	{
		/// the duration (in seconds) of the life of the ball
		public float LifeSpan = 2f;
		/// the feedback to play when the ball dies
		public PLFeedbacks DeathFeedback;


		/// <summary>
		/// On start, we trigger the programmed death of the ball
		/// </summary>
		protected virtual void Start()
		{
			StartCoroutine(ProgrammedDeath());
		}

		/// <summary>
		/// Waits for 2 seconds, then kills the ball object after having played the PLFeedbacks
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator ProgrammedDeath()
		{
			yield return PLCoroutine.WaitFor(LifeSpan);
			DeathFeedback?.PlayFeedbacks();
			this.gameObject.SetActive(false);
		}
	}
}