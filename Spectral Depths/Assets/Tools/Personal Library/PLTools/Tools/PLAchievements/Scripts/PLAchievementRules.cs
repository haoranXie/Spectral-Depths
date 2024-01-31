using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// That class is meant to be extended to implement the achievement rules specific to your game.
	/// </summary>
	public abstract class PLAchievementRules : MonoBehaviour, PLEventListener<PLGameEvent>
	{
		public PLAchievementList AchievementList;
		[PLInspectorButton("PrintCurrentStatus")]
		public bool PrintCurrentStatusBtn;

		public virtual void PrintCurrentStatus()
		{
			foreach (PLAchievement achievement in PLAchievementManager.AchievementsList)
			{
				string status = achievement.UnlockedStatus ? "unlocked" : "locked";
				Debug.Log("["+achievement.AchievementID + "] "+achievement.Title+", status : "+status+", progress : "+achievement.ProgressCurrent+"/"+achievement.ProgressTarget);
			}	
		}
		
		/// <summary>
		/// On Awake, loads the achievement list and the saved file
		/// </summary>
		protected virtual void Awake()
		{
			// we load the list of achievements, stored in a ScriptableObject in our Resources folder.
			PLAchievementManager.LoadAchievementList (AchievementList);
			// we load our saved file, to update that list with the saved values.
			PLAchievementManager.LoadSavedAchievements ();
		}

		/// <summary>
		/// On enable, we start listening for PLGameEvents. You may want to extend that to listen to other types of events.
		/// </summary>
		protected virtual void OnEnable()
		{
			this.PLEventStartListening<PLGameEvent>();
		}

		/// <summary>
		/// On disable, we stop listening for PLGameEvents. You may want to extend that to stop listening to other types of events.
		/// </summary>
		protected virtual void OnDisable()
		{
			this.PLEventStopListening<PLGameEvent>();
		}

		/// <summary>
		/// When we catch an PLGameEvent, we do stuff based on its name
		/// </summary>
		/// <param name="gameEvent">Game event.</param>
		public virtual void OnMMEvent(PLGameEvent gameEvent)
		{
			switch (gameEvent.EventName)
			{
				case "Save":
					PLAchievementManager.SaveAchievements ();
					break;
				/*
				// These are just examples of how you could catch a GameStart PLGameEvent and trigger the potential unlock of a corresponding achievement 
				case "GameStart":
					PLAchievementManager.UnlockAchievement("theFirestarter");
					break;
				case "LifeLost":
					PLAchievementManager.UnlockAchievement("theEndOfEverything");
					break;
				case "Pause":
					PLAchievementManager.UnlockAchievement("timeStop");
					break;
				case "Jump":
					PLAchievementManager.UnlockAchievement ("aSmallStepForMan");
					PLAchievementManager.AddProgress ("toInfinityAndBeyond", 1);
					break;*/
			}
		} 
	}
}