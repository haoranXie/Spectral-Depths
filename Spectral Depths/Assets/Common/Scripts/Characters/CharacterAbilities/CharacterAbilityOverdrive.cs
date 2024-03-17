
using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using SpectralDepths.Feedbacks;
using System.Collections.Generic;
using UnityEngine.AI;
using EmeraldAI.Utility;
using EmeraldAI;
using System;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// Intermediatary between AI and Character Controls. Intended to be used with EmeraldAI. Otherwise use CharacterAbilityNodeSwap
	/// </summary>
	[AddComponentMenu("Spectral Depths/Character/Abilities/Character Ability Overdrive")]
	public class CharacterAbilityOverdrive : CharacterAbility, PLEventListener<TopDownEngineEvent>
	{
		//Index of Character as represented in LevelManager and UI
		[Header("Settings")]

		[Tooltip("Key used for instantly taking over a character")]
		public int CharacterKey = -1;
		[Tooltip("Whether or not the character is instantly swapped into when clicking on CharacterKey")]
		public bool QuickSwapOn = true;
		[Tooltip("Whether player should change to RTS mode on Death")]
		public bool DeathSwitch = true;
		[Tooltip("Whether using performance optimizer")]
		public bool UsingProximityManager = false;

		protected CharacterOrientation3D _characterOrientation3D;
		protected CharacterController _characterController;
		protected CharacterSelectable _characterSelectable;
		protected CharacterHandleWeapon _characterHandleWeapon;
		protected CharacterInventory _characterInventory;
		protected NavMeshAgent _navMeshAgent;
		public bool Overdrived;
		protected override void Initialization()
		{
			base.Initialization();
			if (CharacterKey==-1){ CharacterKey= LevelManager.Instance.Players.IndexOf(_character)+1; }
			_characterSelectable = GetComponent<CharacterSelectable>();
			_characterController = GetComponent<CharacterController>();
			_characterHandleWeapon = GetComponent<CharacterHandleWeapon>();
			_characterOrientation3D = GetComponent<CharacterOrientation3D>();
			_characterInventory = GetComponent<CharacterInventory>();
			_characterController = GetComponent<CharacterController>();
			_navMeshAgent = GetComponent<NavMeshAgent>();
			if(_characterHandleWeapon!=null){_characterHandleWeapon.OnWeaponChanged+=OnWeaponChanged;}
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
			if(Input.GetKeyDown(KeyCode.Alpha0 + CharacterKey))
			{
				//Turns off the overdrive for all other overdriven characters
				TopDownEngineEvent.Trigger(TopDownEngineEventTypes.TurnOffOverdrive, _character);
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
			if(_handleWeaponList[0].CurrentWeapon!=null)
			{
				for(int i = 0; i<_handleWeaponList.Count;i++)
				{
					_handleWeaponList[i].ChangeToPlayerVersionOfWeapon();
				}
			}
			/*
			_characterMovement.SetMovement(Vector3.zero);
			*/
			CharacterModeOn();
			EmeraldModeOff();
			Overdrive();
			TurnOffRTSMode();
			OnWeaponChanged();
			_emeraldComponent.CombatComponent.ClearTarget();
			if(_emeraldComponent.BehaviorsComponent.IsOrdered){_emeraldComponent.MovementComponent.ReachedOrderedWaypoint();}
		}

		private void SwitchToAI()
		{
			/*
			if(_handleWeaponList[0].CurrentWeapon!=null)
			{			
				for(int i = 0; i<_handleWeaponList.Count;i++)
				{
					_handleWeaponList[i].ChangeToAIVersionOfWeapon();
				}
			}
			/*
			_characterMovement.SetMovement(Vector3.zero);
			*/
			_emeraldComponent.MovementComponent.StartingDestination = transform.position;
			CharacterModeOff();
			EmeraldModeOn();
			UnderDrive();
			TurnOnRTSMode();
			TurnOffPlayerWeapon();
		}

		/// <summary>
		/// Turns off the player's version of their weapon
		/// </summary>
		protected void TurnOffPlayerWeapon()
		{
			_characterOrientation3D.RotationMode = CharacterOrientation3D.RotationModes.MovementDirection;
			_animator.SetBool("Player Controls", false);
			_emeraldComponent.CombatComponent.ExitCombat();
			if(_characterHandleWeapon.CurrentWeapon!=null)
			{
				if(_characterHandleWeapon.CurrentWeapon.GetComponent<WeaponAim3D>() !=null)
				{
					_characterHandleWeapon.CurrentWeapon.GetComponent<WeaponAim3D>().enabled = false;
					Cursor.visible = true;
				}
			}
		}


		/// <summary>
		/// Activates RTS Manager and triggers components like camera back into RTS control mode
		/// </summary>
		protected void TurnOnRTSMode()
		{
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.RTSOn,_character);
			RTSEvent.Trigger(RTSEventTypes.SwitchToRTS,_character,null);
			if(UsingProximityManager){ProximityManager.Instance.ProximityTarget=CameraSystem.Instance.transform;}
			CameraSystem.Instance.SwapToRTSCamera(this.transform);
		}
		/// <summary>
		/// Deactivates RTS Manager and triggers components like camera to be off RTS control mode
		/// </summary>

		protected void TurnOffRTSMode()
		{
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.RTSOff,_character);
			RTSEvent.Trigger(RTSEventTypes.SwitchToPlayer,_character,null);
			if(UsingProximityManager){ProximityManager.Instance.ProximityTarget=_character.transform;}
			CameraSystem.Instance.SwapToPlayerCamera(_character);
		}
		/// <summary>
		/// Switches out components to work with Character based controls
		/// </summary>
		protected void CharacterModeOn()
		{
			_character.SetCharacterType(Character.CharacterTypes.Player);
			_controller.enabled = true;
			_characterMovement.enabled=true;
			if(_characterHandleWeapon!=null){
				_characterHandleWeapon.enabled=true;
			}
			if(_characterInventory!=null){_characterInventory.ProcessInventory=true;}
			if(_characterSelectable!=null){_characterSelectable.DeSelected();}
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
			_character.SetCharacterType(Character.CharacterTypes.AI);
			_controller.enabled = false;
			_characterMovement.enabled=false;
			_characterOrientation3D.enabled=false;
			if(_characterHandleWeapon!=null){_characterHandleWeapon.enabled=false;}
			if(_characterInventory!=null){_characterInventory.ProcessInventory=false;}
            _animator.SetBool("Player Controls", false);
			_characterController.enabled=false;
			_character.CacheAbilities();
		}
		/// <summary>
		/// Activates player animations and overdrives the player in various ways
		/// </summary>
		protected void Overdrive()
		{
			Overdrived = true;
		}
		/// <summary>
		/// Reverses the effects of Overdrive and switches animations to AI controls
		/// </summary>
		protected void UnderDrive()
		{
			Overdrived = false;
		}

		/// <summary>
		/// Called when the equipped weapon changes
		/// </summary>
		void OnWeaponChanged()
		{
			if(_characterHandleWeapon==null) return;
			switch(_character.CharacterType)
			{
				//If the Character is under Player controls
				case Character.CharacterTypes.Player:
					//When the character has a weapon equipped under player controls
					if(_characterHandleWeapon.CurrentWeapon != null)
					{
						_characterOrientation3D.RotationMode = CharacterOrientation3D.RotationModes.WeaponDirection;
						EmeraldCombatManager.ActivateCombatState(_character.EmeraldComponent);
						_animator.SetInteger("Weapon Type State", 1);
						_animator.SetBool("Player Controls", true);
						if(_characterHandleWeapon.CurrentWeapon!=null)
						{
							if(_characterHandleWeapon.CurrentWeapon.GetComponent<WeaponAim3D>() !=null)
							{
								_characterHandleWeapon.CurrentWeapon.GetComponent<WeaponAim3D>().enabled = true;
							}
						}
					}
					//When the character doesn't have a weapon equipped under player controls
					else
					{
						_characterOrientation3D.RotationMode = CharacterOrientation3D.RotationModes.MovementDirection;
						_animator.SetBool("Player Controls", false);
						_emeraldComponent.CombatComponent.ExitCombat();
					}
				break;
				//If the Character is under AI controls
				case Character.CharacterTypes.AI:
					//If the Character has a weapon equipped under AI controls
					if(_characterHandleWeapon.CurrentWeapon != null)
					{
						if(_characterHandleWeapon.CurrentWeapon.GetComponent<WeaponAim3D>() !=null)
						{
							_characterHandleWeapon.CurrentWeapon.GetComponent<WeaponAim3D>().enabled = false;
						}
					}
				break;
			}
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
        public virtual void OnMMEvent(TopDownEngineEvent engineEvent)
        {
            switch (engineEvent.EventType)
            {
                case TopDownEngineEventTypes.TurnOffOverdrive:
					if(engineEvent.OriginCharacter!=_character && Overdrived)
					{
						_emeraldComponent.MovementComponent.StartingDestination = transform.position;
						CharacterModeOff();
						EmeraldModeOn();
						UnderDrive();
						_characterOrientation3D.RotationMode = CharacterOrientation3D.RotationModes.MovementDirection;
						_animator.SetBool("Player Controls", false);
						_emeraldComponent.CombatComponent.ExitCombat();
						if(_characterHandleWeapon.CurrentWeapon!=null)
						{
							if(_characterHandleWeapon.CurrentWeapon.GetComponent<WeaponAim3D>() !=null)
							{
								_characterHandleWeapon.CurrentWeapon.GetComponent<WeaponAim3D>().enabled = false;
							}
						}
					}
					break;
            }
                    
        }
        protected override void OnEnable()
        {
            base.OnEnable();
			this.PLEventStartListening<TopDownEngineEvent> ();
			if(_characterHandleWeapon!=null){_characterHandleWeapon.OnWeaponChanged+=OnWeaponChanged;}

        }

        protected override void OnDisable()
        {
            base.OnDisable();
			this.PLEventStopListening<TopDownEngineEvent> ();
			if(_characterHandleWeapon!=null){_characterHandleWeapon.OnWeaponChanged-=OnWeaponChanged;}
        }
    }

}