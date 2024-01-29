using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// Use this picker to interrupt all damage of the specified type on the character that picks it
	/// </summary>
	[AddComponentMenu("Spectral Depths/Items/DamageOverTimeInterrupter")]
	public class DamageOverTimeInterrupter : PickableItem
	{
		[Header("Damage Over Time Interrupter")]
		/// whether interrupted damage over time should be of a specific type, or if all damage should be interrupted 
		[Tooltip("whether interrupted damage over time should be of a specific type, or if all damage should be interrupted")]
		public bool InterruptByTypeOnly = false;
		/// The type of damage over time this should interrupt
		[Tooltip("The type of damage over time this should interrupt")]
		[PLCondition("InterruptByTypeOnly", true)]
		public DamageType TargetDamageType;
		/// if this is true, only player characters can pick this up
		[Tooltip("if this is true, only player characters can pick this up")]
		public bool OnlyForPlayerCharacter = true;

		/// <summary>
		/// Triggered when something collides with the stimpack
		/// </summary>
		/// <param name="collider">Other.</param>
		protected override void Pick(GameObject picker)
		{
			Character character = picker.gameObject.PLGetComponentNoAlloc<Character>();
			if (OnlyForPlayerCharacter && (character != null) && (_character.CharacterType != Character.CharacterTypes.Player))
			{
				return;
			}

			Health characterHealth = picker.gameObject.PLGetComponentNoAlloc<Health>();
			// else, we give health to the player
			if (characterHealth != null)
			{
				if (InterruptByTypeOnly)
				{
					characterHealth.InterruptAllDamageOverTimeOfType(TargetDamageType);	
				}
				else
				{
					characterHealth.InterruptAllDamageOverTime();	
				}
			}            
		}
	}
}