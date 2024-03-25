using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Tools;

namespace SpectralDepths.TopDown
{
    [CreateAssetMenu(fileName = "Character Data", menuName = "SpectralDepths/Character Data")]
    public class CharacterData : ScriptableObject
    {
		[Header("Identification")]
		[Tooltip("Name displayed for the character in the UI")]
        public string CharacterName;
		[Tooltip("Preset used for the Character's stats")]
        public string Class;
		[Tooltip("Image displayed for the character in the UI")]
        public Sprite Icon; 
		[Header("Stats")]
		[Tooltip("Max Health for the Character")]
        public int MaxHealth = IdealParams.Stats.LightHealth;
		[Tooltip("Max Poise for the Character")]
        public float MaxPoise = IdealParams.Stats.Poise;
		[Header("Multipliers")]
		[Tooltip("Multiplier for the amount of Poise damage a character recives")]
        public float PoiseResistance = IdealParams.Resistance.PoiseResistance;
		[Header("Special")]
		[Tooltip("Amount of time after not being hit for the character to reset their poise")]
        public float PoiseResetTime = IdealParams.Special.PoiseResetTime;
    }
}