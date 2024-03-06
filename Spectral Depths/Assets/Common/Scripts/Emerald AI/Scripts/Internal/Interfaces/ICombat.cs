using UnityEngine;

namespace EmeraldAI
{
    /// <summary>
    /// An interafce script used for monitoring and tracking a target's combat actions (as well as its damage position). This allows other AI to see any target's information through functions. 
    /// Note: This interface is required (on 3rd party or custom character controllers) to use the blocking and dodging features for player targets.
    /// </summary>
    public interface ICombat
    {
        /// <summary>
        /// Used for getting the transform of the target.
        /// </summary>
        Transform TargetTransform();

        /// <summary>
        /// Used for getting the damage position of the target.
        /// </summary>
        Vector3 DamagePosition();

        /// <summary>
        /// Used for detecting when a target is attacking.
        /// </summary>
        bool IsAttacking();

        /// <summary>
        /// Used for detecting when a target is blocking.
        /// </summary>
        bool IsBlocking();

        /// <summary>
        /// Used for detecting when a target is dodging.
        /// </summary>
        bool IsDodging();

        /// <summary>
        /// Used through Emerald AI to trigger stunned mechanics, however, can also be extended to trigger stunned mechanics through custom character controllers, given they have them.
        /// </summary>
        void TriggerStun(float StunLength);
    }
}