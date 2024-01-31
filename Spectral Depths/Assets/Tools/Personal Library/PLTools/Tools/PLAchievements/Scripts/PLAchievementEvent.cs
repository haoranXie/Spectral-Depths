using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// An event type used to broadcast the fact that an achievement has been unlocked
	/// </summary>
	public struct PLAchievementUnlockedEvent
	{
		/// the achievement that has been unlocked
		public PLAchievement Achievement;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="newAchievement">New achievement.</param>
		public PLAchievementUnlockedEvent(PLAchievement newAchievement)
		{
			Achievement = newAchievement;
		}

		static PLAchievementUnlockedEvent e;
		public static void Trigger(PLAchievement newAchievement)
		{
			e.Achievement = newAchievement;
			PLEventManager.TriggerEvent(e);
		}
	}
	
	public struct PLAchievementChangedEvent
	{
		/// the achievement that has been unlocked
		public PLAchievement Achievement;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="newAchievement">New achievement.</param>
		public PLAchievementChangedEvent(PLAchievement newAchievement)
		{
			Achievement = newAchievement;
		}

		static PLAchievementChangedEvent e;
		public static void Trigger(PLAchievement newAchievement)
		{
			e.Achievement = newAchievement;
			PLEventManager.TriggerEvent(e);
		}
	}
}