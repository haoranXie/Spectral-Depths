using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using SpectralDepths.Feedbacks;
using System.Collections.Generic;
using EmeraldAI;

namespace SpectralDepths.TopDown
{
	public class SpecialEvent : ButtonActivated, PLEventListener<TopDownEngineEvent>
	{

		[Header("UI Driven Events")]

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
		[SerializeField] private AudioSource _audioSource;
		[SerializeField] private AudioClip _notificationClip = null;		
		public bool ConsecutiveNotification = false;
		[Header("Game Driven Events")]
		[Tooltip("Whether or not to trigger movements")]
		public bool MovementToTrigger;
		[PLCondition("MovementToTrigger", true)] 
		public Transform PointToMoveTowards;
		public List<EmeraldSystem> Characters;
		public bool CustomDeath;
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
			if(CustomDeath) LevelManager.Instance.CustomDeath = true;
			if(VNScene) ActivateVNMode();
			else if(NotificationToTrigger) ShowNotification();
			else if(MovementToTrigger) MoveCharacters();
		}

		private void ShowNotification()
		{
			if(_audioSource!=null){_audioSource.PlayOneShot(_notificationClip);}
			_notificationToShow.gameObject.SetActive(true);
			if(GUIManager.Instance!=null) GUIManager.Instance.AnimationPlayer.SetTrigger("TurtorialNotificationOn");
		}

		private void MoveCharacters()
		{
			foreach(EmeraldSystem character in Characters)
			{
				EmeraldAPI.Movement.SetCustomDestination(character, PointToMoveTowards.position);
			}
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
			yield return new WaitForSeconds(0.5f);
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