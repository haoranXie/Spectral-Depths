using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using System.Collections.Generic;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// This ability lets you swap entire ability nodes for the ones set in parameters
	/// </summary>
	public enum ControlTypes{AI, Player, NoSwap}
	[AddComponentMenu("Spectral Depths/Character/Abilities/CharacterAbilityNodeSwap")]
	public class CharacterAbilityNodeSwap : CharacterAbility
	{
		[Header("Ability Node Swap")]
        
		/// a list of GameObjects that will replace this Character's set of ability nodes when the ability executes
		[Tooltip("a list of GameObjects that will replace this Character's set of ability nodes when the ability executes")]
		public List<GameObject> AdditionalAbilityNodes;
		/// optinal swapping of control types
		[Tooltip("Sets new control type during swap")]
		public ControlTypes ControlType = ControlTypes.NoSwap;
		[Tooltip("Sets new AIBrain during swap")]
		public AIBrain Brain;
		[Tooltip("Whether swapping requires the character to be selected")]
		public bool NeedSelected = true;
		[Tooltip("Whether using performance optimizer")]
		public bool UsingProximityManager = false;
		[Tooltip("Whether player should change to RTS mode on Death")]
		public bool DeathSwitch = false;
		protected override void Initialization()
		{
			base.Initialization();
		}
		/// <summary>
		/// If the player presses the SwitchCharacter button, we swap abilities.
		/// This ability reuses the SwitchCharacter input to avoid multiplying input entries, but feel free to override this method to add a dedicated one
		/// </summary>
		public override void EarlyProcessAbility()
		{
			base.EarlyProcessAbility();
			if (!AbilityAuthorized)
			{
				return;
			}
			if(NeedSelected)
			{
				if(!this.gameObject.GetComponent<CharacterSelectable>().selected )
				{
					return;
				}
				if(!this.gameObject.GetComponent<CharacterSelectable>().OnlySelected)
				{
					return;
				}
			}
			if (InputManager.Instance.SwitchCharacterButton.State.CurrentState == PLInput.ButtonStates.ButtonDown)
			{
				SwapAbilityNodes();
				SwapControlType();
				SwapBrain();
			}
		}

		/// <summary>
		/// Disables the old ability nodes, swaps with the new, and enables them
		/// </summary>
		public virtual void SwapAbilityNodes()
		{
			foreach (GameObject node in _character.AdditionalAbilityNodes)
			{
				node.gameObject.SetActive(false);
			}
            
			_character.AdditionalAbilityNodes = AdditionalAbilityNodes;

			foreach (GameObject node in _character.AdditionalAbilityNodes)
			{
				node.gameObject.SetActive(true);
			}

			_character.CacheAbilities();
		}

		public virtual void SwapControlType()
		{
			switch(ControlType)
			{
				case ControlTypes.NoSwap:
					//Do Nothing
				break;
				case ControlTypes.Player: //Switching to a player controls
					SwitchToPlayer();
				break;
				case ControlTypes.AI: //Switching to mouse driven ai controls
					SwitchToAI();
				break;
			}
		}

		private void SwitchToPlayer()
		{
			_character.SetCharacterType(Character.CharacterTypes.Player);
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.RTSOff,_character);
			RTSEvent.Trigger(RTSEventTypes.SwitchToPlayer,_character,null);
			for(int i = 0; i<_handleWeaponList.Count;i++)
			{
				_handleWeaponList[i].ChangeToPlayerVersionOfWeapon();
			}
			_characterMovement.SetMovement(Vector3.zero);
			if(UsingProximityManager){ProximityManager.Instance.ProximityTarget=_character.transform;}
			CameraSystem.Instance.SwapToPlayerCamera(_character);
		}

		private void SwitchToAI()
		{
			_character.SetCharacterType(Character.CharacterTypes.AI);
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.RTSOn,_character);
			RTSEvent.Trigger(RTSEventTypes.SwitchToRTS,_character,null);
			for(int i = 0; i<_handleWeaponList.Count;i++)
			{
				_handleWeaponList[i].ChangeToAIVersionOfWeapon();
			}
			_characterMovement.SetMovement(Vector3.zero);
			if(UsingProximityManager){ProximityManager.Instance.ProximityTarget=CameraSystem.Instance.transform;}
			CameraSystem.Instance.SwapToRTSCamera(this.transform);
		}
		public virtual void SwapBrain()
		{
			_character.SetAIBrain(Brain);
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
	}
}