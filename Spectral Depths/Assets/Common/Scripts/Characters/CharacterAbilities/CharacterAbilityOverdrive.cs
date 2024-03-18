
using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using SpectralDepths.Feedbacks;
using System.Collections.Generic;
using UnityEngine.AI;
using EmeraldAI.Utility;
using EmeraldAI;
using System;
using UnityEngine.Rendering.Universal;
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
		[Header("Overdrive")]
		[Tooltip("How long overdrive lasts")]
		public float OverdriveLength = 5f;
		/// whether or not to speed up the character during overdrive
		[Tooltip("whether or not to speed up the character during overdrive")]
		public bool SpeedUp = false;
		[Tooltip("Multiplier to all player Action Speeds")]
		[PLCondition("SpeedUp", true)]
		public float OverdriveMultiplier = 1.3f;
		/// whether or not to slow time during overdrive
		[Tooltip("whether or not to slow time during overdrive")]
		public bool SlowTime = false;
		/// the new timescale to apply
		[PLCondition("SlowTime", true)]
		[Tooltip("the new timescale to apply")]
		public float TimeScale = 0.5f;
		/// the duration to apply the new timescale for
		[PLCondition("SlowTime", true)]
		[Tooltip("the duration to apply the new timescale for")]
		public float Duration = 5f;
		/// whether or not the timescale should be lerped
		[Tooltip("whether or not the timescale should be lerped")]
		[PLCondition("SlowTime", true)]
		public bool LerpTimeScale = true;
		/// the speed at which to lerp the timescale
		[Tooltip("the speed at which to lerp the timescale")]
		[PLCondition("SlowTime", true)]
		public float LerpSpeed = 5f;
		[Header("Overdrive VFX")]
		[Tooltip("The Fullscreen effect")]
		public ScriptableRendererFeature FullScreenOverdrive;
		[Tooltip("The Fullscreen effect material")]
		public Material FullScreenOverdriveMaterial;
		[Tooltip("Start Itensity of the voranoi")]
		public float VoranoiItensityStartAmount = 2.5f;
		[Tooltip("Start Itensity of the vignette")]
		public float VignetteItensityStartAmount = 1.25f;
		[Tooltip("How long until overdrive fades out")]
		public float OverdriveFadeOutTime = 1.5f;
		protected CharacterOrientation3D _characterOrientation3D;
		protected CharacterController _characterController;
		protected CharacterSelectable _characterSelectable;
		protected CharacterHandleWeapon _characterHandleWeapon;
		protected CharacterInventory _characterInventory;
		protected NavMeshAgent _navMeshAgent;
		private int _voranoiIntensity = Shader.PropertyToID("_VoranoiIntensity");
		private int _vignetteItensity = Shader.PropertyToID("_VignetteItensity");
		private bool _playerControlled;
		private bool _overdrived = false;
		protected override void Initialization()
		{
			base.Initialization();
			FullScreenOverdrive.SetActive(false);
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
					ResetPlayerWeapon();
					break;
			}
		}

		private IEnumerator OverdriveEffect()
		{
			if(SpeedUp){_animator.SetFloat("Overdrive Multiplier", OverdriveMultiplier);}
			if(SlowTime){PLTimeScaleEvent.Trigger(PLTimeScaleMethods.For, TimeScale, Duration, LerpTimeScale, LerpSpeed, true);}
			FullScreenOverdrive.SetActive(true);
			FullScreenOverdriveMaterial.SetFloat(_voranoiIntensity, VoranoiItensityStartAmount);
			FullScreenOverdriveMaterial.SetFloat(_vignetteItensity, VignetteItensityStartAmount);
			//We wait for the overdrive to finish
			float elapsedTime = 0f;
			while(elapsedTime <= OverdriveLength)
			{
				elapsedTime += Time.unscaledDeltaTime;
				yield return null;
			}
			UnderDrive();
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
				//Unselects all characters
				RTSEvent.Trigger(RTSEventTypes.UnselectedEveryone, null, null);
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
			UnderDrive();
			CharacterModeOff();
			EmeraldModeOn();
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
		/// Resets the player weapon during initliazation if under AI control
		/// </summary>

		protected void ResetPlayerWeapon()
		{
			_characterOrientation3D.RotationMode = CharacterOrientation3D.RotationModes.MovementDirection;
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
			_playerControlled = true;
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
			_playerControlled = false;
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
			_overdrived = true;
			StartCoroutine(OverdriveEffect());
		}
		/// <summary>
		/// Reverses the effects of Overdrive and switches animations to AI controls
		/// </summary>
		protected void UnderDrive()
		{
			_overdrived = false;
			_animator.SetFloat("Overdrive Multiplier", 1f);
			if(FullScreenOverdrive.isActive&& _playerControlled){FullScreenOverdrive.SetActive(false);}
			if(_playerControlled){PLTimeScaleEvent.Trigger(PLTimeScaleMethods.Unfreeze, 1f, 0f, false, 0f, false);}
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
			_emeraldComponent.AnimationComponent.ResetTriggers(0);
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
			_emeraldComponent.AnimationComponent.ResetTriggers(0);
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
			if(_overdrived){PLTimeScaleEvent.Trigger(PLTimeScaleMethods.Unfreeze, 1f, 0f, false, 0f, false);}
		}
        public virtual void OnMMEvent(TopDownEngineEvent engineEvent)
        {
            switch (engineEvent.EventType)
            {
                case TopDownEngineEventTypes.TurnOffOverdrive:
					if(engineEvent.OriginCharacter!=_character && _playerControlled)
					{
						StopCoroutine(OverdriveEffect());
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