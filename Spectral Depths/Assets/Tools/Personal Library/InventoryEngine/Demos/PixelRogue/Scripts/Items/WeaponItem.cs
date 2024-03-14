using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using System;

namespace SpectralDepths.InventoryEngine
{	
	[CreateAssetMenu(fileName = "WeaponItem", menuName = "SpectralDepths/InventoryEngine/WeaponItem", order = 2)]
	[Serializable]
	/// <summary>
	/// Demo class for a weapon item
	/// </summary>
	public class WeaponItem : InventoryItem 
	{
		[Header("Weapon")]
		/// the sprite to use to show the weapon when equipped
		public Sprite WeaponSprite;

		/// <summary>
		/// What happens when the object is used 
		/// </summary>
		public override bool Equip(string CharacterID)
		{
			base.Equip(CharacterID);
			TargetInventory(CharacterID).TargetTransform.GetComponent<InventoryDemoCharacter>().SetWeapon(WeaponSprite,this);
			return true;
		}

		/// <summary>
		/// What happens when the object is used 
		/// </summary>
		public override bool UnEquip(InventoryItem item, string CharacterID)
		{
			base.UnEquip(item, CharacterID);
			TargetInventory(CharacterID).TargetTransform.GetComponent<InventoryDemoCharacter>().SetWeapon(null,this);
			return true;
		}
		
	}
}