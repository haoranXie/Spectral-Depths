using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.TextCore.Text;
using SpectralDepths.TopDown;
using System.Linq;
namespace SpectralDepths.InventoryEngine
{
	[Serializable]
	/// <summary>
	/// Base inventory class. 
	/// Will handle storing items, saving and loading its content, adding items to it, removing items, equipping them, etc.
	/// </summary>
	public class Inventory : MonoBehaviour, PLEventListener<PLInventoryEvent>, PLEventListener<PLGameEvent>
	{
		public static List<Inventory> RegisteredInventories;
        
		/// The different possible inventory types, main are regular, equipment will have special behaviours (use them for slots where you put the equipped weapon/armor/etc).
		public enum InventoryTypes { Main, Equipment }
	
		/// <summary>
		/// [Header("ID")] 
		/// </summary>
		/// whether or not this inventory should be linked to character's character inventory
		[Tooltip("whether or not this inventory should be linked to character's character inventory")]
		public bool DontLinkToCharacterInventory = false;
		/// a unique ID used to identify the owner of this inventory
		[Tooltip("a unique ID used to identify the owner of this inventory")]
		[PLCondition("DontLinkToCharacterInventory", true)] 
		public string CharacterID = "";
		[Tooltip("a unique ID used to identify the name of this inventory")]
		[PLCondition("DontLinkToCharacterInventory", true)] 
		public string InventoryName = "";
		/// the complete list of inventory items in this inventory
		[Tooltip("This is a realtime view of your Inventory's contents. Don't modify this list via the inspector, it's visible for control purposes only.")]
		[PLReadOnly]
		public InventoryItem[] Content;

		[Header("Inventory Type")]
		/// whether this inventory is a main inventory or equipment one
		[Tooltip("Here you can define your inventory's type. Main are 'regular' inventories. Equipment inventories will be bound to a certain item class and have dedicated options.")]
		public InventoryTypes InventoryType = InventoryTypes.Main;

		[Header("Target Transform")]
		[Tooltip("The TargetTransform is any transform in your scene at which objects dropped from the inventory will spawn.")]
		/// the transform at which objects will be spawned when dropped
		public Transform TargetTransform;

		[Header("Persistence")]
		[Tooltip("Here you can define whether or not this inventory should respond to Load and Save events. If you don't want to have your inventory saved to disk, set this to false. You can also have it reset on start, to make sure it's always empty at the start of this level.")]
		/// whether this inventory will be saved and loaded
		public bool Persistent = true;
		/// whether or not this inventory should be reset on start
		public bool ResetThisInventorySaveOnStart = false;
        
		[Header("Debug")]
		/// If true, will draw the contents of the inventory in its inspector
		[Tooltip("The Inventory component is like the database and controller part of your inventory. It won't show anything on screen, you'll need also an InventoryDisplay for that. Here you can decide whether or not you want to output a debug content in the inspector (useful for debugging).")]
		public bool DrawContentInInspector = false;

		/// the owner of the inventory (for games where you have multiple characters)
		public GameObject Owner { get; set; }

		/// The number of free slots in this inventory
		public int NumberOfFreeSlots => Content.Length - NumberOfFilledSlots;

		/// whether or not the inventory is full (doesn't have any remaining free slots)
		public bool IsFull => NumberOfFreeSlots <= 0;

		/// The number of filled slots 


		public int NumberOfFilledSlots
		{
			get
			{
				int numberOfFilledSlots = 0;
				for (int i = 0; i < Content.Length; i++)
				{
					if (!InventoryItem.IsNull(Content[i]))
					{
						numberOfFilledSlots++;
					}
				}
				return numberOfFilledSlots;
			}
		}

		public int NumberOfStackableSlots(string searchedItemID, int maxStackSize)
		{
			int numberOfStackableSlots = 0;
			int i = 0;

			while (i < Content.Length)
			{
				if (InventoryItem.IsNull(Content[i]))
				{
					numberOfStackableSlots += maxStackSize;
				}
				else
				{
					if (Content[i].ItemID == searchedItemID)
					{
						numberOfStackableSlots += maxStackSize - Content[i].Quantity;
					}
				}
				i++;
			}

			return numberOfStackableSlots;
		}

		public static string _resourceItemPath = "Items/";
		public static string _saveFolderName = "InventoryEngine/";
		public static string _saveFileExtension = ".inventory";

