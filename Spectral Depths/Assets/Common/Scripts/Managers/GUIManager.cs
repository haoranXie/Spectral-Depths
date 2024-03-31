using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SpectralDepths.Tools;
using UnityEngine.EventSystems;
using SpectralDepths.Feedbacks;
using System.Collections.Generic;
namespace SpectralDepths.TopDown
{
	/// <summary>
	/// Handles all GUI effects and changes
	/// </summary>
	[AddComponentMenu("Spectral Depths/Managers/GUIManager")]
	public class GUIManager : PLSingleton<GUIManager>, PLEventListener<TopDownEngineEvent>
	{
		/// the main canvas
		[Tooltip("the main canvas")]
		public Canvas MainCanvas;
		[Tooltip("animator used for special UI animations")]
		public Animator AnimationPlayer;
		/// the game object that contains the heads up display (avatar, health, points...)
		[Tooltip("the game object that contains the heads up display (avatar, health, points...)")]
		public GameObject HUD;
		/// the health bars to update
		[Tooltip("the health bars to update")]
		public PLProgressBar[] HealthBars;
		/// the dash bars to update
		[Tooltip("the dash bars to update")]
		public PLRadialProgressBar[] DashBars;
		/// the panels and bars used to display current weapon ammo
		[Tooltip("the panels and bars used to display current weapon ammo")]
		public AmmoDisplay[] AmmoDisplays;
		/// the pause screen game object
		[Tooltip("the pause screen game object")]
		public GameObject PauseScreen;
		/// the death screen
		[Tooltip("the death screen")]
		public GameObject DeathScreen;
		/// The mobile buttons
		[Tooltip("The mobile buttons")]
		public CanvasGroup Buttons;
		/// The mobile arrows
		[Tooltip("The mobile arrows")]
		public CanvasGroup Arrows;
		/// The mobile movement joystick
		[Tooltip("The mobile movement joystick")]
		public CanvasGroup Joystick;
		/// the points counter
		[Tooltip("the points counter")]
		public Text PointsText;
		/// the pattern to apply to format the display of points
		[Tooltip("the pattern to apply to format the display of points")]
		public string PointsTextPattern = "000000";
		[Tooltip("List of gameobjects used for turtorial section")]
		public List<GameObject> TurtorialNotifications;

		bool _checkForClick = false;

		protected float _initialJoystickAlpha;
		protected float _initialButtonsAlpha;
		protected bool _initialized = false;

		/// <summary>
		/// Initialization
		/// </summary>
		protected override void Awake()
		{
			base.Awake();

			Initialization();
		}

		private void Update()
		{
			if(_checkForClick)
			{
				if (Input.GetMouseButtonDown(0))
				{
					AnimationPlayer.SetTrigger("TurtorialNotificationOff");
					_checkForClick = false;
				}
			}
		}

		protected virtual void Initialization()
		{
			if (_initialized)
			{
				return;
			}

			if (Joystick != null)
			{
				_initialJoystickAlpha = Joystick.alpha;
			}
			if (Buttons != null)
			{
				_initialButtonsAlpha = Buttons.alpha;
			}

			_initialized = true;
		}

		/// <summary>
		/// Initialization
		/// </summary>
		protected virtual void Start()
		{
			RefreshPoints();
			SetPauseScreen(false);
			SetDeathScreen(false);
		}

		/// <summary>
		/// Sets the HUD active or inactive
		/// </summary>
		/// <param name="state">If set to <c>true</c> turns the HUD active, turns it off otherwise.</param>
		public virtual void SetHUDActive(bool state)
		{
			if (HUD!= null)
			{ 
				HUD.SetActive(state);
			}
			if (PointsText!= null)
			{ 
				PointsText.enabled = state;
			}
		}

		/// <summary>
		/// Sets the avatar active or inactive
		/// </summary>
		/// <param name="state">If set to <c>true</c> turns the HUD active, turns it off otherwise.</param>
		public virtual void SetAvatarActive(bool state)
		{
			if (HUD != null)
			{
				HUD.SetActive(state);
			}
		}

