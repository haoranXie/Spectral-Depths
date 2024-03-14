using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using System;
using SpectralDepths.InventoryEngine;

namespace SpectralDepths.TopDown
{	
	[CreateAssetMenu(fileName = "InventoryEngineHealth", menuName = "SpectralDepths/TopDownEngine/InventoryEngineHealth", order = 1)]
	[Serializable]
	/// <summary>
	/// Pickable health item
	/// </summary>
	public class InventoryEngineHealth : InventoryItem 
	{
		[Header("Health")]
		[PLInformation("Here you need specify the amount of health gained when using this item.",PLInformationAttribute.InformationType.Info,false)]
		/// the amount of health to add to the player when the item is used
		[Tooltip("the amount of health to add to the player when the item is used")]
		public float HealthBonus;

		/// <summary>
		/// When the item is used, we try to grab our character's Health component, and if it exists, we add our health bonus amount of health
		/// </summary>
		public override bool Use(string CharacterID)
		{
			base.Use(CharacterID);

			if (TargetInventory(CharacterID).Owner == null)
			{
				return false;
			}

			Health characterHealth = TargetInventory(CharacterID).Owner.GetComponent<Health>();
			if (characterHealth != null)
			{
				characterHealth.ReceiveHealth(HealthBonus,TargetInventory(CharacterID).gameObject);
				return true;
			}
			else
			{
				return false;
			}
		}

	}
}