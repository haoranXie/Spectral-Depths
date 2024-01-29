using SpectralDepths.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.TopDown
{
	public enum PLCameraEventTypes { SetTargetCharacter, SetConfiner, StartFollowing, StopFollowing, RefreshPosition, ResetPriorities, RefreshAutoFocus }

	public struct PLCameraEvent
	{
		public PLCameraEventTypes EventType;
		public Character TargetCharacter;
		public Collider Bounds;

		public PLCameraEvent(PLCameraEventTypes eventType, Character targetCharacter = null, Collider bounds = null)
		{
			EventType = eventType;
			TargetCharacter = targetCharacter;
			Bounds = bounds;
		}

		static PLCameraEvent e;
		public static void Trigger(PLCameraEventTypes eventType, Character targetCharacter = null, Collider bounds = null)
		{
			e.EventType = eventType;
			e.Bounds = bounds;
			e.TargetCharacter = targetCharacter;
			PLEventManager.TriggerEvent(e);
		}
	}
}