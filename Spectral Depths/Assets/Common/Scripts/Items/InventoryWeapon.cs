using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using System;
using SpectralDepths.InventoryEngine;

namespace SpectralDepths.TopDown
{	
	[CreateAssetMenu(fileName = "InventoryWeapon", menuName = "SpectralDepths/TopDownEngine/InventoryWeapon", order = 2)]
	[Serializable]
	/// <summary>
	/// Weapon item in the Spectral Depths
	/// </summary>
	public class InventoryWeapon : InventoryItem 
	{
		/// the possible auto equip modes
		public enum AutoEquipModes { NoAutoEquip, AutoEquip, AutoEquipIfEmptyHanded }
        
		[Header("Weapon")]
		[PLInformation("Here you need to bind the weapon you want to equip when picking that item.",PLInformationAttribute.InformationType.Info,false)]
		/// the weapon to equip
		[Tooltip("the weapon to equip")]
		public Weapon EquippableWeapon;
		/// how to equip this weapon when picked : not equip it, automatically equip it, or only equip it if no weapon is currently equipped
		[Tooltip("how to equip this weapon when picked : not equip it, automatically equip it, or only equip it if no weapon is currently equipped")]
		public AutoEquipModes AutoEquipMode = AutoEquipModes.NoAutoEquip;
		/// the ID of the CharacterHandleWeapon you want this weapon to be equipped to
		[Tooltip("the ID of the CharacterHandleWeapon you want this weapon to be equipped to")]
		public int HandleWeaponID = 1;

		/// <summary>
		/// When we grab the weapon, we equip it
		/// </summary>
		public override bool Equip(string CharacterID)
		{
			EquipWeapon (EquippableWeapon, CharacterID);
			return true;
		}

		/// <summary>
		/// When dropping or unequipping a weapon, we remove it
		/// </summary>
		public override bool UnEquip(InventoryItem item, string CharacterID)
		{
			// if this is a currently equipped weapon, we unequip it
			if (this.TargetEquipmentInventory(item, CharacterID) == null)
			{
				return false;
			}

			if (this.TargetEquipmentInventory(item, CharacterID).InventoryContains(this.ItemID).Count > 0)
			{
				EquipWeapon(null, CharacterID);
			}

			return true;
		}

		/// <summary>
		/// Grabs the CharacterHandleWeapon component and sets the weapon
		/// </summary>
		/// <param name="newWeapon">New weapon.</param>
		protected virtual void EquipWeapon(Weapon newWeapon, string CharacterID)
		{
			if (EquippableWeapon == null)
			{
				return;
			}
			if (TargetInventory(CharacterID).Owner == null)
			{
				return;
			}

			Character character = TargetInventory(CharacterID).Owner.GetComponentInParent<Character>();

			if (character == null)
			{
				return;
			}

			// we equip the weapon to the chosen CharacterHandleWeapon
			CharacterHandleWeapon targetHandleWeapon = null;
			CharacterHandleWeapon[] handleWeapons = character.GetComponentsInChildren<CharacterHandleWeapon>();
			foreach (CharacterHandleWeapon handleWeapon in handleWeapons)
			{
				if (handleWeapon.HandleWeaponID == HandleWeaponID)
				{
					targetHandleWeapon = handleWeapon;
				}
			}
			
			if (targetHandleWeapon != null)
			{
				targetHandleWeapon.ChangeWeapon(newWeapon, this.ItemID);
			}
		}
	}
}