using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EmeraldAI.SoundDetection
{
    [RequireComponent(typeof(AudioSource))]
    public class AttractModifier : MonoBehaviour
    {
        #region Variables
        public FactionClass PlayerFaction;
        public int Radius = 10;
        public float MinVelocity = 3.5f;
        public float SoundCooldownSeconds = 1f;
        public float ReactionCooldownSeconds = 1f;
        public LayerMask TriggerLayers = ~0; //By default, use all layers for triggering an AttractModifier
        public LayerMask EmeraldAILayer;
        public TriggerTypes TriggerType = TriggerTypes.OnCollision;
        public ReactionObject AttractReaction;
        public bool EnemyRelationsOnly = true;
        public List<AudioClip> TriggerSounds = new List<AudioClip>();
        AudioSource m_AudioSource;
        bool ReactionTriggered;
        bool SoundTriggered;
        #endregion

        #region Editor Variables
        public bool HideSettingsFoldout;
        public bool AttractModifierFoldout;
        #endregion

        void Start()
        {
            m_AudioSource = GetComponent<AudioSource>();

            if (TriggerType == TriggerTypes.OnStart)
            {
                GetTargets();
            }
        }

        /// <summary>
        /// Invokes the specified reaction during a trigger collision.
        /// </summary>
        private void OnTriggerEnter(Collider collision)
        {
            if (TriggerType == TriggerTypes.OnTrigger)
            {
                GetTargets(((1 << collision.gameObject.layer) & TriggerLayers) != 0);
            }
        }

        /// <summary>
        /// Invokes the specified reaction during a collision that meets or exceeds the MinVelocity.
        /// </summary>
        private void OnCollisionEnter(Collision collision)
        {
            if (TriggerType == TriggerTypes.OnCollision && collision.relativeVelocity.magnitude >= MinVelocity)
            {
                GetTargets(((1 << collision.gameObject.layer) & TriggerLayers) != 0);
            }
        }

        /// <summary>
        /// Invokes the specified reaction when called (Requries the OnCustomCall TriggerType).
        /// </summary>
        public void ActivateAttraction ()
        {
            if (TriggerType == TriggerTypes.OnCustomCall)
            {
                GetTargets();
            }
        }

        /// <summary>
        /// Find all Emerald AI targets within the specified radius and invoke the AttractReaction.
        /// </summary>
        void GetTargets (bool HasTriggerLayer = true)
        {
            PlayTriggerSound();

            if (ReactionTriggered || Time.time < 0.5f || !HasTriggerLayer)
                return;

            Collider[] m_DetectedTargets = Physics.OverlapSphere(transform.position, Radius, EmeraldAILayer);

            if (m_DetectedTargets.Length == 0)
                return;

            for (int i = 0; i < m_DetectedTargets.Length; i++)
            {
                if (m_DetectedTargets[i].GetComponent<EmeraldSoundDetector>() != null)
                {
                    EmeraldSystem EmeraldComponent = m_DetectedTargets[i].GetComponent<EmeraldSystem>(); //Cache each EmeraldSystem

                    //Don't allow AI with follower targets to use Attract Modifiers.
                    if (EmeraldComponent.TargetToFollow != null) continue;

                    //Only allow AI with an Enemy relation to receive Attract Modifiers.
                    if (EnemyRelationsOnly && EmeraldComponent.DetectionComponent.FactionRelationsList.Exists(x => x.FactionIndex == PlayerFaction.FactionIndex && x.RelationType != 0)) continue;

                    if (AttractReaction != null)
                    {
                        EmeraldComponent.SoundDetectorComponent.DetectedAttractModifier = gameObject; //Assign the detected Emerald AI agent as the DetectedAttractModifier
                        EmeraldComponent.SoundDetectorComponent.InvokeReactionList(AttractReaction, true); //Invoke the ReactionList.
                    }
                    else
                    {
                        Debug.Log("There's no Reaction Object on the " + gameObject.name + "'s AttractReaction slot. Please add one in order for Attract Modifier to work correctly.");
                    }
                }
            }

            ReactionTriggered = true;
            Invoke("ReactionCooldown", ReactionCooldownSeconds);
        }

        void PlayTriggerSound ()
        {
            if (SoundTriggered || Time.time < 0.5f)
                return;

            if (TriggerSounds.Count > 0)
                m_AudioSource.PlayOneShot(TriggerSounds[Random.Range(0, TriggerSounds.Count)]);

            SoundTriggered = true;
            Invoke("SoundCooldown", SoundCooldownSeconds);
        }

        void SoundCooldown()
        {
            SoundTriggered = false;
        }

        void ReactionCooldown ()
        {
            ReactionTriggered = false;
        }
    }
}