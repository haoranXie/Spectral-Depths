using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using UnityEngine.Events;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// Add this component to a character and it'll be able to activate/desactivate the pause
	/// </summary>
	[PLHiddenProperties("AbilityStopFeedbacks")]
	[AddComponentMenu("Spectral Depths/Character/Abilities/Character Pause")]
	public class CharacterPause : CharacterAbility
	{
		/// This method is only used to display a helpbox text at the beginning of the ability's inspector
		public override string HelpBoxText() { return "Allows this character (and the player controlling it) to press the pause button to pause the game."; }
		/*
		[Header("Pause audio tracks")]
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
		*/
		[Header("Hooks")] 
		/// a UnityEvent that will trigger when the game pauses 
		[Tooltip("a UnityEvent that will trigger when the game pauses")]
		public UnityEvent OnPause;
		/// a UnityEvent that will trigger when the game unpauses
		[Tooltip("a UnityEvent that will trigger when the game unpauses")]
		public UnityEvent OnUnpause;


		/// <summary>
		/// Every frame, we check the input to see if we need to pause/unpause the game
		/// </summary>
		protected override void HandleInput()
		{
			if (_inputManager.PauseButton.State.CurrentState == PLInput.ButtonStates.ButtonDown && !GameManager.Instance.QuickPause)
			{
				TriggerPause();
			}
		}

		/// <summary>
		/// If the pause button has been pressed, we change the pause state
		/// </summary>
		protected virtual void TriggerPause()
		{
			if (_condition.CurrentState == CharacterStates.CharacterConditions.Dead)
			{
				return;
			}
			if (!AbilityAuthorized)
			{
				return;
			}
			PlayAbilityStartFeedbacks();
			// we trigger a Pause event for the GameManager and other classes that could be listening to it too
			TopDownEngineEvent.Trigger(TopDownEngineEventTypes.TogglePause, null);
		}

		/// <summary>
		/// Puts the character in the pause state
		/// </summary>
		public virtual void PauseCharacter()
		{
			if (!this.enabled)
			{
				return;
			}
			_condition.ChangeState(CharacterStates.CharacterConditions.Paused);
			OnPause?.Invoke();
			/*
			if(!GameManager.Instance.QuickPause)
			{

				if (MuteSfxTrackSounds) { PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.MuteTrack, PLSoundManager.PLSoundManagerTracks.Sfx); }
				if (MuteUITrackSounds) { PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.MuteTrack, PLSoundManager.PLSoundManagerTracks.UI); }
				if (MuteMusicTrackSounds) { PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.MuteTrack, PLSoundManager.PLSoundManagerTracks.Music); }
				if (MuteMasterTrackSounds) { PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.MuteTrack, PLSoundManager.PLSoundManagerTracks.Master); }
			}
			*/
		}

		/// <summary>
		/// Restores the character to the state it was in before the pause.
		/// </summary>
		public virtual void UnPauseCharacter()
		{
			if (!this.enabled)
			{
				return;
			}
			_condition.RestorePreviousState();
			OnUnpause?.Invoke();
			/*
			if(!GameManager.Instance.QuickPause)
			{
				OnUnpause?.Invoke();

				if (MuteSfxTrackSounds) { PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.UnmuteTrack, PLSoundManager.PLSoundManagerTracks.Sfx); }
				if (MuteUITrackSounds) { PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.UnmuteTrack, PLSoundManager.PLSoundManagerTracks.UI); }
				if (MuteMusicTrackSounds) { PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.UnmuteTrack, PLSoundManager.PLSoundManagerTracks.Music); }
				if (MuteMasterTrackSounds) { PLSoundManagerTrackEvent.Trigger(PLSoundManagerTrackEventTypes.UnmuteTrack, PLSoundManager.PLSoundManagerTracks.Master); }
			}
			*/

		}
	}
}