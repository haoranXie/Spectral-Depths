using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace EmeraldAI
{
    /// <summary>
    /// Since most animations are repeated, a parent class is used for each animation category 
    /// (NonCombat, Type 1, and Type 2). Animations that are not used are simply not displayed.
    /// This also makes for adding new animations with future updates easier.
    /// </summary>
    [System.Serializable]
    public class AnimationParentClass
    {
        /// <summary>
        /// List of Animations (NonCombat Only)
        /// </summary>
        public List<AnimationClass> IdleList = new List<AnimationClass>();

        /// <summary>
        /// Stationary Idle Animation
        /// </summary>
        public AnimationClass IdleStationary;

        /// <summary>
        /// Warning Animation (Type 1 and Type 2 Only)
        /// </summary>
        public AnimationClass IdleWarning;

        /// <summary>
        /// Walk Animations
        /// </summary>
        public AnimationClass WalkLeft, WalkForward, WalkRight, WalkBack;

        /// <summary>
        /// Run Animations
        /// </summary>
        public AnimationClass RunLeft, RunForward, RunRight;

        /// <summary>
        /// Turn Animations
        /// </summary>
        public AnimationClass TurnLeft, TurnRight;

        /// <summary>
        /// List of Hit Animations
        /// </summary>
        public List<AnimationClass> HitList = new List<AnimationClass>();

        /// <summary>
        /// List of Death Animations
        /// </summary>
        public List<AnimationClass> DeathList = new List<AnimationClass>();

        /// <summary>
        /// Strafe Animations (Type 1 and Type 2 Only)
        /// </summary>
        public AnimationClass StrafeLeft, StrafeRight;

        /// <summary>
        /// Block Animations (Type 1 and Type 2 Only)
        /// </summary>
        public AnimationClass BlockIdle, BlockHit;

        /// <summary>
        /// Dodge Animations (Type 1 and Type 2 Only)
        /// </summary>
        public AnimationClass DodgeLeft, DodgeBack, DodgeRight;

        /// <summary>
        /// Recoil Animation (Type 1 and Type 2 Only)
        /// </summary>
        public AnimationClass Recoil;

        /// <summary>
        /// Stunned Animation (Type 1 and Type 2 Only)
        /// </summary>
        public AnimationClass Stunned;

        /// <summary>
        /// Equip and Unequip Animations (Type 1 and Type 2 Only)
        /// </summary>
        public AnimationClass PutAwayWeapon, PullOutWeapon;

        /// <summary>
        /// List of Attack Animations (Type 1 and Type 2 Only)
        /// </summary>
        public List<AnimationClass> AttackList = new List<AnimationClass>(); 
    }
}