using SpectralDepths.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// This decision will return true if force move command is issued and this object is selected
	/// </summary>
	[AddComponentMenu("Spectral Depths/Character/AI/Decisions/AIDecisionCommmandForceAttack")]
	public class AIDecisionCommmandForceAttack : AIDecision, PLEventListener<RTSEvent>
	{        
		/// <summary>
		/// On Decide we check whether the force move command
		/// </summary>
		/// <returns></returns>

		private bool _commandAttackMoved;
		public override bool Decide()
		{
			return CheckIfForceMoved();
		}

		/// <summary>
		/// Returns true if force move command
		/// </summary>
		/// <returns></returns>
		protected virtual bool CheckIfForceMoved()
		{
			if (_commandAttackMoved)
			{				
				_commandAttackMoved=false;
				return true;
			}
			else
			{
				return false;
			}
		}

		public void OnMMEvent(RTSEvent rtsEvent)
		{
			switch(rtsEvent.EventType)
			{
				case RTSEventTypes.CommandForceAttack:
                    if(rtsEvent.SelectedTable.ContainsKey(_brain.Owner.gameObject.GetInstanceID())){
						_commandAttackMoved = true;
					}
					break;
			}
		}
		public override void OnEnterState()
		{
			base.OnEnterState();
			_commandAttackMoved=false;
		}
		public override void OnExitState()
		{
			base.OnEnterState();
			_commandAttackMoved=false;
		}
		public void OnEnable()
		{
			this.PLEventStartListening<RTSEvent>();
		}
		public void OnDisable()
		{
			this.PLEventStopListening<RTSEvent>();
		}
	}
}