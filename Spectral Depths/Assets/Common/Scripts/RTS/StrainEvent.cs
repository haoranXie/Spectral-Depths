using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Tools;

namespace SpectralDepths.TopDown
{

	public enum StrainEventTypes
	{
		IncreaseStrain,
        DecreaseStrain,
        StrainChanged,
        MaxedStrain
	}
	public struct StrainEvent
	{
        //Relevant amount of energy. For example, UseEnergy AmountOfEnergy subtracts energy while RecoverEnergy adds energy
        public int AmountOfStrain;
		public Character OriginCharacter;
		public StrainEventTypes EventType;
        public StrainEvent(StrainEventTypes eventType, Character originCharacter, int amountOfStrain)
		{
			EventType = eventType;
            AmountOfStrain = amountOfStrain;
			OriginCharacter=originCharacter;
		}

		static StrainEvent e;
        public static void Trigger(StrainEventTypes eventType, Character originCharacter, int amountOfStrain)
		{
			e.EventType = eventType;
            e.AmountOfStrain = amountOfStrain;
			e.OriginCharacter=originCharacter;
            PLEventManager.TriggerEvent(e);
		}
	}
}