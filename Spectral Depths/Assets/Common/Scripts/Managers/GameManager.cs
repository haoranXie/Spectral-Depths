using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using System.Collections.Generic;
using SpectralDepths.InventoryEngine;
using SpectralDepths.Feedbacks;
using UnityEngine.Events;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// A list of the possible Spectral Depths base events
	/// LevelStart : triggered by the LevelManager when a level starts
	///	LevelComplete : can be triggered when the end of a level is reached
	/// LevelEnd : same thing
	///	Pause : triggered when a pause is starting
	///	UnPause : triggered when a pause is ending and going back to normal
	///	PlayerDeath : triggered when the player character dies
	///	RespawnStarted : triggered when the player character respawn sequence starts
	///	RespawnComplete : triggered when the player character respawn sequence ends
	///	StarPicked : triggered when a star bonus gets picked
	///	GameOver : triggered by the LevelManager when all lives are lost
	/// CharacterSwap : triggered when the character gets swapped
	/// CharacterSwitch : triggered when the character gets switched
	/// Repaint : triggered to ask for a UI refresh
	/// TogglePause : triggered to request a pause (or unpause)
	/// </summary>
	public enum TopDownEngineEventTypes
	{
		SpawnCharacterStarts,
		LevelStart,
		LevelComplete,
		LevelEnd,
		Pause,
		UnPause,
		PlayerDeath,
		SpawnComplete,
		RespawnStarted,
		RespawnComplete,
		StarPicked,
		GameOver,
		CharacterSwap,
		CharacterSwitch,
		Repaint,
		TogglePause,
		SelectionChanged,
		RTSOff,
		RTSOn,
		LoadNextScene
	}

	/// <summary>
	/// A type of events used to signal level start and end (for now)
	/// </summary>
	public struct TopDownEngineEvent
	{
		public TopDownEngineEventTypes EventType;
		public Character OriginCharacter;

		/// <summary>
		/// Initializes a new instance of the <see cref="SpectralDepths.TopDown.TopDownEngineEvent"/> struct.
		/// </summary>
		/// <param name="eventType">Event type.</param>
		public TopDownEngineEvent(TopDownEngineEventTypes eventType, Character originCharacter)
		{
			EventType = eventType;
			OriginCharacter = originCharacter;
		}

		static TopDownEngineEvent e;
		public static void Trigger(TopDownEngineEventTypes eventType, Character originCharacter)
		{
			e.EventType = eventType;
			e.OriginCharacter = originCharacter;
			PLEventManager.TriggerEvent(e);
		}
	} 

	/// <summary>
	/// A list of the methods available to change the current score
	/// </summary>
	public enum PointsMethods
	{
		Add,
		Set
	}

	/// <summary>
	/// A type of event used to signal changes to the current score
	/// </summary>
	public struct TopDownEnginePointEvent
	{
		public PointsMethods PointsMethod;
		public int Points;
		/// <summary>
		/// Initializes a new instance of the <see cref="SpectralDepths.TopDown.TopDownEnginePointEvent"/> struct.
		/// </summary>
		/// <param name="pointsMethod">Points method.</param>
		/// <param name="points">Points.</param>
		public TopDownEnginePointEvent(PointsMethods pointsMethod, int points)
		{
			PointsMethod = pointsMethod;
			Points = points;
		}

		static TopDownEnginePointEvent e;
		public static void Trigger(PointsMethods pointsMethod, int points)
		{
			e.PointsMethod = pointsMethod;
			e.Points = points;
			PLEventManager.TriggerEvent(e);
		}
	}

	/// <summary>
	/// A list of the possible pause methods
	/// </summary>
	public enum PauseMethods
	{
		PauseMenu,
		NoPauseMenu
	}

	/// <summary>
	/// A class to store points of entry into levels, one per level.
	/// </summary>
	public class PointsOfEntryStorage
	{
		public string LevelName;
		public int PointOfEntryIndex;
		public Character.FacingDirections FacingDirection;

		public PointsOfEntryStorage(string levelName, int pointOfEntryIndex, Character.FacingDirections facingDirection)
		{
			LevelName = levelName;
			FacingDirection = facingDirection;
			PointOfEntryIndex = pointOfEntryIndex;
		}
	}

	/// <summary>
	/// The game manager is a persistent singleton that handles points and time
	/// </summary>
	[AddComponentMenu("Spectral Depths/Managers/Game Manager")]
	public class GameManager : 	PLPersistentSingleton<GameManager>, 
		PLEventListener<PLGameEvent>, 
		PLEventListener<TopDownEngineEvent>, 
		PLEventListener<TopDownEnginePointEvent>
	{
		/// the target frame rate for the game
		[Tooltip("the target frame rate for the game")]
		public int TargetFrameRate = 300;
		[Header("Lives")]
		/// the maximum amount of lives the character can currently have
		[Tooltip("the maximum amount of lives the character can currently have")]
		public int MaximumLives = 0;
		/// the current number of lives 
		[Tooltip("the current number of lives ")]
		public int CurrentLives = 0;

		[Header("Bindings")]
		/// the name of the scene to redirect to when all lives are lost
		[Tooltip("the name of the scene to redirect to when all lives are lost")]
		public string GameOverScene;

		[Header("Points")]
		/// the current number of game points
		[PLReadOnly]
		[Tooltip("the current number of game points")]
		public int Points;

		[Header("Pause")]
		/// if this is true, the game will automatically pause when opening an inventory
		[Tooltip("if this is true, the game will automatically pause when opening an inventory")]
		public bool PauseGameWhenInventoryOpens = true;
		/// if this is true, the game will pause when player clicks pause button
		[Tooltip("if this is true, the game will pause when player clicks pause button")]
		public bool QuickPause = true;
		/// true if the game is currently paused	
		/// whether or not to mute the sfx track when the game pauses, and to unmute it when it unpauses 
		[Tooltip("whether or not to mute the sfx track when the game pauses, and to unmute it when it unpauses")]
		public bool MuteSfxTrackSounds = true;
		/// whether or not to mute the UI track when the game pauses, and to unmute it when it unpauses 
		[Tooltip("whether or not to mute the UI track when the game pauses, and to unmute it when it unpauses")]
		public bool MuteUITrackSounds = false;
		/// whether or not to mute the music track when the game pauses, and to unmute it when it unpauses 
		[Tooltip("whether or not to mute the music track when the game pauses, and to unmute it when it unpauses")]
		public bool MuteMusicTrackSounds = false;
		/// whether or not to mute the master track when the game pauses, and to unmute it when it unpauses 
		[Tooltip("whether or not to mute the master track when the game pauses, and to unmute it when it unpauses")]
		public bool MuteMasterTrackSounds = false;

		[Header("Hooks")] 
		/// a UnityEvent that will trigger when the game pauses 
		[Tooltip("a UnityEvent that will trigger when the game pauses")]
		public UnityEvent OnPause;
		/// a UnityEvent that will trigger when the game unpauses
		[Tooltip("a UnityEvent that will trigger when the game unpauses")]
		public UnityEvent OnUnpause;	
		public bool Paused { get; set; } 
		// true if we've stored a map position at least once
		public bool StoredLevelMapPosition{ get; set; }
		/// the current player
		public Vector2 LevelMapPosition { get; set; }
		/// the stored selected character
		public Character PersistentCharacter { get; set; }
		/// the list of points of entry and exit
		[Tooltip("the list of points of entry and exit")]
		public List<PointsOfEntryStorage> PointsOfEntry;
		/// the stored selected character
		public Character StoredCharacter { get; set; }

		// storage
		protected bool _inventoryOpen = false;
		protected bool _pauseMenuOpen = false;
		protected InventoryInputManager _inventoryInputManager;
		protected int _initialMaximumLives;
		protected int _initialCurrentLives;


		/// <summary>
		/// On Awake we initialize our list of points of entry and get InputManager
		/// </summary>
		protected override void Awake()
		{
			base.Awake ();
			PointsOfEntry = new List<PointsOfEntryStorage> ();
		}

		/// <summary>
		/// On Start(), sets the target framerate to whatever's been specified
		/// </summary>
		protected virtual void Start()
		{
			Application.targetFrameRate = TargetFrameRate;
			_initialCurrentLives = CurrentLives;
			_initialMaximumLives = MaximumLives;
		}
		/// <summary>
		/// Update to detect input for pausing
		/// </summary>
		protected virtual void Update()
		{
			if(QuickPause)
			{
				InputPause();
			}
		}
					
		/// <summary>
		/// this method resets the whole game manager
		/// </summary>
		public virtual void Reset()
		{
			Points = 0;
			PLTimeScaleEvent.Trigger(PLTimeScaleMethods.Reset, 1f, 0f, false, 0f, true);
			Paused = false;
		}
		/// <summary>
		/// Use this method to decrease the current number of lives
		/// </summary>
		public virtual void LoseLife()
		{
			CurrentLives--;
		}

		/// <summary>
		/// Use this method when a life (or more) is gained
		/// </summary>
		/// <param name="lives">Lives.</param>
		public virtual void GainLives(int lives)
		{
			CurrentLives += lives;
			if (CurrentLives > MaximumLives)
			{
				CurrentLives = MaximumLives;
			}
		}

		/// <summary>
		/// Use this method to increase the max amount of lives, and optionnally the current amount as well
		/// </summary>
		/// <param name="lives">Lives.</param>
		/// <param name="increaseCurrent">If set to <c>true</c> increase current.</param>
		public virtual void AddLives(int lives, bool increaseCurrent)
		{
			MaximumLives += lives;
			if (increaseCurrent)
			{
				CurrentLives += lives;
			}
		}

		/// <summary>
		/// Resets the number of lives to their initial values.
		/// </summary>
		public virtual void ResetLives()
		{
			CurrentLives = _initialCurrentLives;
			MaximumLives = _initialMaximumLives;
		}

		/// <summary>
		/// Adds the points in parameters to the current game points.
		/// </summary>
		/// <param name="pointsToAdd">Points to add.</param>
		public virtual void AddPoints(int pointsToAdd)
		{
			Points += pointsToAdd;
			GUIManager.Instance.RefreshPoints();
		}
		
		/// <summary>
		/// use this to set the current points to the one you pass as a parameter
		/// </summary>
		/// <param name="points">Points.</param>
		public virtual void SetPoints(int points)
		{
			Points = points;
			GUIManager.Instance.RefreshPoints();
		}
		
		/// <summary>
		/// Enables the inventory input manager if found
		/// </summary>
		/// <param name="status"></param>
		protected virtual void SetActiveInventoryInputManager(bool status)
		{
			_inventoryInputManager = GameObject.FindObjectOfType<InventoryInputManager> ();
			if (_inventoryInputManager != null)
			{
				_inventoryInputManager.enabled = status;
			}
		}

		/// <summary>
		/// Pauses the game or unpauses it depending on the current state
		/// </summary>
		public virtual void Pause(PauseMethods pauseMethod = PauseMethods.PauseMenu, bool unpauseIfPaused = true)
		{	
			if ((pauseMethod == PauseMethods.PauseMenu) && _inventoryOpen)
			{
				return;
			}

			// if time is not already stopped		
			if (Time.timeScale>0.0f)
			{
				PLTimeScaleEvent.Trigger(PLTimeScaleMethods.For, 0f, 0f, false, 0f, true);
				Instance.Paused=true;
				if ((GUIManager.HasInstance) && (pauseMethod == PauseMethods.PauseMenu))
				{
					GUIManager.Instance.SetPauseScreen(true);	
					_pauseMenuOpen = true;
					SetActiveInventoryInputManager (false);
				}
				if (pauseMethod == PauseMethods.NoPauseMenu)
				{
					_inventoryOpen = true;
				}
			}
			else
			{
				if (unpauseIfPaused)
				{
					UnPause(pauseMethod);	
				}
			}		
			MuteSound();
			LevelManager.Instance.ToggleCharacterPause();
		}
        
		/// <summary>
		/// Unpauses the game
		/// </summary>
		public virtual void UnPause(PauseMethods pauseMethod = PauseMethods.PauseMenu)
		{
			PLTimeScaleEvent.Trigger(PLTimeScaleMethods.Unfreeze, 1f, 0f, false, 0f, false);
			Instance.Paused = false;
			if ((GUIManager.HasInstance) && (pauseMethod == PauseMethods.PauseMenu))
			{ 
				GUIManager.Instance.SetPauseScreen(false);
				_pauseMenuOpen = false;
				SetActiveInventoryInputManager (true);
			}
			if (_inventoryOpen)
			{
				_inventoryOpen = false;
			}
			UnMuteSound();
			LevelManager.Instance.ToggleCharacterPause();
		}
		/// <summary>
		/// Pauses game when player presses pause button
		/// </summary>
		public virtual void InputPause()
		{
			if (InputManager.Instance.PauseButton.State.CurrentState == PLInput.ButtonStates.ButtonDown )
			{
				TopDownEngineEvent.Trigger(TopDownEngineEventTypes.TogglePause, null);
			}
		}

		/// <summary>
		/// Mutes Sound based on parameters
		/// </summary>
		public virtual void MuteSound()
		{
			OnPause?.Invoke();

			if (MuteSfxTrackSounds) { PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.MuteTrack, PLSoundManager.PLSoundManagerTracks.Sfx); }
			if (MuteUITrackSounds) { PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.MuteTrack, PLSoundManager.PLSoundManagerTracks.UI); }
			if (MuteMusicTrackSounds) { PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.MuteTrack, PLSoundManager.PLSoundManagerTracks.Music); }
			if (MuteMasterTrackSounds) { PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.MuteTrack, PLSoundManager.PLSoundManagerTracks.Master); }		
		}
		/// <summary>
		/// Unmutes Sound based on parameters
		/// </summary>
		public virtual void UnMuteSound()
		{
			OnUnpause?.Invoke();

			if (MuteSfxTrackSounds) { PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.UnmuteTrack, PLSoundManager.PLSoundManagerTracks.Sfx); }
			if (MuteUITrackSounds) { PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.UnmuteTrack, PLSoundManager.PLSoundManagerTracks.UI); }
			if (MuteMusicTrackSounds) { PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.UnmuteTrack, PLSoundManager.PLSoundManagerTracks.Music); }
			if (MuteMasterTrackSounds) { PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.UnmuteTrack, PLSoundManager.PLSoundManagerTracks.Master); }
		}
        
		/// <summary>
		/// Stores the points of entry for the level whose name you pass as a parameter.
		/// </summary>
		/// <param name="levelName">Level name.</param>
		/// <param name="entryIndex">Entry index.</param>
		/// <param name="exitIndex">Exit index.</param>
		public virtual void StorePointsOfEntry(string levelName, int entryIndex, Character.FacingDirections facingDirection)
		{
			if (PointsOfEntry.Count > 0)
			{
				foreach (PointsOfEntryStorage point in PointsOfEntry)
				{
					if (point.LevelName == levelName)
					{
						point.PointOfEntryIndex = entryIndex;
						return;
					}
				}	
			}

			PointsOfEntry.Add (new PointsOfEntryStorage (levelName, entryIndex, facingDirection));
		}

		/// <summary>
		/// Gets point of entry info for the level whose scene name you pass as a parameter
		/// </summary>
		/// <returns>The points of entry.</returns>
		/// <param name="levelName">Level name.</param>
		public virtual PointsOfEntryStorage GetPointsOfEntry(string levelName)
		{
			if (PointsOfEntry.Count > 0)
			{
				foreach (PointsOfEntryStorage point in PointsOfEntry)
				{
					if (point.LevelName == levelName)
					{
						return point;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Clears the stored point of entry infos for the level whose name you pass as a parameter
		/// </summary>
		/// <param name="levelName">Level name.</param>
		public virtual void ClearPointOfEntry(string levelName)
		{
			if (PointsOfEntry.Count > 0)
			{
				foreach (PointsOfEntryStorage point in PointsOfEntry)
				{
					if (point.LevelName == levelName)
					{
						PointsOfEntry.Remove (point);
					}
				}
			}
		}

		/// <summary>
		/// Clears all points of entry.
		/// </summary>
		public virtual void ClearAllPointsOfEntry()
		{
			PointsOfEntry.Clear ();
		}

		/// <summary>
		/// Deletes all save files
		/// </summary>
		public virtual void ResetAllSaves()
		{
			PLSaveLoadManager.DeleteSaveFolder("InventoryEngine");
			PLSaveLoadManager.DeleteSaveFolder("TopDownEngine");
			PLSaveLoadManager.DeleteSaveFolder("PLAchievements");
		}

		/// <summary>
		/// Stores the selected character for use in upcoming levels
		/// </summary>
		/// <param name="selectedCharacter">Selected character.</param>
		public virtual void StoreSelectedCharacter(Character selectedCharacter)
		{
			StoredCharacter = selectedCharacter;
		}

		/// <summary>
		/// Clears the selected character.
		/// </summary>
		public virtual void ClearSelectedCharacter()
		{
			StoredCharacter = null;
		}
		
		/// <summary>
		/// Sets a new persistent character
		/// </summary>
		/// <param name="newCharacter"></param>
		public virtual void SetPersistentCharacter(Character newCharacter)
		{
			PersistentCharacter = newCharacter;
		}
		
		/// <summary>
		/// Destroys a persistent character if there's one
		/// </summary>
		public virtual void DestroyPersistentCharacter()
		{
			if (PersistentCharacter != null)
			{
				Destroy(PersistentCharacter.gameObject);
				SetPersistentCharacter(null);
			}
			

			if (LevelManager.Instance.Players[0] != null)
			{
				if (LevelManager.Instance.Players[0].gameObject.PLGetComponentNoAlloc<CharacterPersistence>() != null)
				{
					Destroy(LevelManager.Instance.Players[0].gameObject);	
				}
			}
		}

		/// <summary>
		/// Catches PLGameEvents and acts on them, playing the corresponding sounds
		/// </summary>
		/// <param name="gameEvent">PLGameEvent event.</param>
		public virtual void OnMMEvent(PLGameEvent gameEvent)
		{
			switch (gameEvent.EventName)
			{
				case "inventoryOpens":
					if (PauseGameWhenInventoryOpens)
					{
						Pause(PauseMethods.NoPauseMenu, false);
					}					
					break;

				case "inventoryCloses":
					if (PauseGameWhenInventoryOpens)
					{
						UnPause(PauseMethods.NoPauseMenu);
					}
					break;
			}
		}

		/// <summary>
		/// Catches TopDownEngineEvents and acts on them, playing the corresponding sounds
		/// </summary>
		/// <param name="engineEvent">TopDownEngineEvent event.</param>
		public virtual void OnMMEvent(TopDownEngineEvent engineEvent)
		{
			switch (engineEvent.EventType)
			{
				case TopDownEngineEventTypes.TogglePause:
					if (Paused)
					{
						TopDownEngineEvent.Trigger(TopDownEngineEventTypes.UnPause, null);
					}
					else
					{
						TopDownEngineEvent.Trigger(TopDownEngineEventTypes.Pause, null);
					}
					break;
				case TopDownEngineEventTypes.Pause:
					Pause ();
					break;

				case TopDownEngineEventTypes.UnPause:
					UnPause ();
					break;
			}
		}

		/// <summary>
		/// Catches TopDownEnginePointsEvents and acts on them, playing the corresponding sounds
		/// </summary>
		/// <param name="pointEvent">TopDownEnginePointEvent event.</param>
		public virtual void OnMMEvent(TopDownEnginePointEvent pointEvent)
		{
			switch (pointEvent.PointsMethod)
			{
				case PointsMethods.Set:
					SetPoints(pointEvent.Points);
					break;

				case PointsMethods.Add:
					AddPoints(pointEvent.Points);
					break;
			}
		}

		/// <summary>
		/// OnDisable, we start listening to events.
		/// </summary>
		protected virtual void OnEnable()
		{
			this.PLEventStartListening<PLGameEvent> ();
			this.PLEventStartListening<TopDownEngineEvent> ();
			this.PLEventStartListening<TopDownEnginePointEvent> ();
		}

		/// <summary>
		/// OnDisable, we stop listening to events.
		/// </summary>
		protected virtual void OnDisable()
		{
			this.PLEventStopListening<PLGameEvent> ();
			this.PLEventStopListening<TopDownEngineEvent> ();
			this.PLEventStopListening<TopDownEnginePointEvent> ();
		}
	}
}