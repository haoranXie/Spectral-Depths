using SpectralDepths.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.InventoryEngine
{	
	/// <summary>
	/// The possible inventory related events
	/// </summary>
	public enum PLInventoryEventType { Pick, Select, Click, Move, UseRequest, ItemUsed, EquipRequest, ItemEquipped, UnEquipRequest, ItemUnEquipped, Drop, Destroy, Error, Redraw, ContentChanged, InventoryOpens, InventoryCloseRequest, InventoryCloses, InventoryLoaded }

	/// <summary>
	/// Inventory events are used throughout the Inventory Engine to let other interested classes know that something happened to an inventory.  
	/// </summary>
	public struct PLInventoryEvent
	{
		/// the type of event
		public PLInventoryEventType InventoryEventType;
		/// the slot involved in the event
		public InventorySlot Slot;
		/// the name of the inventory where the event happened
		public string TargetInventoryName;
		/// the item involved in the event
		public InventoryItem EventItem;
		/// the quantity involved in the event
		public int Quantity;
		/// the index inside the inventory at which the event happened
		public int Index;
		/// the unique ID of the player triggering this event
		public string CharacterID;

		public PLInventoryEvent(PLInventoryEventType eventType, InventorySlot slot, string targetInventoryName, InventoryItem eventItem, int quantity, int index, string characterID)
		{
			InventoryEventType = eventType;
			Slot = slot;
			TargetInventoryName = targetInventoryName;
			EventItem = eventItem;
			Quantity = quantity;
			Index = index;
			CharacterID = (characterID != "") ? characterID : "Player1";
		}

		static PLInventoryEvent e;
		public static void Trigger(PLInventoryEventType eventType, InventorySlot slot, string targetInventoryName, InventoryItem eventItem, int quantity, int index, string characterID)
		{
			e.InventoryEventType = eventType;
			e.Slot = slot;
			e.TargetInventoryName = targetInventoryName;
			e.EventItem = eventItem;
			e.Quantity = quantity;
			e.Index = index;
			e.CharacterID = (characterID != "") ? characterID : "Player1";
			PLEventManager.TriggerEvent(e);
		}
	}
}