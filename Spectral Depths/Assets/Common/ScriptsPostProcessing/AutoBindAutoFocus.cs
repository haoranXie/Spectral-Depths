using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Tools;
using SpectralDepths.Feedbacks;
using SpectralDepths.FeedbacksForThirdParty;

namespace SpectralDepths.TopDown
{
	public class AutoBindAutoFocus : TopDownMonoBehaviour, PLEventListener<PLCameraEvent>
	{
		/// the AutoFocus component on the camera
		public PLAutoFocus_URP AutoFocus { get; set; }
		private Character _targetCharacter;

		protected virtual void Start()
		{
			AutoFocus = FindObjectOfType<PLAutoFocus_URP>();
		}
        
		public virtual void OnMMEvent(PLCameraEvent cameraEvent)
		{
			switch (cameraEvent.EventType)
			{
				case PLCameraEventTypes.SetTargetCharacter:
					_targetCharacter =cameraEvent.TargetCharacter;
					break;
				case PLCameraEventTypes.StartFollowing:
					AutoBindAutoFocusToCamera();
					break;
				case PLCameraEventTypes.RefreshAutoFocus:
					AutoBindAutoFocusToCamera();
					break;
			}
		}
		
		protected virtual void AutoBindAutoFocusToCamera()
		{
			if (AutoFocus == null)
			{
				AutoFocus = FindObjectOfType<PLAutoFocus_URP>();
			}
			if (AutoFocus != null)
			{	
				AutoFocus.FocusTargets = new Transform[1];
				if(_targetCharacter!=null)
				{
					AutoFocus.FocusTargets[0] = _targetCharacter.transform;
					return;
				}
				//AutoFocus.FocusTargets[0] = LevelManager.Instance.Players[0].transform; 
			}
		}

		protected virtual void OnEnable()
		{
			this.PLEventStartListening<PLCameraEvent>();
		}

		protected virtual void OnDisable()
		{
			this.PLEventStopListening<PLCameraEvent>();
		}
	}
}