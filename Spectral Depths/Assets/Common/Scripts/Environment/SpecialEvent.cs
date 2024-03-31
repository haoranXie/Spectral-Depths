using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using SpectralDepths.Feedbacks;

namespace SpectralDepths.TopDown
{
	public class SpecialEvent : ButtonActivated, PLEventListener<TopDownEngineEvent>
	{

		[Header("Special Events")]

		/// the new timescale to apply
		[Tooltip("Whether to trigger a VN Scene")]
		public bool VNScene;
		///
		[Tooltip("The VN Script to swap to")]
		[PLCondition("VNScene", true)] 
		[SerializeField] private TextAsset fileToRead = null;
		[Tooltip("Whether or not to show a notification")]
		public bool NotificationToTrigger;
		[Tooltip("Reference to the gameobject notification")]
		[PLCondition("NotificationToTrigger", true)] 
		[SerializeField] private GameObject _notificationToShow = null;
		public bool ConsecutiveNotification = false;
		private bool _justActivatedVn = false;
		/// <summary>
		/// When the button is pressed we start modifying the timescale
		/// </summary>
		public override void TriggerButtonAction()
		{
			if (!CheckNumberOfUses())
			{
				return;
			}
			base.TriggerButtonAction();
			if(VNScene) ActivateVNMode();
			else if(NotificationToTrigger) ShowNotification();
		}

		private void ShowNotification()
		{
			_notificationToShow.gameObject.SetActive(true);
			if(GUIManager.Instance!=null) GUIManager.Instance.AnimationPlayer.SetTrigger("TurtorialNotificationOn");
		}
		private void ActivateVNMode()
		{
			_justActivatedVn = true;
			//TopDownEngineEvent.Trigger(TopDownEngineEventTypes.TogglePause, null);
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.ActiveCinematicMode, null);
			VNEvent.Trigger(VNEventTypes.ChangeVNScene,fileToRead);
		}

		public override void TriggerExitAction(GameObject collider)
		{
			if (!CheckConditions(collider))
			{
				return;
			}

			if (!TestForLastObject(collider))
			{
				return;
			}
		}
		public virtual void OnMMEvent(TopDownEngineEvent engineEvent)
		{
			switch (engineEvent.EventType)
			{
				case TopDownEngineEventTypes.SwitchToGameMode:
					if(_justActivatedVn)
					{
						_justActivatedVn = false;
						if(ConsecutiveNotification) StartCoroutine(DelayShowNotification());
					}
					break;
			}
		}

		IEnumerator DelayShowNotification()
		{
			yield return new WaitForSeconds(1);
			ShowNotification();
		}
		/// <summary>
		/// Pauses the game without Menu
		/// </summary>
		public virtual void Pause()
		{	
			// if time is not already stopped		
			if (Time.timeScale>0.0f)
			{
				PLTimeScaleEvent.Trigger(PLTimeScaleMethods.For, 0f, 0f, false, 0f, true);
			}
			LevelManager.Instance.ToggleCharacterPause();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			this.PLEventStartListening<TopDownEngineEvent>();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			this.PLEventStopListening<TopDownEngineEvent>();
		}
	}
}