using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Tools;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// PlayerSelected: triggered by RTSManager whenever player selects new character
	/// SelectionDisabled: triggered by CharacterSelectable when disabled
	/// CommandForceMove: triggered by RTSManager whenever a force move command is issued
	/// FinalWaypoint
	/// </summary>
	public enum RTSEventTypes
	{
		PlayerSelected,
		SelectionDisabled,
		SwitchToRTS,
		SwitchToPlayer,
		CommandForceMove,
		CommandForceAttack,
		FinalWaypointReached,
		UnselectedEveryone
	}
	public struct RTSEvent
	{
        public Dictionary<int, Character> SelectedTable;
		public Character OriginCharacter;
		public RTSEventTypes EventType;


        public RTSEvent(RTSEventTypes eventType, Character originCharacter, Dictionary<int, Character> selectedTable)
		{
			EventType = eventType;
            SelectedTable = selectedTable;
			OriginCharacter=originCharacter;
		}

		static RTSEvent e;
        public static void Trigger(RTSEventTypes eventType, Character originCharacter, Dictionary<int, Character> selectedTable)
		{
			e.EventType = eventType;
            e.SelectedTable = selectedTable;
			e.OriginCharacter=originCharacter;
            PLEventManager.TriggerEvent(e);
		}
	}
}