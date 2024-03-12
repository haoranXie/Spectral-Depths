
using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using SpectralDepths.Feedbacks;
using System.Collections.Generic;
using UnityEngine.AI;

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

		protected CharacterOrientation3D _characterOrientation3D;
		protected CharacterController _characterController;
		protected CharacterSelectable _characterSelectable;
		protected NavMeshAgent _navMeshAgent;
		protected override void Initialization()
		{
			base.Initialization();
			if (CharacterIndex==-1){ CharacterIndex= LevelManager.Instance.Players.IndexOf(_character)+2; }
			_characterSelectable = GetComponent<CharacterSelectable>();
			_characterController = GetComponent<CharacterController>();
			_characterOrientation3D = GetComponent<CharacterOrientation3D>();
			_characterController = GetComponent<CharacterController>();
			_navMeshAgent = GetComponent<NavMeshAgent>();
			
			switch(_character.CharacterType)
			{
				//If the Character is under Player controls
				case Character.CharacterTypes.Player:
					EmeraldModeOff();
					CharacterModeOn();
					break;
				//If the Character is under AI controls
				case Character.CharacterTypes.AI:
					EmeraldModeOn();
					CharacterModeOff();
					break;
			}
			

		}
        
		public override void EarlyProcessAbility()
		{
			base.EarlyProcessAbility();
			if (!AbilityAuthorized)
			{
				return;
			}
			if(QuickSwapOn){QuickSwap();}
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
			/*
			for(int i = 0; i<_handleWeaponList.Count;i++)
			{
				_handleWeaponList[i].ChangeToPlayerVersionOfWeapon();
			}
			_characterMovement.SetMovement(Vector3.zero);
			*/
			_character.SetCharacterType(Character.CharacterTypes.Player);
			CharacterModeOn();
			EmeraldModeOff();
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.RTSOff,_character);
			RTSEvent.Trigger(RTSEventTypes.SwitchToPlayer,_character,null);
			if(UsingProximityManager){ProximityManager.Instance.ProximityTarget=_character.transform;}
			CameraSystem.Instance.SwapToPlayerCamera(_character);
		}

		private void SwitchToAI()
		{
			/*
			for(int i = 0; i<_handleWeaponList.Count;i++)
			{
				_handleWeaponList[i].ChangeToAIVersionOfWeapon();
			}
			_characterMovement.SetMovement(Vector3.zero);
			*/
			_character.SetCharacterType(Character.CharacterTypes.AI);
			CharacterModeOff();
			EmeraldModeOn();
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.RTSOn,_character);
			RTSEvent.Trigger(RTSEventTypes.SwitchToRTS,_character,null);
			if(UsingProximityManager){ProximityManager.Instance.ProximityTarget=CameraSystem.Instance.transform;}
			CameraSystem.Instance.SwapToRTSCamera(this.transform);

		}

		/// <summary>
		/// Switches out components to work with Character based controls
		/// </summary>
		protected void CharacterModeOn()
		{
			_controller.enabled = true;
			_characterMovement.enabled=true;
			_characterOrientation3D.enabled=true;
			_characterController.enabled=true;
			_controller.Reset();
			_character.CacheAbilities();
		}
		/// <summary>
		/// Disables Character based controls
		/// </summary>
		protected void CharacterModeOff()
		{
			_controller.enabled = false;
			_characterMovement.enabled=false;
			_characterOrientation3D.enabled=false;
			_characterController.enabled=false;
			_character.CacheAbilities();
		}
		/// <summary>
		/// Switches out components to work with Emerald based controls
		/// </summary>
		protected void EmeraldModeOn()
		{
			_navMeshAgent.enabled=true;
			_emeraldComponent.MovementComponentOn = true;
			_emeraldComponent.BehaviorsComponentOn = true;
			_emeraldComponent.DetectionComponentOn = true;
			_emeraldComponent.CombatComponentOn = true;
		}
		/// <summary>
		/// Disables Emelerad based controls
		/// </summary>
		protected void EmeraldModeOff()
		{
			_navMeshAgent.enabled=false;
			_emeraldComponent.MovementComponentOn = false;
			_emeraldComponent.BehaviorsComponentOn = false;
			_emeraldComponent.DetectionComponentOn = false;
			_emeraldComponent.CombatComponentOn = false;
		}


		protected override void OnDeath()
		{
			base.OnDeath();
			if(DeathSwitch)
			{	
				//If under player control
				if(_character.CharacterType==Character.CharacterTypes.Player){
					//Reset into RTS mode
					TopDownEngineEvent.Trigger(TopDownEngineEventTypes.RTSOn,_character);
					CameraSystem.Instance.SwapToRTSCamera(this.transform);
				}
				if(UsingProximityManager){ProximityManager.Instance.ProximityTarget=CameraSystem.Instance.transform;}
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