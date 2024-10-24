﻿using UnityEngine;

namespace SpectralDepths.TopDown
{
	public enum DamageTypeModes { BaseDamage, TypedDamage }
	/// <summary>
	/// A scriptable object you can create assets from, to identify damage types
	/// </summary>
	[CreateAssetMenu(menuName = "SpectralDepths/TopDownEngine/DamageType", fileName = "DamageType")]
	public class DamageType : ScriptableObject
	{
	}    
}