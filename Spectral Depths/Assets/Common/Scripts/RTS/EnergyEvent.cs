using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Tools;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// UseEnergy: Uses energy from energy manager
	/// RecoverEnergy: Recovers energy from energy manager
	/// EnergyChanged: Triggered by energy manager when energy is changed
    /// InsufficientAmountofEnergy: Triggered when not enough energy
	/// FinalWaypoint
	/// </summary>
	public enum EnergyEventTypes
	{
		UseEnergy,
        RecoverEnergy,
        EnergyChanged,
        InsufficientAmountofEnergy
	}
	public struct EnergyEvent
	{
        //Relevant amount of energy. For example, UseEnergy AmountOfEnergy subtracts energy while RecoverEnergy adds energy
        public int AmountOfEnergy;
		public Character OriginCharacter;
		public EnergyEventTypes EventType;
        public EnergyEvent(EnergyEventTypes eventType, Character originCharacter, int amountOfEnergy)
		{
			EventType = eventType;
            AmountOfEnergy = amountOfEnergy;
			OriginCharacter=originCharacter;
		}

		static EnergyEvent e;
        public static void Trigger(EnergyEventTypes eventType, Character originCharacter, int amountOfEnergy)
		{
			e.EventType = eventType;
            e.AmountOfEnergy = amountOfEnergy;
			e.OriginCharacter=originCharacter;
            PLEventManager.TriggerEvent(e);
		}
	}
}