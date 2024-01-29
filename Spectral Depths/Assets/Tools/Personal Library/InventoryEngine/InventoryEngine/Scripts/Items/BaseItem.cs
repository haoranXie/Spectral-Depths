using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using System;

namespace SpectralDepths.InventoryEngine
{	
	[CreateAssetMenu(fileName = "BaseItem", menuName = "SpectralDepths/InventoryEngine/BaseItem", order = 0)]
	[Serializable]
	/// <summary>
	/// Base item class, to use when your object doesn't do anything special
	/// </summary>
	public class BaseItem : InventoryItem 
	{
				
	}
}