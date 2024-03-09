
using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using SpectralDepths.Feedbacks;
using System.Collections.Generic;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// Intermediatary between AI and Character Controls. Intended to be used with EmeraldAI. Otherwise use CharacterAbilityNodeSwap
	/// </summary>
	[AddComponentMenu("Spectral Depths/Character/Abilities/Character Ability Overdrive")]
	public class CharacterAbilityOverdrive : CharacterAbility
	{
		//Index of Character as represented in LevelManager and UI
		[Header("Settings")]

		[Tooltip("Character Index usually represented in UI and Squad Management. Key for selection")]
		public int CharacterIndex = -1;
		[Tooltip("Whether or not the character is instantly swapped into when clicking on CharacterIndex")]
		public bool QuickSwapOn = true;
		[Tooltip("Whether player should change to RTS mode on Death")]
		public bool DeathSwitch = true;
		[Tooltip("Whether using performance optimizer")]
		public bool UsingProximityManager = false;
		protected override void Initialization()
		{
			base.Initialization();
			if (CharacterIndex==-1){ CharacterIndex= LevelManager.Instance.Players.IndexOf(_character)+1; }
			/*
			switch(_character.CharacterType)
			{
				//If the Character is under Player controls
				case Character.CharacterTypes.Player:
					SwitchToAI();
					break;
				//If the Character is under AI controls
				case Character.CharacterTypes.AI:
					SwitchToPlayer();
					break;
			}
			*/
		}
        
		public override void EarlyProcessAbility()
		{
			base.EarlyProcessAbility();
			if (!AbilityAuthorized)
			{
				return;
			}

			//if(QuickSwapOn){QuickSwap();}

        }

		protected void QuickSwap()
		{
			if(Input.GetKeyDown(KeyCode.Alpha0 + CharacterIndex))
			{
				switch(_character.CharacterType)
				{
					//If the Character is under Player controls
					case Character.CharacterTypes.Player:
						SwitchToAI();
						break;
					//If the Character is under AI controls
					case Character.CharacterTypes.AI:
						SwitchToPlayer();
						break;
				}
			}
		}

		private void SwitchToPlayer()
		{
			_character.SetCharacterType(Character.CharacterTypes.Player);
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.RTSOff,_character);
			RTSEvent.Trigger(RTSEventTypes.SwitchToPlayer,_character,null);
			/*
			for(int i = 0; i<_handleWeaponList.Count;i++)
			{
				_handleWeaponList[i].ChangeToPlayerVersionOfWeapon();
			}
			_characterMovement.SetMovement(Vector3.zero);
			*/
			if(UsingProximityManager){ProximityManager.Instance.ProximityTarget=_character.transform;}
			CameraSystem.Instance.SwapToPlayerCamera(_character);
		}

		private void SwitchToAI()
		{
			_character.SetCharacterType(Character.CharacterTypes.AI);
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.RTSOn,_character);
			RTSEvent.Trigger(RTSEventTypes.SwitchToRTS,_character,null);
			/*
			for(int i = 0; i<_handleWeaponList.Count;i++)
			{
				_handleWeaponList[i].ChangeToAIVersionOfWeapon();
			}
			_characterMovement.SetMovement(Vector3.zero);
			*/
			if(UsingProximityManager){ProximityManager.Instance.ProximityTarget=CameraSystem.Instance.transform;}
			CameraSystem.Instance.SwapToRTSCamera(this.transform);
		}

		/// <summary>
		/// Switches out components to work with Character based controls
		/// </summary>
		protected void CharacterModeOn()
		{
			_controller.enabled = true;
			_controller.CollisionsOn();
			_controller.Reset();
			_character.GetComponent<CharacterController>().enabled=true;
			_character.GetComponent<CharacterMovement>().enabled = true;
			_character.CacheAbilities();
		}
		/// <summary>
		/// Disables Character based controls
		/// </summary>
		protected void CharacterModeOff()
		{
			_controller.enabled = false;
			_character.GetComponent<CharacterController>().enabled=false;
			_character.GetComponent<CharacterMovement>().enabled =false;
			_character.CacheAbilities();
		}
		/// <summary>
		/// Switches out components to work with Emerald based controls
		/// </summary>
		protected void EmeraldModeOn()
		{
			
		}
		/// <summary>
		/// Disables Emelerad based controls
		/// </summary>
		protected void EmeraldModeOff()
		{

			_controller.enabled = false;
			_character.GetComponent<CharacterController>().enabled=false;

		}


		protected override void OnDeath()
		{
			base.OnDeath();
			if(DeathSwitch)
			{
				TopDownEngineEvent.Trigger(TopDownEngineEventTypes.RTSOn,_character);
				if(UsingProximityManager){ProximityManager.Instance.ProximityTarget=CameraSystem.Instance.transform;}
				CameraSystem.Instance.SwapToRTSCamera(this.transform);
			}
		}

        protected override void OnEnable()
        {
            base.OnEnable();

        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

    }

}