using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Ideal parameters that can be distributed across characters to help with fine tuning
/// </summary>
public static class IdealParams
{
    public class Stats
    {
        //Amount of max HP 
        public static int HeavyHealth = 1000;
        public static int MediumHealth = 500;
        public static int LightHealth = 100;
        //Amount of poise for all characters
        public static float Poise = 100;
    }
    public class Resistance
    {
        public static int PoiseResistance = 1;
    }
    public class Damage
    {
        //Amount of damage dealt against the Poise Stat
        public static float HeavyPoiseDamage = 75;
        public static float MediumPoiseDamage = 25;
        public static float LightPoiseDamage = 7;
    }   

    public class Special
    {
		[Header("Special")]
		[Tooltip("Amount of time after not being hit for the character to reset their poise")]
        public static float PoiseResetTime = 5f;
    }
}