		/// <summary>
		/// Returns (if found) an inventory matching the searched name and CharacterID
		/// </summary>
		/// <param name="inventoryName"></param>
		/// <param name="CharacterID"></param>
		/// <returns></returns>
		public static Inventory FindInventory(string inventoryName, string CharacterID)
		{
			if (inventoryName == null)
			{
				return null;
			}
            
			foreach (Inventory inventory in RegisteredInventories)
			{
				//
				if ((inventory.InventoryName == inventoryName) && (inventory.CharacterID == CharacterID))
				{
					return inventory;
				}
			}
			return null;
		}

		/// <summary>
		/// Returns (if found) an inventory matching the searched name and CharacterID
		/// </summary>
		/// <param name="inventoryName"></param>
		/// <param name="CharacterID"></param>
		/// <returns></returns>
		public static Inventory FindInventoryWithoutName(string CharacterID)
		{            
			foreach (Inventory inventory in RegisteredInventories)
			{
				if ((inventory.CharacterID == CharacterID))
				{
					return inventory;
				}
			}
			return null;
		}

		/// <summary>
		/// On Awake we register this inventory
		/// </summary>
		protected virtual void Awake()
		{
			RegisterInventory();
		}

		/// <summary>
		/// Registers this inventory so other scripts can access it later on
		/// </summary>
		protected virtual void RegisterInventory()
		{
			if(string.IsNullOrEmpty(CharacterID)){
				CharacterID = gameObject.GetInstanceID().ToString();
			}
			if(string.IsNullOrEmpty(InventoryName)){
				switch (InventoryType){
					case InventoryTypes.Equipment:
						InventoryName = CharacterID+"WeaponInventory";
						break;
					case InventoryTypes.Main:
						InventoryName = CharacterID+"MainInventory";
						break;
				}
			}

			if (RegisteredInventories == null)
			{
				RegisteredInventories = new List<Inventory>();
			}
			if (RegisteredInventories.Count > 0)
			{
				for (int i = RegisteredInventories.Count - 1; i >= 0; i--)
				{
					if (RegisteredInventories[i] == null)
					{
						RegisteredInventories.RemoveAt(i);
					}
				}    
			}
			RegisteredInventories.Add(this);
		}

		/// <summary>
		/// Sets the owner of this inventory, useful to apply the effect of an item for example.
		/// </summary>
		/// <param name="newOwner">New owner.</param>
		public virtual void SetOwner(GameObject newOwner)
		{
			Owner = newOwner;
		}

		/// <summary>
		/// Tries to add an item of the specified type. Note that this is name based.
		/// </summary>
		/// <returns><c>true</c>, if item was added, <c>false</c> if it couldn't be added (item null, inventory full).</returns>
		/// <param name="itemToAdd">Item to add.</param>
		public virtual bool AddItem(InventoryItem itemToAdd, int quantity)
		{
			// if the item to add is null, we do nothing and exit
			if (itemToAdd == null)
			{
				Debug.LogWarning(InventoryName + " : The item you want to add to the inventory is null");
				return false;
			}

			List<int> list = InventoryContains(itemToAdd.ItemID);
			// if there's at least one item like this already in the inventory and it's stackable
			if (list.Count > 0 && itemToAdd.MaximumStack > 1)
			{
				// we store items that match the one we want to add
				for (int i = 0; i < list.Count; i++)
				{
					// if there's still room in one of these items of this kind in the inventory, we add to it
					if (Content[list[i]].Quantity < itemToAdd.MaximumStack)
					{
						// we increase the quantity of our item
						Content[list[i]].Quantity += quantity;
						// if this exceeds the maximum stack
						if (Content[list[i]].Quantity > Content[list[i]].MaximumStack)
						{
							InventoryItem restToAdd = itemToAdd;
							int restToAddQuantity = Content[list[i]].Quantity - Content[list[i]].MaximumStack;
							// we clamp the quantity and add the rest as a new item
							Content[list[i]].Quantity = Content[list[i]].MaximumStack;
							AddItem(restToAdd, restToAddQuantity);
						}
						PLInventoryEvent.Trigger(PLInventoryEventType.ContentChanged, null, InventoryName, null, 0, 0, CharacterID);
						return true;
					}
				}
			}
			// if we've reached the max size of our inventory, we don't add the item
			if (NumberOfFilledSlots >= Content.Length)
			{
				return false;
			}
			while (quantity > 0)
			{
				if (quantity > itemToAdd.MaximumStack)
				{
					AddItem(itemToAdd, itemToAdd.MaximumStack);
					quantity -= itemToAdd.MaximumStack;
				}
				else
				{
					AddItemToArray(itemToAdd, quantity);
					quantity = 0;
				}
			}
			// if we're still here, we add the item in the first available slot
			PLInventoryEvent.Trigger(PLInventoryEventType.ContentChanged, null, InventoryName, null, 0, 0, CharacterID);
			return true;
		}
        
