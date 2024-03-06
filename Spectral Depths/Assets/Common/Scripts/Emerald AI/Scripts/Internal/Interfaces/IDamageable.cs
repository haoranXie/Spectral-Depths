using UnityEngine;
using System.Collections.Generic;

namespace EmeraldAI
{
    /// <summary>
    /// An interafce script used for monitoring and tracking a target. This allows other AI to see any target's information by using customizable functions.
    /// </summary>
    public interface IDamageable
    {
        /// <summary>
        /// Used for passing damage to any script that has an IDamageable component.
        /// </summary>
        void Damage(int DamageAmount, Transform AttackerTransform = null, int RagdollForce = 100, bool CriticalHit = false);

        int Health { get; set; }

        int StartHealth { get; set; }

        /// <summary>
        /// Used for tracking active damage over time effects on targets.
        /// </summary>
        List<string> ActiveEffects { get; set; }
    }

    public static class IDamageableHelper
    {
        public static bool IsDead (this GameObject receiver)
        {
            var m_IDamageable = receiver.GetComponent<IDamageable>();
            if (m_IDamageable != null) return m_IDamageable.Health <= 0;
            else return false;
        }

        public static bool CheckAbilityActiveEffects (this GameObject receiver, EmeraldAbilityObject AbilityData)
        {
            var m_IDamageable = receiver.GetComponent<IDamageable>();
            if (m_IDamageable != null)
            {
                return !m_IDamageable.ActiveEffects.Contains(AbilityData.AbilityName) && AbilityData.AbilityName != string.Empty;
            }
            else
            {
                return false;
            }
        }

        public static void AddAbilityActiveEffect(this GameObject receiver, EmeraldAbilityObject AbilityData)
        {
            var m_IDamageable = receiver.GetComponent<IDamageable>();
            if (m_IDamageable != null)
            {
                if (!m_IDamageable.ActiveEffects.Contains(AbilityData.AbilityName) && AbilityData.AbilityName != string.Empty)
                {
                    m_IDamageable.ActiveEffects.Add(AbilityData.AbilityName);
                }
            }
        }

        public static void RemoveAbilityActiveEffect(this GameObject receiver, EmeraldAbilityObject AbilityData)
        {
            var m_IDamageable = receiver.GetComponent<IDamageable>();
            if (m_IDamageable != null)
            {
                if (m_IDamageable.ActiveEffects.Contains(AbilityData.AbilityName) && AbilityData.AbilityName != string.Empty)
                {
                    m_IDamageable.ActiveEffects.Remove(AbilityData.AbilityName);
                }
            }
        }
    }
}