		/// <summary>
		/// Called by the input manager, this method turns controls visible or not depending on what's been chosen
		/// </summary>
		/// <param name="state">If set to <c>true</c> state.</param>
		/// <param name="movementControl">Movement control.</param>
		public virtual void SetMobileControlsActive(bool state, InputManager.MovementControls movementControl = InputManager.MovementControls.Joystick)
		{
			Initialization();
            
			if (Joystick != null)
			{
				Joystick.gameObject.SetActive(state);
				if (state && movementControl == InputManager.MovementControls.Joystick)
				{
					Joystick.alpha=_initialJoystickAlpha;
				}
				else
				{
					Joystick.alpha=0;
					Joystick.gameObject.SetActive (false);
				}
			}

			if (Arrows != null)
			{
				Arrows.gameObject.SetActive(state);
				if (state && movementControl == InputManager.MovementControls.Arrows)
				{
					Arrows.alpha=_initialJoystickAlpha;
				}
				else
				{
					Arrows.alpha=0;
					Arrows.gameObject.SetActive (false);
				}
			}

			if (Buttons != null)
			{
				Buttons.gameObject.SetActive(state);
				if (state)
				{
					Buttons.alpha=_initialButtonsAlpha;
				}
				else
				{
					Buttons.alpha=0;
					Buttons.gameObject.SetActive (false);
				}
			}
		}

		/// <summary>
		/// Sets the pause screen on or off.
		/// </summary>
		/// <param name="state">If set to <c>true</c>, sets the pause.</param>
		public virtual void SetPauseScreen(bool state)
		{
			if (PauseScreen != null)
			{
				PauseScreen.SetActive(state);
				EventSystem.current.sendNavigationEvents = state;
			}
		}

		/// <summary>
		/// Sets the death screen on or off.
		/// </summary>
		/// <param name="state">If set to <c>true</c>, sets the pause.</param>
		public virtual void SetDeathScreen(bool state)
		{
			if (DeathScreen != null)
			{
				DeathScreen.SetActive(state);
				EventSystem.current.sendNavigationEvents = state;
			}
		}

		/// <summary>
		/// Sets the jetpackbar active or not.
		/// </summary>
		/// <param name="state">If set to <c>true</c>, sets the pause.</param>
		public virtual void SetDashBar(bool state, string CharacterID)
		{
			if (DashBars == null)
			{
				return;
			}

			foreach (PLRadialProgressBar jetpackBar in DashBars)
			{
				if (jetpackBar != null)
				{ 
					if (jetpackBar.CharacterID == CharacterID)
					{
						jetpackBar.gameObject.SetActive(state);
					}					
				}
			}	        
		}

		/// <summary>
		/// Sets the ammo displays active or not
		/// </summary>
		/// <param name="state">If set to <c>true</c> state.</param>
		/// <param name="CharacterID">Player I.</param>
		public virtual void SetAmmoDisplays(bool state, string CharacterID, int ammoDisplayID)
		{
			if (AmmoDisplays == null)
			{
				return;
			}

			foreach (AmmoDisplay ammoDisplay in AmmoDisplays)
			{
				if (ammoDisplay != null)
				{ 
					if ((ammoDisplay.CharacterID == CharacterID) && (ammoDisplayID == ammoDisplay.AmmoDisplayID))
					{
						ammoDisplay.gameObject.SetActive(state);
					}					
				}
			}
		}
        		
		/// <summary>
		/// Sets the text to the game manager's points.
		/// </summary>
		public virtual void RefreshPoints()
		{
			if (PointsText!= null)
			{ 
				PointsText.text = GameManager.Instance.Points.ToString(PointsTextPattern);
			}
		}