		/// <summary>
		/// Adds the specified quantity of the specified item to the inventory, at the destination index of choice
		/// </summary>
		/// <param name="itemToAdd"></param>
		/// <param name="quantity"></param>
		/// <param name="destinationIndex"></param>
		/// <returns></returns>
		public virtual bool AddItemAt(InventoryItem itemToAdd, int quantity, int destinationIndex)
		{
			int tempQuantity = quantity;
			
			if (!InventoryItem.IsNull(Content[destinationIndex]))
			{
				if ((Content[destinationIndex].ItemID != itemToAdd.ItemID) || (Content[destinationIndex].MaximumStack <= 1))
				{
					return false;
				}
				else
				{
					tempQuantity += Content[destinationIndex].Quantity;
				}
			}
			
			if (tempQuantity > itemToAdd.MaximumStack)
			{
				tempQuantity = itemToAdd.MaximumStack;
			}
            
			Content[destinationIndex] = itemToAdd.Copy(CharacterID);
			Content[destinationIndex].Quantity = tempQuantity;
            
			// if we're still here, we add the item in the first available slot
			PLInventoryEvent.Trigger(PLInventoryEventType.ContentChanged, null, InventoryName, null, 0, 0, CharacterID);
			return true;
		}

		/// <summary>
		/// Tries to move the item at the first parameter slot to the second slot
		/// </summary>
		/// <returns><c>true</c>, if item was moved, <c>false</c> otherwise.</returns>
		/// <param name="startIndex">Start index.</param>
		/// <param name="endIndex">End index.</param>
		public virtual bool MoveItem(int startIndex, int endIndex)
		{
			bool swap = false;
			// if what we're trying to move is null, this means we're trying to move an empty slot
			if (InventoryItem.IsNull(Content[startIndex]))
			{
				Debug.LogWarning("InventoryEngine : you're trying to move an empty slot.");
				return false;
			}
			// if both objects are swappable, we'll swap them
			if (Content[startIndex].CanSwapObject)
			{
				if (!InventoryItem.IsNull(Content[endIndex]))
				{
					if (Content[endIndex].CanSwapObject)
					{
						swap = true;
					}
				}
			}
			// if the target slot is empty
			if (InventoryItem.IsNull(Content[endIndex]))
			{
				// we create a copy of our item to the destination
				Content[endIndex] = Content[startIndex].Copy(CharacterID);
				// we remove the original
				RemoveItemFromArray(startIndex);
				// we mention that the content has changed and the inventory probably needs a redraw if there's a GUI attached to it
				PLInventoryEvent.Trigger(PLInventoryEventType.ContentChanged, null, InventoryName, null, 0, 0, CharacterID);
				return true;
			}
			else
			{
				// if we can swap objects, we'll try and do it, otherwise we return false as the slot we target is not null
				if (swap)
				{
					// we swap our items
					InventoryItem tempItem = Content[endIndex].Copy(CharacterID);
					Content[endIndex] = Content[startIndex].Copy(CharacterID);
					Content[startIndex] = tempItem;
					PLInventoryEvent.Trigger(PLInventoryEventType.ContentChanged, null, InventoryName, null, 0, 0, CharacterID);
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		/// <summary>
		/// This method lets you move the item at startIndex to the chosen targetInventory, at an optional endIndex there
		/// </summary>
		/// <param name="startIndex"></param>
		/// <param name="targetInventory"></param>
		/// <param name="endIndex"></param>
		/// <returns></returns>
		public virtual bool MoveItemToInventory(int startIndex, Inventory targetInventory, int endIndex = -1)
		{
			// if what we're trying to move is null, this means we're trying to move an empty slot
			if (InventoryItem.IsNull(Content[startIndex]))
			{
				Debug.LogWarning("InventoryEngine : you're trying to move an empty slot.");
				return false;
			}
            
			// if our destination isn't empty, we exit too
			if ( (endIndex >=0) && (!InventoryItem.IsNull(targetInventory.Content[endIndex])) )
			{
				Debug.LogWarning("InventoryEngine : the destination slot isn't empty, can't move.");
				return false;
			}

			InventoryItem itemToMove = Content[startIndex].Copy(CharacterID);
            
			// if we've specified a destination index, we use it, otherwise we add normally
			if (endIndex >= 0)
			{
				targetInventory.AddItemAt(itemToMove, itemToMove.Quantity, endIndex);    
			}
			else
			{
				targetInventory.AddItem(itemToMove, itemToMove.Quantity);
			}
            
			// we then remove from the original inventory
			RemoveItem(startIndex, itemToMove.Quantity);

			return true;
		}

		/// <summary>
		/// Removes the specified item from the inventory.
		/// </summary>
		/// <returns><c>true</c>, if item was removed, <c>false</c> otherwise.</returns>
		/// <param name="itemToRemove">Item to remove.</param>
		public virtual bool RemoveItem(int i, int quantity)
		{
			if (i < 0 || i >= Content.Length)
			{
				Debug.LogWarning("InventoryEngine : you're trying to remove an item from an invalid index.");
				return false;
			}
			if (InventoryItem.IsNull(Content[i]))
			{
				Debug.LogWarning("InventoryEngine : you're trying to remove from an empty slot.");
				return false;
			}

			quantity = Mathf.Max(0, quantity);
            
			Content[i].Quantity -= quantity;
			if (Content[i].Quantity <= 0)
			{
				bool suppressionSuccessful = RemoveItemFromArray(i);
				PLInventoryEvent.Trigger(PLInventoryEventType.ContentChanged, null, InventoryName, null, 0, 0, CharacterID);
				return suppressionSuccessful;
			}
			else
			{
				PLInventoryEvent.Trigger(PLInventoryEventType.ContentChanged, null, InventoryName, null, 0, 0, CharacterID);
				return true;
			}
		}
        
		/// <summary>
		/// Removes the specified quantity of the item matching the specified itemID
		/// </summary>
		/// <param name="itemID"></param>
		/// <param name="quantity"></param>
		/// <returns></returns>
		public virtual bool RemoveItemByID(string itemID, int quantity)
		{
			if (quantity < 1)
			{
				Debug.LogWarning("InventoryEngine : you're trying to remove an incorrect quantity ("+quantity+") from your inventory.");
				return false;
			}
            
			if (itemID == null || itemID == "")
			{
				Debug.LogWarning("InventoryEngine : you're trying to remove an item but itemID hasn't been specified.");
				return false;
			}

			int quantityLeftToRemove = quantity;
			
            
			List<int> list = InventoryContains(itemID);
			foreach (int index in list)
			{
				int quantityAtIndex = Content[index].Quantity;
				RemoveItem(index, quantityLeftToRemove);
				quantityLeftToRemove -= quantityAtIndex;
				if (quantityLeftToRemove <= 0)
				{
					return true;
				}
			}
			
			return false;
		}

		/// <summary>
		/// Destroys the item stored at index i
		/// </summary>
		/// <returns><c>true</c>, if item was destroyed, <c>false</c> otherwise.</returns>
		/// <param name="i">The index.</param>
		public virtual bool DestroyItem(int i)
		{
			Content[i] = null;

			PLInventoryEvent.Trigger(PLInventoryEventType.ContentChanged, null, InventoryName, null, 0, 0, CharacterID);
			return true;
		}

		/// <summary>
		/// Empties the current state of the inventory.
		/// </summary>
		public virtual void EmptyInventory()
		{
			Content = new InventoryItem[Content.Length];

			PLInventoryEvent.Trigger(PLInventoryEventType.ContentChanged, null, InventoryName, null, 0, 0, CharacterID);
		}

		/// <summary>
		/// Adds the item to content array.
		/// </summary>
		/// <returns><c>true</c>, if item to array was added, <c>false</c> otherwise.</returns>
		/// <param name="itemToAdd">Item to add.</param>
		/// <param name="quantity">Quantity.</param>
		protected virtual bool AddItemToArray(InventoryItem itemToAdd, int quantity)
		{
			if (NumberOfFreeSlots == 0)
			{
				return false;
			}
			int i = 0;
			while (i < Content.Length)
			{
				if (InventoryItem.IsNull(Content[i]))
				{
					Content[i] = itemToAdd.Copy(CharacterID);
					Content[i].Quantity = quantity;
					return true;
				}
				i++;
			}
			return false;
		}

		/// <summary>
		/// Removes the item at index i from the array.
		/// </summary>
		/// <returns><c>true</c>, if item from array was removed, <c>false</c> otherwise.</returns>
		/// <param name="i">The index.</param>
		protected virtual bool RemoveItemFromArray(int i)
		{
			if (i < Content.Length)
			{
				//Content[i].ItemID = null;
				Content[i] = null;
				return true;
			}
			return false;
		}

		/// <summary>
		/// Resizes the array to the specified new size
		/// </summary>
		/// <param name="newSize">New size.</param>
		public virtual void ResizeArray(int newSize)
		{
			InventoryItem[] temp = new InventoryItem[newSize];
			for (int i = 0; i < Mathf.Min(newSize, Content.Length); i++)
			{
				temp[i] = Content[i];
			}
			Content = temp;
		}

		/// <summary>
		/// Returns the total quantity of items matching the specified name
		/// </summary>
		/// <returns>The quantity.</returns>
		/// <param name="searchedItem">Searched item.</param>
		public virtual int GetQuantity(string searchedItemID)
		{
			List<int> list = InventoryContains(searchedItemID);
			int total = 0;
			foreach (int i in list)
			{
				total += Content[i].Quantity;
			}
			return total;
		}

		/// <summary>
		/// Returns a list of all the items in the inventory that match the specified name
		/// </summary>
		/// <returns>A list of item matching the search criteria.</returns>
		/// <param name="searchedType">The searched type.</param>
		public virtual List<int> InventoryContains(string searchedItemID)
		{
			List<int> list = new List<int>();

			for (int i = 0; i < Content.Length; i++)
			{
				if (!InventoryItem.IsNull(Content[i]))
				{
					if (Content[i].ItemID == searchedItemID)
					{
						list.Add(i);
					}
				}
			}
			return list;
		}

		/// <summary>
		/// Returns a list of all the items in the inventory that match the specified class
		/// </summary>
		/// <returns>A list of item matching the search criteria.</returns>
		/// <param name="searchedType">The searched type.</param>
		public virtual List<int> InventoryContains(SpectralDepths.InventoryEngine.ItemClasses searchedClass)
		{
			List<int> list = new List<int>();

			for (int i = 0; i < Content.Length; i++)
			{
				if (InventoryItem.IsNull(Content[i]))
				{
					continue;
				}
				if (Content[i].ItemClass == searchedClass)
				{
					list.Add(i);
				}
			}
			return list;
		}

		/// <summary>
		/// Saves the inventory to a file
		/// </summary>
		public virtual void SaveInventory()
		{
			SerializedInventory serializedInventory = new SerializedInventory();
			FillSerializedInventory(serializedInventory);
			PLSaveLoadManager.Save(serializedInventory, DetermineSaveName(), _saveFolderName);
		}

		/// <summary>
		/// Tries to load the inventory if a file is present
		/// </summary>
		public virtual void LoadSavedInventory()
		{
			SerializedInventory serializedInventory = (SerializedInventory)PLSaveLoadManager.Load(typeof(SerializedInventory), DetermineSaveName(), _saveFolderName);
			ExtractSerializedInventory(serializedInventory);
			PLInventoryEvent.Trigger(PLInventoryEventType.InventoryLoaded, null, InventoryName, null, 0, 0, CharacterID);
		}

		/// <summary>
		/// Fills the serialized inventory for storage
		/// </summary>
		/// <param name="serializedInventory">Serialized inventory.</param>
		protected virtual void FillSerializedInventory(SerializedInventory serializedInventory)
		{
			serializedInventory.InventoryType = InventoryType;
			serializedInventory.DrawContentInInspector = DrawContentInInspector;
			serializedInventory.ContentType = new string[Content.Length];
			serializedInventory.ContentQuantity = new int[Content.Length];
			for (int i = 0; i < Content.Length; i++)
			{
				if (!InventoryItem.IsNull(Content[i]))
				{
					serializedInventory.ContentType[i] = Content[i].ItemID;
					serializedInventory.ContentQuantity[i] = Content[i].Quantity;
				}
				else
				{
					serializedInventory.ContentType[i] = null;
					serializedInventory.ContentQuantity[i] = 0;
				}
			}
		}

		protected InventoryItem _loadedInventoryItem;

		/// <summary>
		/// Extracts the serialized inventory from a file content
		/// </summary>
		/// <param name="serializedInventory">Serialized inventory.</param>
		protected virtual void ExtractSerializedInventory(SerializedInventory serializedInventory)
		{
			if (serializedInventory == null)
			{
				return;
			}

			InventoryType = serializedInventory.InventoryType;
			DrawContentInInspector = serializedInventory.DrawContentInInspector;
			Content = new InventoryItem[serializedInventory.ContentType.Length];
			for (int i = 0; i < serializedInventory.ContentType.Length; i++)
			{
				if ((serializedInventory.ContentType[i] != null) && (serializedInventory.ContentType[i] != ""))
				{
					_loadedInventoryItem = Resources.Load<InventoryItem>(_resourceItemPath + serializedInventory.ContentType[i]);
					if (_loadedInventoryItem == null)
					{
						Debug.LogError("InventoryEngine : Couldn't find any inventory item to load at "+_resourceItemPath
							+" named "+serializedInventory.ContentType[i] + ". Make sure all your items definitions names (the name of the InventoryItem scriptable " +
							"objects) are exactly the same as their ItemID string in their inspector. " +
							"Once that's done, also make sure you reset all saved inventories as the mismatched names and IDs may have " +
							"corrupted them.");
					}
					else
					{
						Content[i] = _loadedInventoryItem.Copy(CharacterID);
						Content[i].Quantity = serializedInventory.ContentQuantity[i];
					}
				}
				else
				{
					Content[i] = null;
				}
			}
		}

		protected virtual string DetermineSaveName()
		{
			return gameObject.name + "_" + CharacterID + _saveFileExtension;
		}

		/// <summary>
		/// Destroys any save file 
		/// </summary>
		public virtual void ResetSavedInventory()
		{
			PLSaveLoadManager.DeleteSave(DetermineSaveName(), _saveFolderName);
			Debug.LogFormat("Inventory save file deleted");
		}

		/// <summary>
		/// Triggers the use and potential consumption of the item passed in parameter. You can also specify the item's slot (optional) and index.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="slot">Slot.</param>
		/// <param name="index">Index.</param>
		public virtual bool UseItem(InventoryItem item, int index, InventorySlot slot = null)
		{
			if (InventoryItem.IsNull(item))
			{
				PLInventoryEvent.Trigger(PLInventoryEventType.Error, slot, InventoryName, null, 0, index, CharacterID);
				return false;
			}
			if (!item.IsUsable)
			{
				return false;
			}
			if (item.Use(CharacterID))
			{
				// remove 1 from quantity
				PLInventoryEvent.Trigger(PLInventoryEventType.ItemUsed, slot, InventoryName, item.Copy(CharacterID), 0, index, CharacterID);
				if (item.Consumable)
				{
					RemoveItem(index, item.ConsumeQuantity);    
				}
			}
			return true;
		}

		/// <summary>
		/// Triggers the use of an item, as specified by its name. Prefer this signature over the previous one if you don't particularly care what slot the item will be taken from in case of duplicates.
		/// </summary>
		/// <param name="itemName"></param>
		/// <returns></returns>
		public virtual bool UseItem(string itemName)
		{
			List<int> list = InventoryContains(itemName);
			if (list.Count > 0)
			{
				UseItem(Content[list[list.Count - 1]], list[list.Count - 1], null);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Equips the item at the specified slot 
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="index">Index.</param>
		/// <param name="slot">Slot.</param>
		public virtual void EquipItem(InventoryItem item, int index, InventorySlot slot = null)
		{
			if (InventoryType == Inventory.InventoryTypes.Main)
			{
				InventoryItem oldItem = null;
				if (InventoryItem.IsNull(item))
				{
					PLInventoryEvent.Trigger(PLInventoryEventType.Error, slot, InventoryName, null, 0, index, CharacterID);
					return;
				}
				// if the object is not equipable, we do nothing and exit
				if (!item.IsEquippable)
				{
					return;
				}
				// if a target equipment inventory is not set, we do nothing and exit
				/*
				if (item.TargetEquipmentInventory(CharacterID) == null)
				{
					Debug.LogWarning("InventoryEngine Warning : " + Content[index].ItemName + "'s target equipment inventory couldn't be found.");
					return;
				}
				*/
				// if the object can't be moved, we play an error sound and exit
				if (!item.CanMoveObject)
				{
					PLInventoryEvent.Trigger(PLInventoryEventType.Error, slot, InventoryName, null, 0, index, CharacterID);
					return;
				}
				// if the object can't be equipped if the inventory is full, and if it indeed is, we do nothing and exit
				if (!item.EquippableIfInventoryIsFull)
				{
					if (item.TargetEquipmentInventory(item, CharacterID).IsFull)
					{
						return;
					}
				}
				// call the equip method of the item
				if (!item.Equip(CharacterID))
				{
					return;
				}
				// if this is a mono slot inventory, we prepare to swap
				if (item.TargetEquipmentInventory(item, CharacterID).Content.Length == 1)
				{
					if (!InventoryItem.IsNull(item.TargetEquipmentInventory(item, CharacterID).Content[0]))
					{
						if (
							(item.CanSwapObject)
							&& (item.TargetEquipmentInventory(item, CharacterID).Content[0].CanMoveObject)
							&& (item.TargetEquipmentInventory(item, CharacterID).Content[0].CanSwapObject)
						)
						{
							// we store the item in the equipment inventory
							oldItem = item.TargetEquipmentInventory(item, CharacterID).Content[0].Copy(CharacterID);
							item.TargetEquipmentInventory(item, CharacterID).EmptyInventory();
						}
					}
				}
				// we add one to the target equipment inventory
				item.TargetEquipmentInventory(item, CharacterID).AddItem(item.Copy(CharacterID), item.Quantity);
				// remove 1 from quantity
				if (item.MoveWhenEquipped)
				{
					RemoveItem(index, item.Quantity);    
				}
				if (oldItem != null)
				{
					oldItem.Swap(CharacterID);
					if (oldItem.ForceSlotIndex)
					{
						AddItemAt(oldItem, oldItem.Quantity, oldItem.TargetIndex);    
					}
					else
					{
						AddItem(oldItem, oldItem.Quantity);    
					}
				}
				PLInventoryEvent.Trigger(PLInventoryEventType.ItemEquipped, slot, InventoryName, item, item.Quantity, index, CharacterID);
				PLInventoryEvent.Trigger(PLInventoryEventType.WeaponInventoryChanged, null, item.TargetEquipmentInventoryName, item, item.Quantity, index, CharacterID);
			}
		}

		/// <summary>
		/// Drops the item, removing it from the inventory and potentially spawning an item on the ground near the character
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="index">Index.</param>
		/// <param name="slot">Slot.</param>
		public virtual void DropItem(InventoryItem item, int index, InventorySlot slot = null)
		{
			if (InventoryItem.IsNull(item))
			{
				PLInventoryEvent.Trigger(PLInventoryEventType.Error, slot, InventoryName, null, 0, index, CharacterID);
				return;
			}

			//If this inventory is not linked to any characters, it should dropped at one of the squad characters
			if(DontLinkToCharacterInventory)
			{
				//First we try to drop it at where a selected character is
				if(GameRTSController.Instance != null)
				{
					if(GameRTSController.Instance.SelectedTable.Count != 0)
					{
						SpectralDepths.TopDown.Character character = GameRTSController.Instance.SelectedTable.FirstOrDefault().Value;
						item.SpawnPrefab(character.CharacterID);
					}
					//Then we try to drop it at where a scene character is at
					else
					{
						item.SpawnPrefab(LevelManager.Instance.Players[0].CharacterID);
					}
				}
				//Then we try to drop it at where a scene character is at
				else
				{
					item.SpawnPrefab(LevelManager.Instance.Players[0].CharacterID);
				}
			}
			//If this is linked to character
			else
			{
				item.SpawnPrefab(CharacterID);
			}
            
			if (InventoryName == item.TargetEquipmentInventoryName)
			{
				if (item.UnEquip(item, CharacterID))
				{
					DestroyItem(index);
				}
			} else
			{
				DestroyItem(index);
			}

		}

		public virtual void DestroyItem(InventoryItem item, int index, InventorySlot slot = null)
		{
			if (InventoryItem.IsNull(item))
			{
				PLInventoryEvent.Trigger(PLInventoryEventType.Error, slot, InventoryName, null, 0, index, CharacterID);
				return;
			}
			DestroyItem(index);
		}

		public virtual void UnEquipItem(InventoryItem item, int index, InventorySlot slot = null)
		{
			// if there's no item at this slot, we trigger an error
			if (InventoryItem.IsNull(item))
			{
				PLInventoryEvent.Trigger(PLInventoryEventType.Error, slot, InventoryName, null, 0, index, CharacterID);
				return;
			}
			// if we're not in an equipment inventory, we trigger an error
			if (InventoryType != InventoryTypes.Equipment)
			{
				PLInventoryEvent.Trigger(PLInventoryEventType.Error, slot, InventoryName, null, 0, index, CharacterID);
				return;
			}
			// we trigger the unequip effect of the item
			if (!item.UnEquip(item, CharacterID))
			{
				return;
			}
			PLInventoryEvent.Trigger(PLInventoryEventType.ItemUnEquipped, slot, InventoryName, item, item.Quantity, index, CharacterID);

			// if there's a target inventory, we'll try to add the item back to it
			if (item.TargetInventory(CharacterID) != null)
			{
				bool itemAdded = false;
				if (item.ForceSlotIndex)
				{
					itemAdded = item.TargetInventory(CharacterID).AddItemAt(item, item.Quantity, item.TargetIndex);
					if (!itemAdded)
					{
						itemAdded = item.TargetInventory(CharacterID).AddItem(item, item.Quantity);    	
					}
				}
				else
				{
					itemAdded = item.TargetInventory(CharacterID).AddItem(item, item.Quantity);    
				}
				
				// if we managed to add the item
				if (itemAdded)
				{
					DestroyItem(index);
				}
				else
				{
					// if we couldn't (inventory full for example), we drop it to the ground
					PLInventoryEvent.Trigger(PLInventoryEventType.Drop, slot, InventoryName, item, item.Quantity, index, CharacterID);
				}
			}
		}

		/// <summary>
		/// Catches inventory events and acts on them
		/// </summary>
		/// <param name="inventoryEvent">Inventory event.</param>
		public virtual void OnMMEvent(PLInventoryEvent inventoryEvent)
		{
			// if this event doesn't concern our inventory display, we do nothing and exit
			if (inventoryEvent.TargetInventoryName != InventoryName)
			{
				return;
			}
			if (inventoryEvent.CharacterID != CharacterID)
			{
				return;
			}
			switch (inventoryEvent.InventoryEventType)
			{
				case PLInventoryEventType.Pick:
					if (inventoryEvent.EventItem.ForceSlotIndex)
					{
						AddItemAt(inventoryEvent.EventItem, inventoryEvent.Quantity, inventoryEvent.EventItem.TargetIndex);    
					}
					else
					{
						AddItem(inventoryEvent.EventItem, inventoryEvent.Quantity);    
					}
					break;

				case PLInventoryEventType.UseRequest:
					UseItem(inventoryEvent.EventItem, inventoryEvent.Index, inventoryEvent.Slot);
					break;

				case PLInventoryEventType.EquipRequest:
					EquipItem(inventoryEvent.EventItem, inventoryEvent.Index, inventoryEvent.Slot);
					break;

				case PLInventoryEventType.UnEquipRequest:
					UnEquipItem(inventoryEvent.EventItem, inventoryEvent.Index, inventoryEvent.Slot);
					break;

				case PLInventoryEventType.Destroy:
					DestroyItem(inventoryEvent.EventItem, inventoryEvent.Index, inventoryEvent.Slot);
					break;

				case PLInventoryEventType.Drop:
					DropItem(inventoryEvent.EventItem, inventoryEvent.Index, inventoryEvent.Slot);
					break;
			}
		}

		/// <summary>
		/// When we catch an PLGameEvent, we do stuff based on its name
		/// </summary>
		/// <param name="gameEvent">Game event.</param>
		public virtual void OnMMEvent(PLGameEvent gameEvent)
		{
			if ((gameEvent.EventName == "Save") && Persistent)
			{
				SaveInventory();
			}
			if ((gameEvent.EventName == "Load") && Persistent)
			{
				if (ResetThisInventorySaveOnStart)
				{
					ResetSavedInventory();
				}
				LoadSavedInventory();
			}
		}

		/// <summary>
		/// On enable, we start listening for PLGameEvents. You may want to extend that to listen to other types of events.
		/// </summary>
		protected virtual void OnEnable()
		{
			this.PLEventStartListening<PLGameEvent>();
			this.PLEventStartListening<PLInventoryEvent>();
		}

		/// <summary>
		/// On disable, we stop listening for PLGameEvents. You may want to extend that to stop listening to other types of events.
		/// </summary>
		protected virtual void OnDisable()
		{
			this.PLEventStopListening<PLGameEvent>();
			this.PLEventStopListening<PLInventoryEvent>();
		}
	}
}