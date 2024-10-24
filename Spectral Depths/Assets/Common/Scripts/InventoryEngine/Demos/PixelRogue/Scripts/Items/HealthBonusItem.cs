﻿using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using System;

namespace SpectralDepths.InventoryEngine
{	
	[CreateAssetMenu(fileName = "HealthBonusItem", menuName = "SpectralDepths/InventoryEngine/HealthBonusItem", order = 1)]
	[Serializable]
	/// <summary>
	/// Demo class for a health item
	/// </summary>
	public class HealthBonusItem : InventoryItem 
	{
		[Header("Health Bonus")]
		/// the amount of health to add to the player when the item is used
		public int HealthBonus;

		/// <summary>
		/// What happens when the object is used 
		/// </summary>
		public override bool Use(string CharacterID)
		{
			base.Use(CharacterID);
			// This is where you would increase your character's health,
			// with something like : 
			// Player.Life += HealthValue;
			// of course this all depends on your game codebase.
			Debug.LogFormat("increase character "+CharacterID+"'s health by "+HealthBonus);
			return true;
		}
		
	}
}