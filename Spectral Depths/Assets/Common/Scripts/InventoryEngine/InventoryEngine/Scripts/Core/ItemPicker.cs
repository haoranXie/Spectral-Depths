using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SpectralDepths.Tools;
using UnityEngine.TextCore.Text;
using SpectralDepths.TopDown;
namespace SpectralDepths.InventoryEngine
{	
	/// <summary>
	/// Add this component to an object so it can be picked and added to an inventory
	/// </summary>
	public class ItemPicker : MonoBehaviour 
	{
		[Header("Settings")]
		/// Use Manually assigned inventory
		[Tooltip("Use Manually assigned inventory")]
		public bool UseManualInventory = false;
		/// Character ID
		[Tooltip("Character ID")]
		[PLCondition("UseManualInventory", true)] 
		public string ManualCharacterID = "";
		/// Target character inventory
		[Tooltip("Target character inventory")]
		[PLCondition("UseManualInventory", true)] 
		public string ManualTargetInventory = "";

		[Header("Item to pick")]
		/// the item that should be picked 
		[PLInformation("Add this component to a Trigger box collider 2D and it'll make it pickable, and will add the specified item to its target inventory. Just drag a previously created item into the slot below. For more about how to create items, have a look at the documentation. Here you can also specify how many of that item should be picked when picking the object.",PLInformationAttribute.InformationType.Info,false)]
		public InventoryItem Item ;
		[Header("Pick Quantity")]
		/// the initial quantity of that item that should be added to the inventory when picked
		[Tooltip("the initial quantity of that item that should be added to the inventory when picked")]
		public int Quantity = 1;
		/// the current quantity of that item that should be added to the inventory when picked
		[PLReadOnly]
		[Tooltip("the current quantity of that item that should be added to the inventory when picked")]
		public int RemainingQuantity = 1;
		
		[Header("Conditions")]
		/// if you set this to true, a character will be able to pick this item even if its inventory is full
		[Tooltip("if you set this to true, a character will be able to pick this item even if its inventory is full")]
		public bool PickableIfInventoryIsFull = false;
		/// if you set this to true, the object will be disabled when picked
		[Tooltip("if you set this to true, the object will be disabled when picked")]
		public bool DisableObjectWhenDepleted = false;
		/// if you set this to true, the object will be destroyed when picked
		[Tooltip("if you set this to true, the object will be destroyed when picked")]
		public bool DestroyObjectWhenDepleted = false;
		/// if this is true, this object will only be allowed to be picked by colliders with a Player tag
		[Tooltip("if this is true, this object will only be allowed to be picked by colliders with a Player tag")]
		public bool RequirePlayerTag = false;
		[Tooltip("if this is true, this pickable item will only be pickable by objects with the right layer masks ")]
		public bool RequireLayerMask = true;
		/// what layer masks are required for it to be picked up
		[Tooltip("what layer masks are required for it to be picked up")]
		[PLCondition("RequireLayerMask", true)] 
		public LayerMask PickableLayerMasks;
		/// if this is true, this pickable item will only be pickable by objects with a Character component 
		[Tooltip("if this is true, this pickable item will only be pickable by objects with a Character component")]
		public bool RequireCharacterComponent = true;
		[Header("Position Offset")]
		/// Position Offset when dropped
		[Tooltip(" Position Offset when dropped")]
		public Vector3 PositionOffset = Vector3.zero;
		[Tooltip("Rotation Offset when dropped")]
		public Vector3 RotationOffset = Vector3.zero;
		protected int _pickedQuantity = 0;
		protected Inventory _targetInventory;
		/// <summary>
		/// On Start we initialize our item picker
		/// </summary>
		protected virtual void Start()
		{
			Initialization ();
		}

		/// <summary>
		/// On Init we look for our target inventory
		/// </summary>
		protected virtual void Initialization()
		{
			if(UseManualInventory)
			{
				FindTargetInventory(ManualTargetInventory, ManualCharacterID);
			}
			ResetQuantity();
		}

		/// <summary>
		/// Resets the remaining quantity to the initial quantity
		/// </summary>
		public virtual void ResetQuantity()
		{
			RemainingQuantity = Quantity;
		}
        
		/// <summary>
		/// Triggered when something collides with the picker
		/// </summary>
		/// <param name="collider">Other.</param>
		public virtual void OnTriggerEnter(Collider collider)
		{
			SpectralDepths.TopDown.Character _character = collider.GetComponentInParent<SpectralDepths.TopDown.Character>();
			CharacterInventory characterInventory = collider.GetComponentInParent<CharacterInventory>();

			if(RequireCharacterComponent)
			{
				if(_character==null)
				{
					return;
				}
				if(RequireLayerMask)
				{
					if (( PickableLayerMasks & (1 << _character.gameObject.layer)) == 0)
					{
						return;
					}
				}
			}

			// if what's colliding with the picker ain't a characterBehavior, we do nothing and exit
			if (RequirePlayerTag && (!collider.CompareTag("Player")))
			{
				return;
			}

			string CharacterID = "Player1";
			string targetInventoryName = Item.TargetInventoryName;

			if(characterInventory!=null)
			{
				CharacterID = characterInventory.CharacterID;
				targetInventoryName = characterInventory.MainInventoryName;
			}

			if(UseManualInventory)
			{
				CharacterID = ManualCharacterID;
				targetInventoryName = ManualTargetInventory;
			}

			Pick(targetInventoryName, CharacterID);
		}

