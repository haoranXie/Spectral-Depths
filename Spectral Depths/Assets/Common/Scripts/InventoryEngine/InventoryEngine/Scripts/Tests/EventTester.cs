using System.Collections;
using System.Collections.Generic;
using SpectralDepths.InventoryEngine;
using SpectralDepths.Tools;
using UnityEngine;

/// <summary>
/// This class shows examples of how you can listen to PLInventoryEvents, from any class
/// </summary>
public class EventTester : MonoBehaviour, PLEventListener<PLInventoryEvent>
{
	/// <summary>
	/// When we catch a PLInventoryEvent, we filter on its type and display info about the item used
	/// </summary>
	/// <param name="inventoryEvent"></param>
	public virtual void OnMMEvent(PLInventoryEvent inventoryEvent)
	{
		if (inventoryEvent.InventoryEventType == PLInventoryEventType.ItemUsed)
		{
			PLDebug.DebugLogTime("item used");
			PLDebug.DebugLogTime("ItemID : "+inventoryEvent.EventItem.ItemID);
			PLDebug.DebugLogTime("Item name : "+inventoryEvent.EventItem.ItemName);
		}
	}
    
	/// <summary>
	/// On enable we start listening for PLInventoryEvents 
	/// </summary>
	protected virtual void OnEnable()
	{
		this.PLEventStartListening<PLInventoryEvent>();
	}
    
	/// <summary>
	/// On disable we stop listening for PLInventoryEvents 
	/// </summary>
	protected virtual void OnDisable()
	{
		this.PLEventStopListening<PLInventoryEvent>();
	}
}