
using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using SpectralDepths.Feedbacks;
using System.Collections.Generic;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// Intermediatary between AI and Character Controls
	/// </summary>
	[AddComponentMenu("Spectral Depths/Character/Abilities/Character Ability Overdrive")]
	public class CharacterAbilityOverdrive : CharacterAbility
	{
		//Index of Character as represented in LevelManager and UI
		private int _characterIndex;

		protected override void Initialization()
		{
			base.Initialization();
			_characterIndex = LevelManager.Instance.Players.IndexOf(_character);
		}
        
		public override void EarlyProcessAbility()
		{
			base.EarlyProcessAbility();
			if (!AbilityAuthorized)
			{
				return;
			}

        }

    }

}