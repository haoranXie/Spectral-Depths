using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Tools;
using System;
using SpectralDepths.TopDown;

namespace SpectralDepths.InventoryEngine
{	
	[CreateAssetMenu(fileName = "EnergyRefiller", menuName = "SpectralDepths/InventoryEngine/EnergyRefillerItem", order = 1)]
	[Serializable]
	/// <summary>
	/// Energy Refiller Item
	/// </summary>
	public class EnergyRefillerItem : InventoryItem 
	{
		[Header("Energy Refilled")]
		/// the amount of energy refilled by item
		public int EnergyAmount;

		/// <summary>
		/// What happens when the object is used 
		/// </summary>
		public override bool Use(string CharacterID)
		{
			base.Use(CharacterID);
			EnergyEvent.Trigger(EnergyEventTypes.RecoverEnergy, null, EnergyAmount);
			return true;
		}
	}
}