using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// This decision will return true if force move command is issued and this object is selected
	/// </summary>
	[AddComponentMenu("Spectral Depths/Character/AI/Decisions/AIDecisionCommmandForceMoved")]
	public class AIDecisionCommmandForceMoved : AIDecision, MMEventListener<RTSEvent>
	{        
		/// <summary>
		/// On Decide we check whether the force move command
		/// </summary>
		/// <returns></returns>

		private bool _commandMoved;
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
			if (_commandMoved)
			{				
				_commandMoved=false;
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
				case RTSEventTypes.CommandForceMove:
                    if(rtsEvent.SelectedTable.ContainsKey(_brain.Owner.gameObject.GetInstanceID())){
						_commandMoved = true;
					}
					break;
			}
		}
		public void OnEnable()
		{
			this.MMEventStartListening<RTSEvent>();
		}
		public void OnDisable()
		{
			this.MMEventStopListening<RTSEvent>();
		}
	}
}