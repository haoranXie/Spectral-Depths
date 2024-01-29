using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using System;

namespace SpectralDepths.InventoryEngine
{	
	[CreateAssetMenu(fileName = "BombItem", menuName = "SpectralDepths/InventoryEngine/BombItem", order = 2)]
	[Serializable]
	/// <summary>
	/// Demo class for a bomb item
	/// </summary>
	public class BombItem : InventoryItem 
	{
		/// <summary>
		/// When the bomb gets used, we display a debug message just to show it worked
		/// In a real game you'd probably spawn it
		/// </summary>
		public override bool Use(string playerID)
		{
			base.Use(playerID);
			Debug.LogFormat("bomb explosion");
			return true;
		}		
	}
}