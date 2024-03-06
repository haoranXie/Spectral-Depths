using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace EmeraldAI.Utility
{
    /// <summary>
    /// Handles all Emerald AI combat related functionality.
    /// </summary>
    public static class EmeraldCombatManager
    {
        /// <summary>
        /// Handles the generation of all Emerald AI attacks.
        /// </summary>
        public static void GenerateAttack(EmeraldSystem EmeraldComponent, AttackClass SentAttackClass, bool OverridePickType = false)
        {
            if (SentAttackClass.AttackDataList.Count > 0)
            {
                List<AttackClass.AttackData> AvailableAttacks = new List<AttackClass.AttackData>();

                //Go through the attack list and get all cooled down attacks. Note: This is ignored for the Ordered Pick Type.
                for (int i = 0; i < SentAttackClass.AttackDataList.Count; i++)
                {
                    SentAttackClass.AttackDataList[i].CooldownIgnored = false; //Reset before checking cooldown again

                    float CooldownTime = 0;
                    if (SentAttackClass.AttackDataList[i].AbilityObject != null) CooldownTime = (SentAttackClass.AttackDataList[i].CooldownTimeStamp + SentAttackClass.AttackDataList[i].AbilityObject.CooldownSettings.CooldownLength);

                    if (Time.time >= CooldownTime || SentAttackClass.AttackDataList[i].CooldownTimeStamp == 0 || SentAttackClass.AttackDataList[i].AbilityObject != null && !SentAttackClass.AttackDataList[i].AbilityObject.CooldownSettings.Enabled)
                    {
                        if (!AvailableAttacks.Contains(SentAttackClass.AttackDataList[i]))
                        {
                            AvailableAttacks.Add(SentAttackClass.AttackDataList[i]);
                        }
                    }
                }

                //If there are no available attacks, ignore the cooldowns and pick a random ability to avoid an error.
                if (AvailableAttacks.Count == 0 && SentAttackClass.AttackPickType != AttackPickTypes.Order)
                {
                    SetAttackValues(EmeraldComponent, SentAttackClass.AttackDataList[Random.Range(0, SentAttackClass.AttackDataList.Count)], true);
                    Debug.Log("All cooldowns are busy with the: '" + EmeraldComponent.gameObject.name + "' AI so the a random attack within its Attack List was used. To avoid this, add more attacks or decrease some of this AI's cooldowns times within the Ability Objects.");
                    return;
                }

                if (SentAttackClass.AttackPickType == AttackPickTypes.Odds) //Pick an ability by using the odds for each available ability
                {
                    List<float> OddsList = new List<float>();
                    for (int i = 0; i < AvailableAttacks.Count; i++)
                    {
                        OddsList.Add(AvailableAttacks[i].AttackOdds);
                    }
                    int OddsIndex = (int)GenerateProbability(OddsList.ToArray());
                    SetAttackValues(EmeraldComponent, AvailableAttacks[OddsIndex], false);
                }
                else if (SentAttackClass.AttackPickType == AttackPickTypes.Order) //Pick an ability by going through the list in order
                {
                    SetAttackValues(EmeraldComponent, SentAttackClass.AttackDataList[SentAttackClass.AttackListIndex], true);

                    SentAttackClass.AttackListIndex++;
                    if (SentAttackClass.AttackListIndex == SentAttackClass.AttackDataList.Count) SentAttackClass.AttackListIndex = 0;
                }
                else if (SentAttackClass.AttackPickType == AttackPickTypes.Random) //Pick a random ability from the list
                {
                    int RandomIndex = Random.Range(0, AvailableAttacks.Count);
                    SetAttackValues(EmeraldComponent, AvailableAttacks[RandomIndex], false);
                }

                //Overrides the current pick type by picking the closest attack. So far, this is done to force a random attack from the AI's attack list.
                if (OverridePickType)
                {
                    //TODO: This section will be improved with an update.
                    /*
                    float min = SentAttackClass.AttackDataList.Min(attack => attack.TooCloseDistance);
                    var ClosestAttacks = SentAttackClass.AttackDataList.Where(attack => attack.TooCloseDistance == min).ToList();
                    var RandomCloseAttack = ClosestAttacks[Random.Range(0, ClosestAttacks.Count)];
                    Debug.Log(EmeraldComponent.name + "   " + ClosestAttacks.Count);
                    int AttackIndex = SentAttackClass.AttackDataList.FindIndex(attack => attack == RandomCloseAttack);
                    SentAttackClass.AttackListIndex = AttackIndex;
                    EmeraldComponent.CombatComponent.CurrentEmeraldAIAbility = SentAttackClass.AttackDataList[AttackIndex].AbilityObject;
                    EmeraldComponent.CombatComponent.CurrentAnimationIndex = SentAttackClass.AttackDataList[AttackIndex].AttackAnimation;
                    SetAttackValues(EmeraldComponent, AvailableAttacks[AttackIndex], true);
                    */

                    int RandomIndex = Random.Range(0, AvailableAttacks.Count);
                    SetAttackValues(EmeraldComponent, AvailableAttacks[RandomIndex], false);
                }
            }
        }
        
        /// <summary>
        /// Sets the data of the currently generated attack.
        /// </summary>
        static void SetAttackValues(EmeraldSystem EmeraldComponent, AttackClass.AttackData AttackData, bool CooldownIgnored)
        {
            EmeraldComponent.CombatComponent.CurrentAttackData = AttackData;

            AttackData.CooldownIgnored = CooldownIgnored;

            EmeraldComponent.CombatComponent.CurrentAnimationIndex = AttackData.AttackAnimation;
            EmeraldComponent.CombatComponent.CurrentEmeraldAIAbility = AttackData.AbilityObject;

            EmeraldComponent.CombatComponent.AttackDistance = AttackData.AttackDistance;
            //Don't allow the attack distance from being higher than the detection distance.
            if (EmeraldComponent.CombatComponent.AttackDistance > EmeraldComponent.DetectionComponent.DetectionRadius) EmeraldComponent.CombatComponent.AttackDistance = EmeraldComponent.DetectionComponent.DetectionRadius;
            EmeraldComponent.CombatComponent.TooCloseDistance = AttackData.TooCloseDistance;
            if (EmeraldComponent.CombatComponent.CombatState) EmeraldComponent.m_NavMeshAgent.stoppingDistance = EmeraldComponent.CombatComponent.AttackDistance;
        }

        /// <summary>
        /// Generate the next attack using the current attack class and the AI's Pick Type.
        /// </summary>
        public static void GenerateNextAttack (EmeraldSystem EmeraldComponent)
        {
            if (EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type1)
                GenerateAttack(EmeraldComponent, EmeraldComponent.CombatComponent.Type1Attacks);
            else if (EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type2)
                GenerateAttack(EmeraldComponent, EmeraldComponent.CombatComponent.Type2Attacks);

            EmeraldComponent.AnimationComponent.AttackingTracker = false;
        }

        /// <summary>
        /// Overrides the currently generated attack with the closest attack within an AI's Attack List. 
        /// </summary>
        public static void GenerateClosestAttack(EmeraldSystem EmeraldComponent)
        {
            EmeraldComponent.AnimationComponent.AttackingTracker = false;

            if (EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type1)
                GenerateAttack(EmeraldComponent, EmeraldComponent.CombatComponent.Type1Attacks, true);
            else if (EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type2)
                GenerateAttack(EmeraldComponent, EmeraldComponent.CombatComponent.Type2Attacks, true);
        }

        /// <summary>
        /// Updates the AI's current attack and weapon transforms based on the sent AttackTransformName from an EmeraldAttackEvent Animation Event.
        /// This is done by searching for a matching name within the Attack Transform list. If no match is found, the AI's head transfrom will be used instead.
        /// </summary>
        public static void UpdateAttackTransforms (EmeraldSystem EmeraldComponent, string AttackTransformName)
        {
            if (EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type1)
            {
                Transform AttackTransform = EmeraldComponent.CombatComponent.WeaponType1AttackTransforms.Find(x => x != null && x.name == AttackTransformName);
                if (AttackTransform != null)
                {
                    EmeraldComponent.CombatComponent.CurrentAttackTransform = AttackTransform;
                }
                else
                {
                    if (EmeraldComponent.CombatComponent.WeaponType1AttackTransforms.Count > 0) EmeraldComponent.CombatComponent.CurrentAttackTransform = EmeraldComponent.CombatComponent.WeaponType1AttackTransforms[0];
                    else EmeraldComponent.CombatComponent.CurrentAttackTransform = EmeraldComponent.DetectionComponent.HeadTransform;
                }
            }
            else if (EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type2)
            {
                Transform AttackTransform = EmeraldComponent.CombatComponent.WeaponType2AttackTransforms.Find(x => x.name == AttackTransformName);
                if (AttackTransform != null)
                {
                    EmeraldComponent.CombatComponent.CurrentAttackTransform = AttackTransform;
                }
                else
                {
                    EmeraldComponent.CombatComponent.CurrentAttackTransform = EmeraldComponent.DetectionComponent.HeadTransform;
                }
            }
        }

        /// <summary>
        /// Returns the weapon transforms based on the sent AttackTransformName from an EmeraldChargeAttack Animation Event.
        /// This is done by searching for a matching name within the Attack Transform list. If no match is found, the EmeraldChargeAttack event will not fire.
        /// </summary>
        public static Transform GetAttackTransform(EmeraldSystem EmeraldComponent, string AttackTransformName)
        {
            Transform WeaponTransform = null;

            if (EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type1)
            {
                WeaponTransform = EmeraldComponent.CombatComponent.WeaponType1AttackTransforms.Find(x => x != null && x.name == AttackTransformName);
            }
            else
            {
                WeaponTransform = EmeraldComponent.CombatComponent.WeaponType2AttackTransforms.Find(x => x.name == AttackTransformName);
            }

            return WeaponTransform;
        }

        /// <summary>
        /// Generates the AI's weapon type swap time.
        /// </summary>
        public static void ResetWeaponSwapTime (EmeraldSystem EmeraldComponent)
        {
            EmeraldComponent.CombatComponent.SwitchWeaponTime = Random.Range((float)EmeraldComponent.CombatComponent.SwitchWeaponTimeMin, (float)EmeraldComponent.CombatComponent.SwitchWeaponTimeMax + 1);
            EmeraldComponent.CombatComponent.SwitchWeaponTimer = 0;
        }

        /// <summary>
        /// Activates the AI's Combat State.
        /// </summary>
        public static void ActivateCombatState(EmeraldSystem EmeraldComponent)
        {
            if (EmeraldComponent.CombatComponent.CombatState)
                return;

            EmeraldComponent.AIAnimator.ResetTrigger("Hit");
            EmeraldComponent.CombatComponent.CombatState = true;
            EmeraldComponent.AIAnimator.SetBool("Idle Active", false);
            EmeraldComponent.AIAnimator.SetBool("Combat State Active", true);
            EmeraldComponent.MovementComponent.CurrentMovementState = EmeraldMovement.MovementStates.Run;
        }

        /// <summary>
        /// Disables an AI's components (called when an AI dies).
        /// </summary>
        public static void DisableComponents(EmeraldSystem EmeraldComponent)
        {
            if (EmeraldComponent.GetComponent<SoundDetection.EmeraldSoundDetector>() != null)
                EmeraldComponent.GetComponent<SoundDetection.EmeraldSoundDetector>().enabled = false;

            EmeraldComponent.CombatComponent.ExitCombat();
            EmeraldComponent.AIBoxCollider.enabled = false;
            EmeraldComponent.DetectionComponent.enabled = false;
            EmeraldComponent.AnimationComponent.enabled = false;
            EmeraldComponent.CombatComponent.enabled = false;
            EmeraldComponent.MovementComponent.enabled = false;
            EmeraldComponent.BehaviorsComponent.enabled = false;
            EmeraldComponent.m_NavMeshAgent.ResetPath();
            EmeraldComponent.m_NavMeshAgent.enabled = false;
        }

        /// <summary>
        /// Enables an AI's components (called when an AI is reset).
        /// </summary>
        public static void EnableComponents(EmeraldSystem EmeraldComponent)
        {
            if (EmeraldComponent.GetComponent<SoundDetection.EmeraldSoundDetector>() != null)
                EmeraldComponent.GetComponent<SoundDetection.EmeraldSoundDetector>().enabled = true;

            if (EmeraldComponent.GetComponent<EmeraldInverseKinematics>() != null) EmeraldComponent.GetComponent<EmeraldInverseKinematics>().EnableInverseKinematics();
            if (EmeraldComponent.LBDComponent != null) EmeraldComponent.LBDComponent.ResetLBDComponents();
            if (EmeraldComponent.ItemsComponent != null) EmeraldComponent.ItemsComponent.ResetSettings();
            EmeraldComponent.m_NavMeshAgent.enabled = true;
            EmeraldComponent.AIBoxCollider.enabled = true;
            EmeraldComponent.DetectionComponent.enabled = true;
            EmeraldComponent.AnimationComponent.enabled = true;
            EmeraldComponent.CombatComponent.enabled = true;
            EmeraldComponent.MovementComponent.enabled = true;
            EmeraldComponent.BehaviorsComponent.enabled = true;
            EmeraldComponent.AIAnimator.enabled = true;
            EmeraldComponent.AIAnimator.Rebind();
            EmeraldComponent.AnimationComponent.InitializeWeaponTypeAnimationAndSettings();
        }

        /// <summary>
        /// Disable colliders and rigidbodies, given they're detected.
        /// </summary>
        public static void DisableRagdoll(EmeraldSystem EmeraldComponent)
        {
            //Return if a LocationBasedDamage component is detected as colliders need to stay active for this feature.
            if (EmeraldComponent.GetComponent<LocationBasedDamage>() != null)
                return;

            foreach (Rigidbody R in EmeraldComponent.GetComponentsInChildren<Rigidbody>())
            {
                R.isKinematic = true;
            }

            if (EmeraldComponent.LBDComponent != null)
                return;

            foreach (Collider C in EmeraldComponent.GetComponentsInChildren<Collider>())
            {
                C.enabled = false;
            }

            EmeraldComponent.GetComponent<BoxCollider>().enabled = true;
        }

        /// <summary>
        /// Enable colliders and rigidbodies inside the AI, given they're detected.
        /// </summary>
        public static void EnableRagdoll(EmeraldSystem EmeraldComponent)
        {
            //Only enable the ragdoll components if the current weapon type death animation lists don't have animations in them.
            if (EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type1 && EmeraldComponent.AnimationComponent.m_AnimationProfile.Type1Animations.DeathList.Count > 0 ||
                EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type2 && EmeraldComponent.AnimationComponent.m_AnimationProfile.Type2Animations.DeathList.Count > 0)
                return;

            EmeraldComponent.AIAnimator.enabled = false;

            if (EmeraldComponent.LBDComponent == null)
            {
                foreach (Collider C in EmeraldComponent.GetComponentsInChildren<Collider>())
                {
                    if (C.transform != EmeraldComponent.transform)
                    {
                        C.enabled = true;
                    }
                }

                foreach (Rigidbody R in EmeraldComponent.GetComponentsInChildren<Rigidbody>())
                {
                    R.isKinematic = false;
                    R.useGravity = true;
                }
            }
            else
            {
                for (int i = 0; i < EmeraldComponent.LBDComponent.ColliderList.Count; i++)
                {
                    if (EmeraldComponent.LBDComponent.ColliderList[i].ColliderObject != null)
                        EmeraldComponent.LBDComponent.ColliderList[i].ColliderObject.enabled = true;
                }

                for (int i = 0; i < EmeraldComponent.LBDComponent.ColliderList.Count; i++)
                {
                    Rigidbody ColliderRigidbody = EmeraldComponent.LBDComponent.ColliderList[i].ColliderObject.GetComponent<Rigidbody>();

                    if (ColliderRigidbody != null)
                    {
                        ColliderRigidbody.isKinematic = false;
                        ColliderRigidbody.useGravity = true;
                    }

                    EmeraldComponent.LBDComponent.ColliderList[i].ColliderObject.enabled = true;
                }
            }

            AddRagdollForce(EmeraldComponent); //Add force to the ragdoll after enable its components
        }

        /// <summary>
        /// Adds force in the opposite direction of the last detected damage source.
        /// </summary>
        public static void AddRagdollForce (EmeraldSystem EmeraldComponent)
        {
            if (EmeraldComponent.CombatComponent.RagdollTransform == null)
            {
                if (EmeraldComponent.DetectionComponent.HeadTransform.GetComponent<Rigidbody>() != null)
                    EmeraldComponent.CombatComponent.RagdollTransform = EmeraldComponent.DetectionComponent.HeadTransform;
                else
                {
                    Rigidbody[] RandomRagdollComponent = EmeraldComponent.GetComponentsInChildren<Rigidbody>();
                    if (RandomRagdollComponent.Length > 0)
                        EmeraldComponent.CombatComponent.RagdollTransform = RandomRagdollComponent[Random.Range(0, RandomRagdollComponent.Length)].transform;
                }
            }

            //Used to provide the force to the ragdoll in the opposite direction of the last target to hit the AI.
            Transform LastAttacker = EmeraldComponent.CombatComponent.LastAttacker;

            if (LastAttacker != null && LastAttacker != EmeraldComponent.CombatTarget)
            {
                EmeraldComponent.CombatComponent.RagdollTransform.GetComponent<Rigidbody>().AddForce((LastAttacker.position - EmeraldComponent.transform.position).normalized * -EmeraldComponent.CombatComponent.ReceivedRagdollForceAmount, ForceMode.Impulse);
            }
            else
            {
                EmeraldComponent.CombatComponent.RagdollTransform.GetComponent<Rigidbody>().AddForce((EmeraldComponent.CombatTarget.position - EmeraldComponent.transform.position).normalized * -EmeraldComponent.CombatComponent.ReceivedRagdollForceAmount, ForceMode.Impulse);
            }
        }

        /// <summary>
        /// Returns true if the conditions are right to allow the AI to trigger an attack.
        /// </summary>
        public static bool AllowedToAttack(EmeraldSystem EmeraldComponent)
        {
            if (EmeraldComponent.DetectionComponent.TargetObstructed || EmeraldComponent.CombatComponent.DeathDelayActive || EmeraldComponent.MovementComponent.DefaultMovementPaused) return false;
            if (!WithinStoppingDistanceOfTarget(EmeraldComponent) || !WithinDistanceOfTarget(EmeraldComponent)) return false;
            if (EmeraldComponent.AIAnimator.GetBool("Hit") || EmeraldComponent.AIAnimator.GetBool("Strafe Active") || EmeraldComponent.AIAnimator.GetBool("Dodge Triggered") || EmeraldComponent.AIAnimator.GetBool("Blocking") || EmeraldComponent.AnimationComponent.IsBackingUp || EmeraldComponent.AnimationComponent.IsBlocking || EmeraldComponent.AnimationComponent.IsAttacking || EmeraldComponent.AnimationComponent.IsRecoiling || EmeraldComponent.AnimationComponent.IsStrafing || EmeraldComponent.AnimationComponent.IsDodging || EmeraldComponent.AnimationComponent.IsGettingHit) return false;
            if (!EmeraldComponent.CombatComponent.TargetWithinAngleLimit() || EmeraldComponent.CurrentTargetInfo.CurrentIDamageable.Health <= 0) return false;
            //The AI has passed all checks and can trigger an attack.
            return true;
        }

        /// <summary>
        /// Checks if the AI is within stopping distance of its current target (using Nav Mesh remainingDistance).
        /// </summary>
        static bool WithinStoppingDistanceOfTarget(EmeraldSystem EmeraldComponent)
        {
            return (EmeraldComponent.m_NavMeshAgent.remainingDistance <= EmeraldComponent.m_NavMeshAgent.stoppingDistance);
        }

        /// <summary>
        /// Checks if the AI is within stopping distance of its current target (using Vector3.Distance).
        /// </summary>
        static bool WithinDistanceOfTarget(EmeraldSystem EmeraldComponent)
        {
            return (EmeraldComponent.CombatComponent.DistanceFromTarget <= EmeraldComponent.m_NavMeshAgent.stoppingDistance);
        }

        /// <summary>
        /// Returns the height between the AI and its current target (only calculating using the Y axis).
        /// </summary>
        public static float GetTargetHeight (EmeraldSystem EmeraldComponent)
        {
            Vector3 m_TargetPos = EmeraldComponent.CombatTarget.position;
            m_TargetPos.x = 0;
            m_TargetPos.z = 0;
            Vector3 m_CurrentPos = EmeraldComponent.transform.position;
            m_CurrentPos.x = 0;
            m_CurrentPos.z = 0;
            return Vector3.Distance(m_TargetPos, m_CurrentPos);
        }

        /// <summary>
        /// Generate the probability for the Random Pick Type.
        /// </summary>
        public static float GenerateProbability(float[] probs)
        {
            float total = 0;

            foreach (float elem in probs)
            {
                total += elem;
            }

            float randomPoint = Random.value * total;

            for (int i = 0; i < probs.Length; i++)
            {
                if (randomPoint < probs[i])
                {
                    return i;
                }
                else
                {
                    randomPoint -= probs[i];
                }
            }
            return probs.Length - 1;
        }

        /// <summary>
        /// Returns the current angle from the passed Transform.
        /// </summary>
        public static float TransformAngle(EmeraldSystem EmeraldComponent, Transform Target)
        {
            if (Target == null)
                return 180;

            Vector3 Direction = new Vector3(Target.position.x, 0, Target.position.z) - new Vector3(EmeraldComponent.transform.position.x, 0, EmeraldComponent.transform.position.z);
            float angle = Vector3.Angle(EmeraldComponent.transform.forward, Direction);
            float RotationDifference = EmeraldComponent.transform.localEulerAngles.x;
            RotationDifference = (RotationDifference > 180) ? RotationDifference - 360 : RotationDifference;
            float AdjustedAngle = Mathf.Abs(angle) - Mathf.Abs(RotationDifference);
            return AdjustedAngle;
        }

        /// <summary>
        /// Returns the current angle from the AI's target.
        /// </summary>
        public static float TargetAngle(EmeraldSystem EmeraldComponent)
        {
            if (EmeraldComponent.CombatTarget == null)
                return 360;
            Vector3 Direction = new Vector3(EmeraldComponent.CombatTarget.position.x, 0, EmeraldComponent.CombatTarget.position.z) - new Vector3(EmeraldComponent.transform.position.x, 0, EmeraldComponent.transform.position.z);
            float angle = Vector3.Angle(EmeraldComponent.transform.forward, Direction);
            float RotationDifference = EmeraldComponent.transform.localEulerAngles.x;
            RotationDifference = (RotationDifference > 180) ? RotationDifference - 360 : RotationDifference;
            return Mathf.Abs(angle) - Mathf.Abs(RotationDifference);
        }

        /// <summary>
        /// Gets the distance between this AI and its current target.
        /// </summary>
        public static float GetDistanceFromTarget(EmeraldSystem EmeraldComponent)
        {
            if (EmeraldComponent.CombatTarget == null) return 0;

            Vector3 CurrentTargetPos = EmeraldComponent.CombatTarget.position;
            CurrentTargetPos.y = 0;
            Vector3 CurrentPos = EmeraldComponent.transform.position;
            CurrentPos.y = 0;
            return Vector3.Distance(CurrentTargetPos, CurrentPos);
        }

        /// <summary>
        /// Gets the distance between this AI and its current look at target.
        /// </summary>
        public static float GetDistanceFromLookTarget(EmeraldSystem EmeraldComponent)
        {
            if (EmeraldComponent.LookAtTarget == null) return 0;

            Vector3 CurrentTargetPos = EmeraldComponent.LookAtTarget.position;
            CurrentTargetPos.y = 0;
            Vector3 CurrentPos = EmeraldComponent.transform.position;
            CurrentPos.y = 0;
            return Vector3.Distance(CurrentTargetPos, CurrentPos);
        }
    }
}