using SpectralDepths.Tools;
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
		private CharacterMovement _characterMovement;
		public override void Initialization()
		{
			base.Initialization();
			_characterMovement = _brain.Owner.GetComponent<CharacterMovement>();
		}
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
			if (_characterMovement.GetMovementVector().magnitude<=0.9)
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