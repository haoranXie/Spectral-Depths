using SpectralDepths.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// This Decision will return true if its PLConeOfVision has detected at least one target, and will set it as the Brain's target
	/// </summary>
	[AddComponentMenu("Spectral Depths/Character/AI/Decisions/AIDecisionDetectTargetConeOfVision3D")]
	public class AIDecisionDetectTargetConeOfVision3D : AIDecision
	{
		/// if this is true, this decision will set the AI Brain's Target to null if no target is found
		[Tooltip("if this is true, this decision will set the AI Brain's Target to null if no target is found")]
		public bool SetTargetToNullIfNoneIsFound = true;

		public PLConeOfVision TargetConeOfVision;

		/// <summary>
		/// On Init we grab our PLConeOfVision
		/// </summary>
		public override void Initialization()
		{
			base.Initialization();
			if (TargetConeOfVision == null)
			{
				TargetConeOfVision = this.gameObject.GetComponent<PLConeOfVision>();    
			}
		}

		/// <summary>
		/// On Decide we look for a target
		/// </summary>
		/// <returns></returns>
		public override bool Decide()
		{
			return DetectTarget();
		}

		/// <summary>
		/// If the PLConeOfVision has at least one target, it becomes our new brain target and this decision is true, otherwise it's false.
		/// </summary>
		/// <returns></returns>
		protected virtual bool DetectTarget()
		{
			if (TargetConeOfVision.VisibleTargets.Count == 0)
			{
				if (SetTargetToNullIfNoneIsFound) { _brain.Target = null; }               
				return false;
			}
			else
			{
				_brain.Target = TargetConeOfVision.VisibleTargets[0];
				return true;
			}
		}
	}
}