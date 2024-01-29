using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// Add this class to a character so it can use weapons
	/// Note that this component will trigger animations (if their parameter is present in the Animator), based on 
	/// the current weapon's Animations
	/// Animator parameters : defined from the Weapon's inspector
	/// </summary>
	[AddComponentMenu("Spectral Depths/Character/Abilities/Character Handle Secondary Weapon")]
	public class CharacterHandleSecondaryWeapon : CharacterHandleWeapon
	{
		/// the ID / index of this CharacterHandleWeapon. This will be used to determine what handle weapon ability should equip a weapon.
		/// If you create more Handle Weapon abilities, make sure to override and increment this  
		public override int HandleWeaponID { get { return 2; } }
        
		/// <summary>
		/// Gets input and triggers methods based on what's been pressed
		/// </summary>
		protected override void HandleInput()
		{
			if (!AbilityAuthorized
			    || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
			    || (CurrentWeapon == null))
			{
				return;
			}

			bool inputAuthorized = true;
			if (CurrentWeapon != null)
			{
				inputAuthorized = CurrentWeapon.InputAuthorized;
			}
			
			if (inputAuthorized && ((_inputManager.SecondaryShootButton.State.CurrentState == PLInput.ButtonStates.ButtonDown) || (_inputManager.SecondaryShootAxis == PLInput.ButtonStates.ButtonDown)))
			{
				ShootStart();
			}
			
			bool buttonPressed =
				(_inputManager.SecondaryShootButton.State.CurrentState == PLInput.ButtonStates.ButtonPressed) ||
				(_inputManager.SecondaryShootAxis == PLInput.ButtonStates.ButtonPressed); 
            
			if (inputAuthorized && ContinuousPress && (CurrentWeapon.TriggerMode == Weapon.TriggerModes.Auto) && buttonPressed)
			{
				ShootStart();
			}

			if (_inputManager.ReloadButton.State.CurrentState == PLInput.ButtonStates.ButtonDown)
			{
				Reload();
			}

			if (inputAuthorized && ((_inputManager.SecondaryShootButton.State.CurrentState == PLInput.ButtonStates.ButtonUp) || (_inputManager.SecondaryShootAxis == PLInput.ButtonStates.ButtonUp)))
			{
				ShootStop();
				CurrentWeapon.WeaponInputReleased();
			}
			
			if ((CurrentWeapon.WeaponState.CurrentState == Weapon.WeaponStates.WeaponDelayBetweenUses)
			    && ((_inputManager.SecondaryShootAxis == PLInput.ButtonStates.Off) && (_inputManager.SecondaryShootButton.State.CurrentState == PLInput.ButtonStates.Off))
			    && !(UseSecondaryAxisThresholdToShoot && (_inputManager.SecondaryMovement.magnitude > _inputManager.Threshold.magnitude)))
			{
				CurrentWeapon.WeaponInputStop();
			}

			if (inputAuthorized && UseSecondaryAxisThresholdToShoot && (_inputManager.SecondaryMovement.magnitude > _inputManager.Threshold.magnitude))
			{
				ShootStart();
			}
		}
	}
}