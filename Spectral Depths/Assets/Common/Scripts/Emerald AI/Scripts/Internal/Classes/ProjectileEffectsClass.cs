using UnityEngine;

namespace EmeraldAI
{
    /// <summary>
    /// Used through projectiles to cache effects so they can be enabled and disabled when needed.
    /// </summary>
    [System.Serializable]
    public class ProjectileEffectsClass
    {
        public ParticleSystemRenderer EffectParticle;
        public GameObject EffectObject;

        public ProjectileEffectsClass(ParticleSystemRenderer m_EffectParticle, GameObject m_EffectObject)
        {
            EffectParticle = m_EffectParticle;
            EffectObject = m_EffectObject;
        }
    }
}