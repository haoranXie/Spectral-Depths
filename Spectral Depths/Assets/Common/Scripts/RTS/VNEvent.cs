using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Tools;

namespace SpectralDepths.TopDown
{

	public enum VNEventTypes
	{
		ChangeVNScene,
        DisableVNScene
	}
	public struct VNEvent
	{
        //Relevant amount of energy. For example, UseEnergy AmountOfEnergy subtracts energy while RecoverEnergy adds energy
        public TextAsset FileToRead;
		public bool PauseGame;
		public bool BlockInput;
		public VNEventTypes EventType;

        public VNEvent(VNEventTypes eventType, TextAsset fileToRead, bool pauseGame = false, bool blockInput = true)
		{
			EventType = eventType;
			FileToRead = fileToRead;
            PauseGame = pauseGame;
			BlockInput = blockInput;
		}

		static VNEvent e;
        public static void Trigger(VNEventTypes eventType, TextAsset fileToRead, bool pauseGame = false, bool blockInput = true)
		{
			e.EventType = eventType;
			e.FileToRead = fileToRead;
            e.PauseGame = pauseGame;
			e.BlockInput = blockInput;
            PLEventManager.TriggerEvent(e);
		}
	}
}