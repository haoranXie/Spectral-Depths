using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.InventoryEngine
{
	/// <summary>
	/// When working in a multiplayer context, add this class to characters that can pick items and ItemPickers will automatically send items to the right CharacterID.
	/// </summary>
	public class InventoryCharacterIdentifier : MonoBehaviour
	{
		/// the unique ID of the player
		public string CharacterID = "Player1";
	}    
}