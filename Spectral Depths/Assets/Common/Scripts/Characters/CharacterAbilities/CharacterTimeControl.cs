using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Tools;
using SpectralDepths.Feedbacks;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// Add this ability to a character, and it'll be able to control time when pressing the TimeControl button
	/// </summary>
	[AddComponentMenu("Spectral Depths/Character/Abilities/Character Time Control")]
	public class CharacterTimeControl : CharacterAbility
	{
		public enum Modes { OneTime, Continuous }
        
		/// the chosen mode for this ability : one time will stop time for the specified duration on button press, even if you release it, while continuous will stop time while the button is pressed, until cooldown consumption duration expiration
		[Tooltip("the chosen mode for this ability : one time will stop time for the specified duration on button press, even if you release it, while continuous will stop time while the button is pressed, until cooldown consumption duration expiration")]
		public Modes Mode = Modes.Continuous;
		/// the time scale to switch to when the time control button gets pressed
		[Tooltip("the time scale to switch to when the time control button gets pressed")]
		public float TimeScale = 0.5f;
		/// the duration for which to keep the timescale changed
		[Tooltip("the duration for which to keep the timescale changed")]
		[PLEnumCondition("Mode", (int)Modes.OneTime)]
		public float OneTimeDuration = 1f;
		/// whether or not the timescale should get lerped
		[Tooltip("whether or not the timescale should get lerped")]
		public bool LerpTimeScale = true;
		/// the speed at which to lerp the timescale
		[Tooltip("the speed at which to lerp the timescale")]
		public float LerpSpeed = 5f;
		/// the cooldown for this ability
		[Tooltip("the cooldown for this ability")]
		public PLCooldown Cooldown;

		protected bool _timeControlled = false;

		/// <summary>
		/// Watches for input press
		/// </summary>
		protected override void HandleInput()
		{
			base.HandleInput();
			if (!AbilityAuthorized)
			{
				return;
			}
			if (_inputManager.TimeControlButton.State.CurrentState == PLInput.ButtonStates.ButtonDown)
			{
				TimeControlStart();
			}
			if (_inputManager.TimeControlButton.State.CurrentState == PLInput.ButtonStates.ButtonUp)
			{
				TimeControlStop();
			}
		}

		/// <summary>
		/// On initialization, we init our cooldown
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			Cooldown.Initialization();
		}

		/// <summary>
		/// Starts the time scale modification
		/// </summary>
		public virtual void TimeControlStart()
		{
			if (Cooldown.Ready())
			{
				PlayAbilityStartFeedbacks();
				if (Mode == Modes.Continuous)
				{
					PLTimeScaleEvent.Trigger(PLTimeScaleMethods.For, TimeScale, Cooldown.ConsumptionDuration, LerpTimeScale, LerpSpeed, true);
					Cooldown.Start();
					_timeControlled = true;    
				}
				else
				{
					PLTimeScaleEvent.Trigger(PLTimeScaleMethods.For, TimeScale, OneTimeDuration, LerpTimeScale, LerpSpeed, false);
					Cooldown.Start();
				}
			}            
		}

		/// <summary>
		/// Stops the time control
		/// </summary>
		public virtual void TimeControlStop()
		{
			Cooldown.Stop();
		}

		/// <summary>
		/// On update, we unfreeze time if needed
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();
			Cooldown.Update();

			if ((Cooldown.CooldownState != PLCooldown.CooldownStates.Consuming) && _timeControlled)
			{
				if (Mode == Modes.Continuous)
				{
					_timeControlled = false;
					PLTimeScaleEvent.Trigger(PLTimeScaleMethods.Unfreeze, 1f, 0f, false, 0f, false);    
				}
			}
		}

		protected virtual void OnCooldownStateChange(PLCooldown.CooldownStates newState)
		{
			if (newState == PLCooldown.CooldownStates.Stopped)
			{
				StopStartFeedbacks();
				PlayAbilityStopFeedbacks();
			}
		}
		
		protected override void OnEnable()
		{
			base.OnEnable();
			Cooldown.OnStateChange += OnCooldownStateChange;
		}
		
		protected override void OnDisable()
		{
			base.OnDisable();
			Cooldown.OnStateChange -= OnCooldownStateChange;
		}
	}
}