using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;

namespace SpectralDepths.TopDown
{
	public class AutoBindAutoFocus : TopDownMonoBehaviour, MMEventListener<MMCameraEvent>
	{
		/// the AutoFocus component on the camera
		public MMAutoFocus_URP AutoFocus { get; set; }

		protected virtual void Start()
		{
			AutoFocus = FindObjectOfType<MMAutoFocus_URP>();
		}
        
		public virtual void OnMMEvent(MMCameraEvent cameraEvent)
		{
			switch (cameraEvent.EventType)
			{
				case MMCameraEventTypes.StartFollowing:
					AutoBindAutoFocusToCamera();
					break;
				case MMCameraEventTypes.RefreshAutoFocus:
					AutoBindAutoFocusToCamera();
					break;
			}
		}
		
		protected virtual void AutoBindAutoFocusToCamera()
		{
			if (AutoFocus == null)
			{
				AutoFocus = FindObjectOfType<MMAutoFocus_URP>();
			}
			if (AutoFocus != null)
			{
				AutoFocus.FocusTargets = new Transform[1];
				AutoFocus.FocusTargets[0] = LevelManager.Instance.Players[0].transform; 
			}
		}

		protected virtual void OnEnable()
		{
			this.MMEventStartListening<MMCameraEvent>();
		}

		protected virtual void OnDisable()
		{
			this.MMEventStopListening<MMCameraEvent>();
		}
	}
}