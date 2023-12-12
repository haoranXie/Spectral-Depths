using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// This decision will return true if character isn't moving
	/// </summary>
	[AddComponentMenu("Spectral Depths/Character/AI/Decisions/AIDecisionNotMoving")]
	public class AIDecisionNotMoving : AIDecision
	{        
		/// <summary>
		/// On Decide we check whether the force move command
		/// </summary>
		/// <returns></returns>
		public override bool Decide()
		{
			return CheckIfMoving();
		}

		/// <summary>
		/// Returns true if force move command
		/// </summary>
		/// <returns></returns>
		protected virtual bool CheckIfMoving()
		{
			if (_brain.Owner.GetComponent<CharacterMovement>().GetMovementVector().magnitude<=0.9)
			{				
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}