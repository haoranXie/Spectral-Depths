using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using System.Collections.Generic;
using SpectralDepths.Feedbacks;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// Add this component to a character and it'll make your character fall down holes in 2D
	/// </summary>
	[PLHiddenProperties("AbilityStartFeedbacks")]
	//[RequireComponent(typeof(TopDownController2D))]
	[AddComponentMenu("Spectral Depths/Character/Abilities/Character Fall Down Holes 2D")]
	public class CharacterFallDownHoles2D : CharacterAbility
	{
		/// the feedback to play when falling
		[Tooltip("the feedback to play when falling")]
		public PLFeedbacks FallingFeedback;

		protected Collider2D _holesTest;
		protected const string _fallingDownHoleAnimationParameterName = "FallingDownHole";
		protected int _fallingDownHoleAnimationParameter;

		/// <summary>
		/// On process ability, we check for holes
		/// </summary>
		public override void ProcessAbility()
		{
			base.ProcessAbility();
			CheckForHoles();
		}

		/// <summary>
		/// if we find a hole below our character, we kill our character
		/// </summary>
		protected virtual void CheckForHoles()
		{
			if (!AbilityAuthorized)
			{
				return;
			}
            
			if (_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
			{
				return;
			}

			if (_controller2D.OverHole && !_controller2D.Grounded)
			{ 
				if ((_movement.CurrentState != CharacterStates.MovementStates.Jumping)
				    && (_movement.CurrentState != CharacterStates.MovementStates.Dashing)
				    && (_condition.CurrentState != CharacterStates.CharacterConditions.Dead))
				{
					_movement.ChangeState(CharacterStates.MovementStates.FallingDownHole);
					FallingFeedback?.PlayFeedbacks(this.transform.position);
					PlayAbilityStartFeedbacks();
					_health.Kill();
				}
			}
		}

		/// <summary>
		/// Adds required animator parameters to the animator parameters list if they exist
		/// </summary>
		protected override void InitializeAnimatorParameters()
		{
			RegisterAnimatorParameter(_fallingDownHoleAnimationParameterName, AnimatorControllerParameterType.Bool, out _fallingDownHoleAnimationParameter);
		}

		/// <summary>
		/// At the end of each cycle, we send our Running status to the character's animator
		/// </summary>
		public override void UpdateAnimator()
		{
			PLAnimatorExtensions.UpdateAnimatorBool(_animator, _fallingDownHoleAnimationParameter, (_movement.CurrentState == CharacterStates.MovementStates.FallingDownHole), _character._animatorParameters, _character.RunAnimatorSanityChecks);
		}
	}
}