﻿using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SpectralDepths.InventoryEngine
{	
	[RequireComponent(typeof(InventoryDisplay))]
	/// <summary>
	/// A component that will handle the playing of songs when paired with an InventoryDisplay
	/// </summary>
	public class InventorySoundPlayer : MonoBehaviour, PLEventListener<PLInventoryEvent>
	{
		public enum Modes { Direct, Event }

		[Header("Settings")] 
		/// the mode to choose to play sounds. Direct will play an audiosource, event will call a PLSfxEvent,
		/// meant to be caught by a PLSoundManager 
		public Modes Mode = Modes.Direct;
		
		[Header("Sounds")]
		[PLInformation("Here you can define the default sounds that will get played when interacting with this inventory.",PLInformationAttribute.InformationType.Info,false)]
		/// the audioclip to play when the inventory opens
		public AudioClip OpenFx;
		/// the audioclip to play when the inventory closes
		public AudioClip CloseFx;
		/// the audioclip to play when moving from one slot to another
		public AudioClip SelectionChangeFx;
		/// the audioclip to play when moving from one slot to another
		public AudioClip ClickFX;
		/// the audioclip to play when moving an object successfully
		public AudioClip MoveFX;
		/// the audioclip to play when an error occurs (selecting an empty slot, etc)
		public AudioClip ErrorFx;
		/// the audioclip to play when an item is used, if no other sound has been defined for it
		public AudioClip UseFx;
		/// the audioclip to play when an item is dropped, if no other sound has been defined for it
		public AudioClip DropFx;
		/// the audioclip to play when an item is equipped, if no other sound has been defined for it
		public AudioClip EquipFx;

		protected string _targetInventoryName;
		protected string _targetCharacterID;
		protected AudioSource _audioSource;

		/// <summary>
		/// On Start we setup our player and grab a few references for future use.
		/// </summary>
		protected virtual void Start()
		{
			SetupInventorySoundPlayer ();
			_audioSource = GetComponent<AudioSource> ();
			_targetInventoryName = this.gameObject.PLGetComponentNoAlloc<InventoryDisplay> ().TargetInventoryName;
			_targetCharacterID = this.gameObject.PLGetComponentNoAlloc<InventoryDisplay> ().CharacterID;
		}

		/// <summary>
		/// Setups the inventory sound player.
		/// </summary>
		public virtual void SetupInventorySoundPlayer()
		{
			AddAudioSource ();			
		}

		/// <summary>
		/// Adds an audio source component if needed.
		/// </summary>
		protected virtual void AddAudioSource()
		{
			if (GetComponent<AudioSource>() == null)
			{
				this.gameObject.AddComponent<AudioSource>();
			}
		}

		/// <summary>
		/// Plays the sound specified in the parameter string
		/// </summary>
		/// <param name="soundFx">Sound fx.</param>
		public virtual void PlaySound(string soundFx)
		{
			if (soundFx==null || soundFx=="")
			{
				return;
			}

			AudioClip soundToPlay=null;
			float volume=1f;

			switch (soundFx)
			{
				case "error":
					soundToPlay=ErrorFx;
					volume=1f;
					break;
				case "select":
					soundToPlay=SelectionChangeFx;
					volume=0.5f;
					break;
				case "click":
					soundToPlay=ClickFX;
					volume=0.5f;
					break;
				case "open":
					soundToPlay=OpenFx;
					volume=1f;
					break;
				case "close":
					soundToPlay=CloseFx;
					volume=1f;
					break;
				case "move":
					soundToPlay=MoveFX;
					volume=1f;
					break;
				case "use":
					soundToPlay=UseFx;
					volume=1f;
					break;
				case "drop":
					soundToPlay=DropFx;
					volume=1f;
					break;
				case "equip":
					soundToPlay=EquipFx;
					volume=1f;
					break;
			}

			if (soundToPlay!=null)
			{
				if (Mode == Modes.Direct)
				{
					_audioSource.PlayOneShot(soundToPlay,volume);	
				}
				else
				{
					PLSfxEvent.Trigger(soundToPlay, null, volume, 1);	
				}
			}
		}		

		/// <summary>
		/// Plays the sound fx specified in parameters at the desired volume
		/// </summary>
		/// <param name="soundFx">Sound fx.</param>
		/// <param name="volume">Volume.</param>
		public virtual void PlaySound(AudioClip soundFx,float volume)
		{
			if (soundFx != null)
			{
				if (Mode == Modes.Direct)
				{
					_audioSource.PlayOneShot(soundFx, volume);
				}
				else
				{
					PLSfxEvent.Trigger(soundFx, null, volume, 1);
				}
			}
		}	

		/// <summary>
		/// Catches PLInventoryEvents and acts on them, playing the corresponding sounds
		/// </summary>
		/// <param name="inventoryEvent">Inventory event.</param>
		public virtual void OnMMEvent(PLInventoryEvent inventoryEvent)
		{
			// if this event doesn't concern our inventory display, we do nothing and exit
			if (inventoryEvent.TargetInventoryName != _targetInventoryName)
			{
				return;
			}

			if (inventoryEvent.CharacterID != _targetCharacterID)
			{
				return;
			}

			switch (inventoryEvent.InventoryEventType)
			{
				case PLInventoryEventType.Select:
					this.PlaySound("select");
					break;
				case PLInventoryEventType.Click:
					this.PlaySound("click");
					break;
				case PLInventoryEventType.InventoryOpens:
					this.PlaySound("open");
					break;
				case PLInventoryEventType.InventoryCloses:
					this.PlaySound("close");
					break;
				case PLInventoryEventType.Error:
					this.PlaySound("error");
					break;
				case PLInventoryEventType.Move:
					if (inventoryEvent.EventItem.MovedSound == null)
					{
						if (inventoryEvent.EventItem.UseDefaultSoundsIfNull) { this.PlaySound ("move"); }
					} else
					{
						this.PlaySound (inventoryEvent.EventItem.MovedSound, 1f);
					}
					break;
				case PLInventoryEventType.ItemEquipped:
					if (inventoryEvent.EventItem.EquippedSound == null)
					{
						if (inventoryEvent.EventItem.UseDefaultSoundsIfNull) { this.PlaySound ("equip"); }
					} else
					{
						this.PlaySound (inventoryEvent.EventItem.EquippedSound, 1f);
					}
					break;
				case PLInventoryEventType.ItemUsed:
					if (inventoryEvent.EventItem.UsedSound == null)
					{
						if (inventoryEvent.EventItem.UseDefaultSoundsIfNull) { this.PlaySound ("use"); 	}
					} else
					{
						this.PlaySound (inventoryEvent.EventItem.UsedSound, 1f);
					}
					break;
				case PLInventoryEventType.Drop:
					if (inventoryEvent.EventItem.DroppedSound == null)
					{
						if (inventoryEvent.EventItem.UseDefaultSoundsIfNull) { this.PlaySound ("drop"); 	}
					} else
					{
						this.PlaySound (inventoryEvent.EventItem.DroppedSound, 1f);
					}
					break;
			}
		}

		/// <summary>
		/// OnEnable, we start listening to PLInventoryEvents.
		/// </summary>
		protected virtual void OnEnable()
		{
			this.PLEventStartListening<PLInventoryEvent>();
		}

		/// <summary>
		/// OnDisable, we stop listening to PLInventoryEvents.
		/// </summary>
		protected virtual void OnDisable()
		{
			this.PLEventStopListening<PLInventoryEvent>();
		}
	}
}