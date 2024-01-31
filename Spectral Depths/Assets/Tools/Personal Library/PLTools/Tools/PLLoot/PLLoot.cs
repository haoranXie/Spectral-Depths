using System.Collections;
using System.Collections.Generic;
using SpectralDepths.Tools;
using UnityEngine;

namespace  SpectralDepths.Tools
{
	/// <summary>
	/// A class defining the contents of a PLLootTable
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class PLLoot<T>
	{
		/// the object to return
		public T Loot;
		/// the weight attributed to this specific object in the table
		public float Weight = 1f;
		/// the chance percentage to display for this object to be looted. ChancePercentages are meant to be computed by the PLLootTable class
		[PLReadOnly] 
		public float ChancePercentage;
        
		/// the computed low bound of this object's range
		public float RangeFrom { get; set; }
		/// the computed high bound of this object's range
		public float RangeTo { get; set; }
	}
    
    
	/// <summary>
	/// a PLLoot implementation for gameobjects
	/// </summary>
	[System.Serializable]
	public class PLLootGameObject : PLLoot<GameObject> { }
    
	/// <summary>
	/// a PLLoot implementation for strings
	/// </summary>
	[System.Serializable]
	public class PLLootString : PLLoot<string> { }
    
	/// <summary>
	/// a PLLoot implementation for floats
	/// </summary>
	[System.Serializable]
	public class PLLootFloat : PLLoot<float> { }
    
}