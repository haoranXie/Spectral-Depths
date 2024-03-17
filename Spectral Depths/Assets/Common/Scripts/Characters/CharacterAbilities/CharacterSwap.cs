using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// Add this ability to a Character and it'll be part of a pool of characters in a scene to swap from. 
	/// You'll need a CharacterSwapManager in your scene for this to work.
	/// </summary>
	[PLHiddenProperties("AbilityStopFeedbacks")]
	[AddComponentMenu("Spectral Depths/Character/Abilities/Character Swap")]
	public class CharacterSwap : CharacterAbility
	{
		[Header("Character Swap")]
		/// the order in which this character should be picked 
		[Tooltip("the order in which this character should be picked ")]
		public int Order = 0;
		/// the CharacterID to put back in the Character class once this character gets swapped
		[Tooltip("the CharacterID to put back in the Character class once this character gets swapped")]
		public string CharacterID = "";

		[Header("AI")] 
		/// if this is true, the AI Brain (if there's one on this character) will reset on swap
		[Tooltip("if this is true, the AI Brain (if there's one on this character) will reset on swap")]
		public bool ResetAIBrainOnSwap = true;

		protected string _savedCharacterID;
		protected Character.CharacterTypes _savedCharacterType;

		/// <summary>
		/// On init, we grab our character type and CharacterID and store them for later
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			if(string.IsNullOrEmpty(CharacterID)){CharacterID = _character.CharacterID;}
			_savedCharacterType = _character.CharacterType;
			_savedCharacterID = _character.CharacterID;
		}

		/// <summary>
		/// Called by the CharacterSwapManager, changes this character's type and sets its input manager
		/// </summary>
		public virtual void SwapToThisCharacter()
		{
			if (!AbilityAuthorized)
			{
				return;
			}
			PlayAbilityStartFeedbacks();
			_character.CharacterID = CharacterID;
			_character.CharacterType = Character.CharacterTypes.Player;
			_character.SetInputManager();
			if (_character.CharacterBrain != null)
			{
				_character.CharacterBrain.BrainActive = false;
			}
		}

		/// <summary>
		/// Called when another character replaces this one as the active one, resets its type and player ID and kills its input
		/// </summary>
		public virtual void ResetCharacterSwap()
		{
			_character.CharacterType = Character.CharacterTypes.AI;
			_character.CharacterID = _savedCharacterID;
			_character.SetInputManager(null);
			_characterMovement.SetHorizontalMovement(0f);
			_characterMovement.SetVerticalMovement(0f);
			_character.ResetInput();
			if (_character.CharacterBrain != null)
			{
				_character.CharacterBrain.BrainActive = true;
				if (ResetAIBrainOnSwap)
				{
					_character.CharacterBrain.ResetBrain();    
				}
			}
		}

		/// <summary>
		/// Returns true if this character is the currently active swap character
		/// </summary>
		/// <returns></returns>
		public virtual bool Current()
		{
			return (_character.CharacterType == Character.CharacterTypes.Player);
		}
	}
}