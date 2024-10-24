﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SpectralDepths.Tools;

namespace SpectralDepths.InventoryEngine
{	
	/// <summary>
	/// A class used to display an item's details in GUI
	/// </summary>
	public class InventoryDetails : MonoBehaviour, PLEventListener<PLInventoryEvent>
	{
		/// the reference inventory from which we'll display item details
		[PLInformation("Specify here the name of the inventory whose content's details you want to display in this Details panel. You can also decide to make it global. If you do so, it'll display the details of all items, regardless of their inventory.",PLInformationAttribute.InformationType.Info,false)]
		public string TargetInventoryName;
		public string CharacterID = "Player1";
		/// if you make this panel global, it'll ignore 
		public bool Global = false;
		/// whether the details are currently hidden or not 
		public bool Hidden { get; protected set; }

		[Header("Default")]
		[PLInformation("By checking HideOnEmptySlot, the Details panel won't be displayed if you select an empty slot.",PLInformationAttribute.InformationType.Info,false)]
		/// whether or not the details panel should be hidden when the currently selected slot is empty
		public bool HideOnEmptySlot=true;
		[PLInformation("Here you can set default values for all fields of the details panel. These values will be displayed when no item is selected (and if you've chosen not to hide the panel in that case).",PLInformationAttribute.InformationType.Info,false)]
		/// the title to display when none is provided
		public string DefaultTitle;
		/// the short description to display when none is provided
		public string DefaultShortDescription;
		/// the description to display when none is provided
		public string DefaultDescription;
		/// the quantity to display when none is provided
		public string DefaultQuantity;
		/// the icon to display when none is provided
		public Sprite DefaultIcon;

		[Header("Behaviour")]
		[PLInformation("Here you can decide whether or not to hide the details panel on start.",PLInformationAttribute.InformationType.Info,false)]
		/// whether or not to hide the details panel at start
		public bool HideOnStart = true;

		[Header("Components")]
		[PLInformation("Here you need to bind the panel components.",PLInformationAttribute.InformationType.Info,false)]
		/// the icon container object
		public Image Icon;
		/// the title container object
		public Text Title;
		/// the short description container object
		public Text ShortDescription;
		/// the description container object
		public Text Description;
		/// the quantity container object
		public Text Quantity;

		protected float _fadeDelay=0.2f;
		protected CanvasGroup _canvasGroup;

		/// <summary>
		/// On Start, we grab and store the canvas group and determine our current Hidden status
		/// </summary>
		protected virtual void Start()
		{
			_canvasGroup = GetComponent<CanvasGroup>();

			if (HideOnStart)
			{
				_canvasGroup.alpha = 0;
			}

			if (_canvasGroup.alpha == 0)
			{
				Hidden = true;
			}
			else
			{
				Hidden = false;
			}
		}

		/// <summary>
		/// Starts the display coroutine or the panel's fade depending on whether or not the current slot is empty
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void DisplayDetails(InventoryItem item)
		{
			if (InventoryItem.IsNull(item))
			{
				if (HideOnEmptySlot && !Hidden)
				{
					StartCoroutine(PLFade.FadeCanvasGroup(_canvasGroup,_fadeDelay,0f));
					Hidden=true;
				}
				if (!HideOnEmptySlot)
				{
					StartCoroutine(FillDetailFieldsWithDefaults(0));
				}
			}
			else
			{
				StartCoroutine(FillDetailFields(item,0f));

				if (HideOnEmptySlot && Hidden)
				{
					StartCoroutine(PLFade.FadeCanvasGroup(_canvasGroup,_fadeDelay,1f));
					Hidden=false;
				}
			}
		}

		/// <summary>
		/// Fills the various detail fields with the item's metadata
		/// </summary>
		/// <returns>The detail fields.</returns>
		/// <param name="item">Item.</param>
		/// <param name="initialDelay">Initial delay.</param>
		protected virtual IEnumerator FillDetailFields(InventoryItem item, float initialDelay)
		{
			yield return new WaitForSeconds(initialDelay);
			if (Title!=null) { Title.text = item.ItemName ; }
			if (ShortDescription!=null) { ShortDescription.text = item.ShortDescription;}
			if (Description!=null) { Description.text = item.Description;}
			if (Quantity!=null) { Quantity.text = item.Quantity.ToString();}
			if (Icon!=null) { Icon.sprite = item.Icon;}
			
			if (HideOnEmptySlot && !Hidden && (item.Quantity == 0))
			{
				StartCoroutine(PLFade.FadeCanvasGroup(_canvasGroup,_fadeDelay,0f));
				Hidden=true;
			}
		}

		/// <summary>
		/// Fills the detail fields with default values.
		/// </summary>
		/// <returns>The detail fields with defaults.</returns>
		/// <param name="initialDelay">Initial delay.</param>
		protected virtual IEnumerator FillDetailFieldsWithDefaults(float initialDelay)
		{
			yield return new WaitForSeconds(initialDelay);
			if (Title!=null) { Title.text = DefaultTitle ;}
			if (ShortDescription!=null) { ShortDescription.text = DefaultShortDescription;}
			if (Description!=null) { Description.text = DefaultDescription;}
			if (Quantity!=null) { Quantity.text = DefaultQuantity;}
			if (Icon!=null) { Icon.sprite = DefaultIcon;}
		}

		/// <summary>
		/// Catches PLInventoryEvents and displays details if needed
		/// </summary>
		/// <param name="inventoryEvent">Inventory event.</param>
		public virtual void OnMMEvent(PLInventoryEvent inventoryEvent)
		{
			// if this event doesn't concern our inventory display, we do nothing and exit
			if (!Global && (inventoryEvent.TargetInventoryName != this.TargetInventoryName))
			{
				return;
			}

			if (inventoryEvent.CharacterID != CharacterID)
			{
				return;
			}

			switch (inventoryEvent.InventoryEventType)
			{
				case PLInventoryEventType.Select:
					DisplayDetails (inventoryEvent.EventItem);
					break;
				case PLInventoryEventType.UseRequest:
					DisplayDetails (inventoryEvent.EventItem);
					break;
				case PLInventoryEventType.InventoryOpens:
					DisplayDetails (inventoryEvent.EventItem);
					break;
				case PLInventoryEventType.Drop:
					DisplayDetails (null);
					break;
				case PLInventoryEventType.EquipRequest:
					DisplayDetails (null);
					break;
			}
		}

		/// <summary>
		/// On Enable, we start listening for PLInventoryEvents
		/// </summary>
		protected virtual void OnEnable()
		{
			this.PLEventStartListening<PLInventoryEvent>();
		}

		/// <summary>
		/// On Disable, we stop listening for PLInventoryEvents
		/// </summary>
		protected virtual void OnDisable()
		{
			this.PLEventStopListening<PLInventoryEvent>();
		}
	}
}