		/// <summary>
		/// Triggered when something collides with the picker
		/// </summary>
		/// <param name="collider">Other.</param>
		public virtual void OnTriggerEnter2D (Collider2D collider) 
		{
			// if what's colliding with the picker ain't a characterBehavior, we do nothing and exit
			if (RequirePlayerTag && (!collider.CompareTag("Player")))
			{
				return;
			}

			string CharacterID = "Player1";
			InventoryCharacterIdentifier identifier = collider.GetComponent<InventoryCharacterIdentifier>();
			if (identifier != null)
			{
				CharacterID = identifier.CharacterID;
			}

			Pick(Item.TargetInventoryName, CharacterID);
		}		
		/// <summary>
		/// Picks this item and adds it to the target inventory specified as a parameter
		/// </summary>
		/// <param name="targetInventoryName">Target inventory name.</param>
		public virtual void Pick(string targetInventoryName, string CharacterID = "Player1")
		{
			FindTargetInventory(targetInventoryName, CharacterID);
			if (_targetInventory == null)
			{
				return;
			}

			if (!Pickable(_targetInventory)) 
			{
				PickFail ();
				return;
			}

			DetermineMaxQuantity ();
			if (!Application.isPlaying)
			{
				if (!Item.ForceSlotIndex)
				{
					_targetInventory.AddItem(Item, 1);	
				}
				else
				{
					_targetInventory.AddItemAt(Item, 1, Item.TargetIndex);
				}
			}				
			else
			{
				PLInventoryEvent.Trigger(PLInventoryEventType.Pick, null, Item.TargetInventoryName, Item, _pickedQuantity, 0, CharacterID);
			}				
			if (Item.Pick(CharacterID))
			{
				RemainingQuantity = RemainingQuantity - _pickedQuantity;
				PickSuccess();
				DisableObjectIfNeeded();
			}			
		}

		/// <summary>
		/// Describes what happens when the object is successfully picked
		/// </summary>
		protected virtual void PickSuccess()
		{
			
		}

		/// <summary>
		/// Describes what happens when the object fails to get picked (inventory full, usually)
		/// </summary>
		protected virtual void PickFail()
		{

		}

		/// <summary>
		/// Disables the object if needed.
		/// </summary>
		protected virtual void DisableObjectIfNeeded()
		{
			// we desactivate the gameobject
			if (DisableObjectWhenDepleted && RemainingQuantity <= 0)
			{
				gameObject.SetActive(false);	
			}

			if (DestroyObjectWhenDepleted && RemainingQuantity <= 0)
			{
				if(GetComponentInParent<Magnetic>()!=null){Destroy(GetComponentInParent<Magnetic>().gameObject);}
				else{Destroy(gameObject);}
			}		
		}

		/// <summary>
		/// Determines the max quantity of item that can be picked from this
		/// </summary>
		protected virtual void DetermineMaxQuantity()
		{
			_pickedQuantity = _targetInventory.NumberOfStackableSlots (Item.ItemID, Item.MaximumStack);
			if (RemainingQuantity < _pickedQuantity)
			{
				_pickedQuantity = RemainingQuantity;
			}
		}

		/// <summary>
		/// Returns true if this item can be picked, false otherwise
		/// </summary>
		public virtual bool Pickable(Inventory targetInventory)
		{
			if (!PickableIfInventoryIsFull && targetInventory.NumberOfFreeSlots == 0)
			{
				// we make sure that there isn't a place where we could store it
				int spaceAvailable = 0;
				List<int> list = targetInventory.InventoryContains(Item.ItemID);
				if (list.Count > 0)
				{
					foreach (int index in list)
					{
						spaceAvailable += (Item.MaximumStack - targetInventory.Content[index].Quantity);
					}
				}

				if (Item.Quantity <= spaceAvailable)
				{
					return true;
				}
				else
				{
					return false;	
				}
			}

			return true;
		}

		/// <summary>
		/// Finds the target inventory based on its name
		/// </summary>
		/// <param name="targetInventoryName">Target inventory name.</param>
		public virtual void FindTargetInventory(string targetInventoryName, string CharacterID)
		{
			_targetInventory = null;
			if (targetInventoryName == null)
			{
				return;
			}

			if(UseManualInventory)
			{
				_targetInventory = Inventory.FindInventory(ManualTargetInventory,ManualCharacterID);
			}
			else
			{
				_targetInventory = Inventory.FindInventory(targetInventoryName, CharacterID);
			}
		}
	}
}