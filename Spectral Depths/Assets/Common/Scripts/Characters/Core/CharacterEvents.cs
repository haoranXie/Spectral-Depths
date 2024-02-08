using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;

namespace SpectralDepths.TopDown
{	
	/// <summary>
	/// A list of possible events used by the character
	/// </summary>
	public enum PLCharacterEventTypes
	{
		ButtonActivation,
		Jump
	}

	/// <summary>
	/// PLCharacterEvents are used in addition to the events triggered by the character's state machine, to signal stuff happening that is not necessarily linked to a change of state
	/// </summary>
	public struct PLCharacterEvent
	{
		public Character TargetCharacter;
		public PLCharacterEventTypes EventType;
		/// <summary>
		/// Initializes a new instance of the <see cref="SpectralDepths.TopDown.PLCharacterEvent"/> struct.
		/// </summary>
		/// <param name="character">Character.</param>
		/// <param name="eventType">Event type.</param>
		public PLCharacterEvent(Character character, PLCharacterEventTypes eventType)
		{
			TargetCharacter = character;
			EventType = eventType;
		}

		static PLCharacterEvent e;
		public static void Trigger(Character character, PLCharacterEventTypes eventType)
		{
			e.TargetCharacter = character;
			e.EventType = eventType;
			PLEventManager.TriggerEvent(e);
		}
	}
	
	public enum PLLifeCycleEventTypes { Death, Revive }

	public struct PLLifeCycleEvent
	{
		public Health AffectedHealth;
		public PLLifeCycleEventTypes PLLifeCycleEventTypes;
		
		public PLLifeCycleEvent(Health affectedHealth, PLLifeCycleEventTypes lifeCycleEventType)
		{
			AffectedHealth = affectedHealth;
			PLLifeCycleEventTypes = lifeCycleEventType;
		}

		static PLLifeCycleEvent e;
		public static void Trigger(Health affectedHealth, PLLifeCycleEventTypes lifeCycleEventType)
		{
			e.AffectedHealth = affectedHealth;
			e.PLLifeCycleEventTypes = lifeCycleEventType;
			PLEventManager.TriggerEvent(e);
		}
	}

	/// <summary>
	/// An event fired when something takes damage
	/// </summary>
	public struct PLDamageTakenEvent
	{
		public Health AffectedHealth;
		public GameObject Instigator;
		public float CurrentHealth;
		public float DamageCaused;
		public float PreviousHealth;

		/// <summary>
		/// Initializes a new instance of the <see cref="SpectralDepths.TopDown.PLDamageTakenEvent"/> struct.
		/// </summary>
		/// <param name="affectedHealth">Affected Health.</param>
		/// <param name="instigator">Instigator.</param>
		/// <param name="currentHealth">Current health.</param>
		/// <param name="damageCaused">Damage caused.</param>
		/// <param name="previousHealth">Previous health.</param>
		public PLDamageTakenEvent(Health affectedHealth, GameObject instigator, float currentHealth, float damageCaused, float previousHealth)
		{
			AffectedHealth = affectedHealth;
			Instigator = instigator;
			CurrentHealth = currentHealth;
			DamageCaused = damageCaused;
			PreviousHealth = previousHealth;
		}

		static PLDamageTakenEvent e;
		public static void Trigger(Health affectedHealth, GameObject instigator, float currentHealth, float damageCaused, float previousHealth)
		{
			e.AffectedHealth = affectedHealth;
			e.Instigator = instigator;
			e.CurrentHealth = currentHealth;
			e.DamageCaused = damageCaused;
			e.PreviousHealth = previousHealth;
			PLEventManager.TriggerEvent(e);
		}
	}
}