		/// <summary>
		/// Updates the health bar.
		/// </summary>
		/// <param name="currentHealth">Current health.</param>
		/// <param name="minHealth">Minimum health.</param>
		/// <param name="maxHealth">Max health.</param>
		/// <param name="CharacterID">Player I.</param>
		public virtual void UpdateHealthBar(float currentHealth,float minHealth,float maxHealth,string CharacterID)
		{
			if (HealthBars == null) { return; }
			if (HealthBars.Length <= 0)	{ return; }

			foreach (PLProgressBar healthBar in HealthBars)
			{
				if (healthBar == null) { continue; }
				if (healthBar.CharacterID == CharacterID)
				{
					healthBar.UpdateBar(currentHealth,minHealth,maxHealth);
				}
			}

		}

		/// <summary>
		/// Updates the dash bars.
		/// </summary>
		/// <param name="currentFuel">Current fuel.</param>
		/// <param name="minFuel">Minimum fuel.</param>
		/// <param name="maxFuel">Max fuel.</param>
		/// <param name="CharacterID">Player I.</param>
		public virtual void UpdateDashBars(float currentFuel, float minFuel, float maxFuel,string CharacterID)
		{
			if (DashBars == null)
			{
				return;
			}

			foreach (PLRadialProgressBar dashbar in DashBars)
			{
				if (dashbar == null) { return; }
				if (dashbar.CharacterID == CharacterID)
				{
					dashbar.UpdateBar(currentFuel,minFuel,maxFuel);	
				}    
			}
		}

		/// <summary>
		/// Updates the (optional) ammo displays.
		/// </summary>
		/// <param name="magazineBased">If set to <c>true</c> magazine based.</param>
		/// <param name="totalAmmo">Total ammo.</param>
		/// <param name="maxAmmo">Max ammo.</param>
		/// <param name="ammoInMagazine">Ammo in magazine.</param>
		/// <param name="magazineSize">Magazine size.</param>
		/// <param name="CharacterID">Player I.</param>
		/// <param name="displayTotal">If set to <c>true</c> display total.</param>
		public virtual void UpdateAmmoDisplays(bool magazineBased, int totalAmmo, int maxAmmo, int ammoInMagazine, int magazineSize, string CharacterID, int ammoDisplayID, bool displayTotal)
		{
			if (AmmoDisplays == null)
			{
				return;
			}

			foreach (AmmoDisplay ammoDisplay in AmmoDisplays)
			{
				if (ammoDisplay == null) { return; }
				if ((ammoDisplay.CharacterID == CharacterID) && (ammoDisplayID == ammoDisplay.AmmoDisplayID))
				{
					ammoDisplay.UpdateAmmoDisplays (magazineBased, totalAmmo, maxAmmo, ammoInMagazine, magazineSize, displayTotal);
				}    
			}
		}

		public virtual void OnMMEvent(TopDownEngineEvent engineEvent)
		{
			switch (engineEvent.EventType)
			{
				case TopDownEngineEventTypes.ActiveCinematicMode:
					AnimationPlayer.SetTrigger("CinematicOff");
					break;
				case TopDownEngineEventTypes.TurnOffCinematicMode:
					AnimationPlayer.SetTrigger("CinematicOn");
					break;
			}
		}
		public virtual void Pause()
		{	
			// if time is not already stopped		
			if (Time.timeScale>0.0f)
			{
				PLTimeScaleEvent.Trigger(PLTimeScaleMethods.For, 0f, 0f, false, 0f, true);
			}
			LevelManager.Instance.ToggleCharacterPause();
		}
		public virtual void UnPause()
		{	
			PLTimeScaleEvent.Trigger(PLTimeScaleMethods.Unfreeze, 1f, 0f, false, 0f, false);
			LevelManager.Instance.ToggleCharacterPause();
		}
		public virtual void HideAllTurtorialNotifications()
		{
			foreach(GameObject turtorialNotification in TurtorialNotifications)
			{
				turtorialNotification.gameObject.SetActive(false);
			}
		}
		public virtual void CheckForClick()
		{
			_checkForClick = true;
		}
		protected virtual void OnEnable()
		{
			this.PLEventStartListening<TopDownEngineEvent> ();
		}

		/// <summary>
		/// OnDisable, we stop listening to events.
		/// </summary>
		protected virtual void OnDisable()
		{
			this.PLEventStopListening<TopDownEngineEvent> ();
		}


	}
}