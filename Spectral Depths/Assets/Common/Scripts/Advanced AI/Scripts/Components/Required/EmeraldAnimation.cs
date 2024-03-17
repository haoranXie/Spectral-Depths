using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmeraldAI.Utility;

namespace EmeraldAI
{
    public class EmeraldAnimation : MonoBehaviour
    {
        [SerializeField]
        #region Animation States
        public AnimatorStateInfo CurrentStateInfo;
        public bool IsEmoting;
        public bool IsIdling;
        public bool IsAttacking;
        public bool IsStrafing;
        public bool IsBlocking;
        public bool IsDodging;
        public bool IsRecoiling;
        public bool IsStunned;
        public bool IsGettingHit;
        public bool IsEquipping;
        public bool IsBackingUp;
        public bool IsTurning;
        public bool IsTurningLeft, IsTurningRight;
        public bool IsSwitchingWeapons;
        public bool IsWarning;
        public bool IsMoving;
        public bool IsDead;
        public bool m_IdleAnimaionIndexOverride = false;
        #endregion

        #region Animation Variables
        public EmeraldAI.Utility.AnimationProfile m_AnimationProfile;
        public bool AnimatorControllerGenerated = false;
        public bool AnimationListsChanged = false;
        public bool MissingRuntimeController = false;
        public bool AnimationsUpdated = false;
        public Animator AIAnimator;
        public AnimatorCullingMode AnimatorCullingMode = AnimatorCullingMode.AlwaysAnimate;
        public YesOrNo UseWarningAnimation = YesOrNo.No;
        public bool AttackingTracker; //Called right when an attack is generated
        public bool AttackTriggered; //Briefly called while an attack is playing
        public bool WarningAnimationTriggered = false;
        public bool BusyBetweenStates = false;
        public AnimationStateTypes CurrentAnimationState = AnimationStateTypes.Idling;
        public delegate void GetHitHandler();
        public event GetHitHandler OnGetHit;
        public delegate void RecoilHandler();
        public event RecoilHandler OnRecoil;
        public delegate void StartAttackAnimationHandler();
        public event StartAttackAnimationHandler OnStartAttackAnimation;
        public delegate void EndAttackAnimationHandler();
        public event StartAttackAnimationHandler OnEndAttackAnimation;
        float LastHitTime;
        Coroutine StunnedCoroutine;
        EmeraldSystem EmeraldComponent;
        #endregion

        #region Editor Variables
        public string[] Type1AttackEnumAnimations;
        public string[] Type2AttackEnumAnimations;
        public string[] Type1AttackBlankOptions = { "No Type 1 Attack Animations" };
        public string[] Type2AttackBlankOptions = { "No Type 2 Attack Animations" };
        public bool HideSettingsFoldout;
        public bool AnimationProfileFoldout;
        #endregion

        void Start()
        {
            InitailizeAnimations();
            SetupAnimator();
        }

        /// <summary>
        /// Initialize the Animation Component.
        /// </summary>
        void InitailizeAnimations()
        {
            AIAnimator = GetComponentInChildren<Animator>();
            AIAnimator.runtimeAnimatorController = m_AnimationProfile.AIAnimator;
            EmeraldComponent = GetComponentInParent<EmeraldSystem>();

            EmeraldComponent.HealthComponent.OnTakeDamage += PlayHitAnimation; //Subscribe to the OnTakeDamage event for Hit Animations
            EmeraldComponent.HealthComponent.OnTakeCritDamage += PlayHitAnimation; //Subscribe to the OnTakeCritDamage event for Hit Animations
            EmeraldComponent.HealthComponent.OnBlock += PlayHitAnimation; //Subscribe to the OnTakeCritDamage event for Block Hit Animations
            EmeraldComponent.HealthComponent.OnDeath += PlayDeathAnimation; //Subscribe to the OnDeath event for Death Animations
            EmeraldComponent.MovementComponent.OnReachedWaypoint += PlayIdleAnimation; //Subscribe to the OnReachedWaypoint event for Idle Animations
            EmeraldComponent.CombatComponent.OnExitCombat += ReturnToDefaultState; //Subscribe to the OnExitCombat event for ReturnToDefaultState

            if (AnimatorCullingMode == AnimatorCullingMode.AlwaysAnimate)
                AIAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            else if (AnimatorCullingMode == AnimatorCullingMode.CullUpdateTransforms)
                AIAnimator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;

            InitializeWeaponTypeAnimationAndSettings();
            AIAnimator.updateMode = AnimatorUpdateMode.Normal;
            AIAnimator.SetFloat("Offset", Random.Range(0.0f, 1.0f)); //Add a randomized offset so AI sharing animations don't start at the exact same frame.
        }

        public void AnimationUpdate ()
        {
            EmeraldComponent.AnimationComponent.CurrentStateInfo = AIAnimator.GetCurrentAnimatorStateInfo(0); //Update the CurrentStateInfo, which is used for getting the current state and tracking the AI's current states and animations.
            CheckAnimationStates(); //Keeps track of the current animation state.

            //While simple, this is needed to check when the current attack animation finishes. When it does, generate a new attack.
            if (IsAttacking && !AttackingTracker)
            {
                OnStartAttackAnimation?.Invoke();
                AttackingTracker = true;
                AttackTriggered = true;
                Invoke(nameof(StopAttackTrigger), 0.5f);
            }
            else if (!IsAttacking && AttackingTracker)
            {
                OnEndAttackAnimation?.Invoke();
                EmeraldCombatManager.GenerateNextAttack(EmeraldComponent); //Generate the next attack once the current one has concluded.
            }
        }

        void StopAttackTrigger()
        {
            AttackTriggered = false;
        }

        /// <summary>
        /// Keeps track of the current animation state.
        /// </summary>
        public void CheckAnimationStates()
        {
            if (!EmeraldComponent.CombatComponent.CombatState) IsIdling = CurrentStateInfo.IsName("Movement") && AIAnimator.GetFloat("Speed") < 0.1f && !IsBackingUp || CurrentStateInfo.IsTag("Idle");
            if (!EmeraldComponent.CombatComponent.CombatState) IsMoving = CurrentStateInfo.IsName("Movement") && AIAnimator.GetFloat("Speed") >= 0.1f && !IsBackingUp;

            if (EmeraldComponent.CombatComponent.CombatState && EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type1)
            {
                IsIdling = CurrentStateInfo.IsName("Combat Movement (Type 1)") && AIAnimator.GetFloat("Speed") < 0.1f;
                IsMoving = CurrentStateInfo.IsName("Combat Movement (Type 1)") && AIAnimator.GetFloat("Speed") >= 0.1f && !IsBackingUp && !IsAttacking;
                if(AIAnimator.GetBool("Player Controls"))
                {
                    IsIdling = CurrentStateInfo.IsName("Player Idle (Type 1)") && AIAnimator.GetFloat("Speed") < 0.1f;
                    IsMoving = (CurrentStateInfo.IsName("Player Walking (Type 1)") ||CurrentStateInfo.IsName("Player Running (Type 1)")) && AIAnimator.GetFloat("Speed") >= 0.1f && !IsBackingUp && !IsAttacking;
                }
            }
            else if (EmeraldComponent.CombatComponent.CombatState && EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type2)
            {
                IsIdling = CurrentStateInfo.IsName("Combat Movement (Type 2)") && AIAnimator.GetFloat("Speed") < 0.1f;
                IsMoving = CurrentStateInfo.IsName("Combat Movement (Type 2)") && AIAnimator.GetFloat("Speed") >= 0.1f && !IsBackingUp && !IsAttacking;
                if(AIAnimator.GetBool("Player Controls"))
                {
                    IsIdling = CurrentStateInfo.IsName("Player Idle (Type 2)") && AIAnimator.GetFloat("Speed") < 0.1f;
                    IsMoving = (CurrentStateInfo.IsName("Player Walking (Type 2)") ||CurrentStateInfo.IsName("Player Running (Type 2)")) && AIAnimator.GetFloat("Speed") >= 0.1f && !IsBackingUp && !IsAttacking;
                }
            }


            IsEquipping = CurrentStateInfo.IsTag("Equip");
            IsBlocking = CurrentStateInfo.IsTag("Block");
            IsRecoiling = CurrentStateInfo.IsTag("Recoil");
            IsStunned = CurrentStateInfo.IsTag("Stunned");
            IsStrafing = CurrentStateInfo.IsTag("Strafing");
            IsDodging = CurrentStateInfo.IsTag("Dodging");
            IsBackingUp = CurrentStateInfo.IsTag("Backing Up") || AIAnimator.GetBool("Walk Backwards");            
            IsAttacking = CurrentStateInfo.IsTag("Attack");
            IsGettingHit = CurrentStateInfo.IsTag("Hit");
            IsWarning = CurrentStateInfo.IsTag("Warning");
            IsEmoting = CurrentStateInfo.IsTag("Emote");

            //This is used to determine when an AI is in between combat and non-combat states. This stops undsired rotations that happens during these transitions and allows the mechanics to function much smoother. 
            BusyBetweenStates = AIAnimator.GetAnimatorTransitionInfo(0).IsName("Combat Movement (Type 1) -> Movement") || AIAnimator.GetAnimatorTransitionInfo(0).IsName("Combat Movement (Type 2) -> Movement") ||
                AIAnimator.GetAnimatorTransitionInfo(0).IsName("Movement -> Combat Movement (Type 1)") || AIAnimator.GetAnimatorTransitionInfo(0).IsName("Movement -> Combat Movement (Type 2)") ||
                AIAnimator.GetAnimatorTransitionInfo(0).IsName("Combat Movement (Type 1) -> Put Away Weapon (Type 1)") || AIAnimator.GetAnimatorTransitionInfo(0).IsName("Combat Movement (Type 2) -> Put Away Weapon (Type 2)") ||
                AIAnimator.GetAnimatorTransitionInfo(0).IsName("Put Away Weapon (Type 1) -> Movement") || AIAnimator.GetAnimatorTransitionInfo(0).IsName("Put Away Weapon (Type 2) -> Movement");

            if (IsIdling) CurrentAnimationState = AnimationStateTypes.Idling;
            if (IsMoving) CurrentAnimationState = AnimationStateTypes.Moving;
            if (IsTurningLeft) CurrentAnimationState = AnimationStateTypes.TurningLeft;
            if (IsTurningRight) CurrentAnimationState = AnimationStateTypes.TurningRight;
            if (IsEquipping) CurrentAnimationState = AnimationStateTypes.Equipping;
            if (IsBlocking) CurrentAnimationState = AnimationStateTypes.Blocking;
            if (IsRecoiling) CurrentAnimationState = AnimationStateTypes.Recoiling;
            if (IsStunned) CurrentAnimationState = AnimationStateTypes.Stunned;
            if (IsStrafing) CurrentAnimationState = AnimationStateTypes.Strafing;
            if (IsDodging) CurrentAnimationState = AnimationStateTypes.Dodging;
            if (IsBackingUp) CurrentAnimationState = AnimationStateTypes.BackingUp;
            if (IsAttacking) CurrentAnimationState = AnimationStateTypes.Attacking;
            if (IsGettingHit) CurrentAnimationState = AnimationStateTypes.GettingHit;
            if (IsDead) CurrentAnimationState = AnimationStateTypes.Dead;
            if (IsEmoting) CurrentAnimationState = AnimationStateTypes.Emoting;
            if (IsSwitchingWeapons) CurrentAnimationState = AnimationStateTypes.SwitchingWeapons;
        }

        public void ResetSettings()
        {
            //Reapply the AI's Animator Controller settings applied on Start because, when the
            //Animator Controller is disabled, they're reset to their default settings. 
            SetWeaponTypeAnimationState();
            AIAnimator.SetBool("Idle Active", false);
            InitializeWeaponTypeAnimationAndSettings();
        }

        /// <summary>
        /// Sets up all Animator related settings.
        /// </summary>
        public void SetupAnimator ()
        {
            AIAnimator = GetComponentInChildren<Animator>();
            EmeraldComponent = GetComponentInParent<EmeraldSystem>(); ;

            if (AIAnimator.layerCount >= 2)
                AIAnimator.SetLayerWeight(1, 1);

            if (GetComponent<EmeraldMovement>().MovementType == EmeraldMovement.MovementTypes.RootMotion)
            {
                EmeraldComponent.m_NavMeshAgent.speed = 0;
                AIAnimator.applyRootMotion = true;
            }
            else
            {
                AIAnimator.applyRootMotion = false;
            }

            if (AIAnimator.layerCount >= 2)
            {
                AIAnimator.SetLayerWeight(1, 1);
            }

            SetWeaponTypeAnimationState();

            AIAnimator.SetInteger("Idle Index", Random.Range(0, m_AnimationProfile.NonCombatAnimations.IdleList.Count));
        }

        /// <summary>
        /// Sets up all Animator related weapon type settings.
        /// </summary>
        public void InitializeWeaponTypeAnimationAndSettings()
        {
            if (EmeraldComponent.CombatComponent.WeaponTypeAmount == EmeraldCombat.WeaponTypeAmounts.Two)
            {
                if (EmeraldComponent.CombatComponent.StartingWeaponType == EmeraldCombat.WeaponTypes.Type1)
                {
                    AIAnimator.SetInteger("Weapon Type State", 1);
                    EmeraldComponent.CombatComponent.CurrentWeaponType = EmeraldCombat.WeaponTypes.Type1;
                    EmeraldComponent.DetectionComponent.PickTargetType = EmeraldComponent.CombatComponent.Type1PickTargetType;
                }
                else if (EmeraldComponent.CombatComponent.StartingWeaponType == EmeraldCombat.WeaponTypes.Type2)
                {
                    AIAnimator.SetInteger("Weapon Type State", 2);
                    EmeraldComponent.CombatComponent.CurrentWeaponType = EmeraldCombat.WeaponTypes.Type2;
                    EmeraldComponent.DetectionComponent.PickTargetType = EmeraldComponent.CombatComponent.Type2PickTargetType;
                }
            }
            else
            { 
                AIAnimator.SetInteger("Weapon Type State", 1);
                EmeraldComponent.CombatComponent.CurrentWeaponType = EmeraldCombat.WeaponTypes.Type1;
                EmeraldComponent.DetectionComponent.PickTargetType = EmeraldComponent.CombatComponent.Type1PickTargetType;
            }
        }

        /// <summary>
        /// Controls whether or not Animate Weapon State will be enabled (depending on if the user has applied the equipping and uneuipping animations).
        /// </summary>
        void SetWeaponTypeAnimationState ()
        {
            if (EmeraldComponent.CombatComponent.WeaponTypeAmount == EmeraldCombat.WeaponTypeAmounts.One)
            {
                if (m_AnimationProfile.Type1Animations.PutAwayWeapon.AnimationClip == null || m_AnimationProfile.Type1Animations.PullOutWeapon.AnimationClip == null)
                    AIAnimator.SetBool("Animate Weapon State", false);
                else if (m_AnimationProfile.Type1Animations.PutAwayWeapon.AnimationClip != null && m_AnimationProfile.Type1Animations.PullOutWeapon.AnimationClip != null)
                    AIAnimator.SetBool("Animate Weapon State", true);
            }
            else if (EmeraldComponent.CombatComponent.WeaponTypeAmount == EmeraldCombat.WeaponTypeAmounts.Two)
            {
                if (m_AnimationProfile.Type1Animations.PutAwayWeapon.AnimationClip == null && m_AnimationProfile.Type1Animations.PullOutWeapon.AnimationClip == null && 
                    m_AnimationProfile.Type2Animations.PutAwayWeapon.AnimationClip == null && m_AnimationProfile.Type2Animations.PullOutWeapon.AnimationClip == null)
                    AIAnimator.SetBool("Animate Weapon State", false);
                else if (m_AnimationProfile.Type1Animations.PutAwayWeapon.AnimationClip != null && m_AnimationProfile.Type1Animations.PullOutWeapon.AnimationClip != null && 
                    m_AnimationProfile.Type2Animations.PutAwayWeapon.AnimationClip != null && m_AnimationProfile.Type2Animations.PullOutWeapon.AnimationClip != null)
                    AIAnimator.SetBool("Animate Weapon State", true);
            }
        }

        /// <summary>
        /// Generates a random idle animation index and plays an idle animation.
        /// </summary>
        public void PlayIdleAnimation ()
        {
            if (!EmeraldComponent.AnimationComponent.m_IdleAnimaionIndexOverride && m_AnimationProfile.NonCombatAnimations.IdleList.Count > 0 &&
                EmeraldComponent.MovementComponent.WaypointType != EmeraldMovement.WaypointTypes.Loop)
            {
                AIAnimator.SetInteger("Idle Index", Random.Range(1, m_AnimationProfile.NonCombatAnimations.IdleList.Count+1));
                AIAnimator.SetBool("Idle Active", true);
            }
        }

        /// <summary>
        /// Plays an AI's Warning animation for the current Weapon Type.
        /// </summary>
        public void PlayWarningAnimation ()
        {
            if (EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type1 && m_AnimationProfile.Type1Animations.IdleWarning.AnimationClip == null || 
                EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type2 && m_AnimationProfile.Type2Animations.IdleWarning.AnimationClip == null || 
                WarningAnimationTriggered)
                return;

            AIAnimator.SetTrigger("Warning");
            WarningAnimationTriggered = true;
        }

        /// <summary>
        /// Plays a stunned animation depending on the StunnedLength.
        /// </summary>
        public void PlayStunnedAnimation (float StunnedLength)
        {
            if (EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type1 && m_AnimationProfile.Type1Animations.Stunned.AnimationClip == null || 
                EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type2 && m_AnimationProfile.Type2Animations.Stunned.AnimationClip == null) return;

            if (!IsStunned && !AIAnimator.GetBool("Blocking") && !AIAnimator.GetBool("Dodge Triggered") && !IsDodging && transform.localScale != Vector3.one * 0.003f)
            {
                if (StunnedCoroutine != null) StopCoroutine(StunnedCoroutine);
                StunnedCoroutine = StartCoroutine(SetStunned(StunnedLength));
            }
        }

        IEnumerator SetStunned(float StunnedLength)
        {
            yield return new WaitForSeconds(0.5f);
            if (IsDodging) yield break; //If this AI is doding, don't trigger a stun.
            AIAnimator.SetBool("Stunned Active", true);
            yield return new WaitForSeconds(StunnedLength);
            AIAnimator.SetBool("Stunned Active", false);
            EmeraldComponent.BehaviorsComponent.IsAiming = false;
        }

        /// <summary>
        /// Plays a random death animation from the AI's DeathList. If either DeathList is empty, it is assumed ragdoll deaths are being used.
        /// </summary>
        public void PlayDeathAnimation ()
        {
            //Only play a death animation if the current weapon type death animation lists have animations in them.
            if (EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type1 && m_AnimationProfile.Type1Animations.DeathList.Count == 0 ||
                EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type2 && m_AnimationProfile.Type2Animations.DeathList.Count == 0)
                return;

            if (EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type1)
            {
                AIAnimator.SetInteger("Death Index", Random.Range(1, m_AnimationProfile.Type1Animations.DeathList.Count + 1));
                int DeathIndex = AIAnimator.GetInteger("Death Index");
                StartCoroutine(DisableAnimator(m_AnimationProfile.Type1Animations.DeathList[DeathIndex-1].AnimationClip.length / m_AnimationProfile.Type1Animations.DeathList[DeathIndex - 1].AnimationSpeed));
            }
            else if (EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type2)
            {
                AIAnimator.SetInteger("Death Index", Random.Range(1, m_AnimationProfile.Type2Animations.DeathList.Count + 1));
                int DeathIndex = AIAnimator.GetInteger("Death Index");
                StartCoroutine(DisableAnimator(m_AnimationProfile.Type2Animations.DeathList[DeathIndex-1].AnimationClip.length / m_AnimationProfile.Type2Animations.DeathList[DeathIndex - 1].AnimationSpeed));
            }

            AIAnimator.SetTrigger("Dead");
        }

        /// <summary>
        /// Plays a hit animation depending on the user set Animation State Conditions.
        /// </summary>
        public void PlayHitAnimation ()
        {
            //Get the current hit animation cooldown depending on the weapon type.
            float CurrentHitAnimationCooldown = EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type1 ? m_AnimationProfile.Type1HitAnimationCooldown : m_AnimationProfile.Type2HitAnimationCooldown;

            //Don't play a hit animation if an AI is dead or if the CurrentHitAnimationCooldown hasn't passed.
            if (EmeraldComponent.HealthComponent.CurrentHealth <= 0 || Time.time < (LastHitTime + CurrentHitAnimationCooldown))
                return;

            LastHitTime = Time.time;

            if (!EmeraldComponent.CombatComponent.CombatState)
            {
                if (m_AnimationProfile.NonCombatAnimations.HitList.Count == 0 && !IsBlocking)
                    return;

                int CurrentIndex = AIAnimator.GetInteger("Hit Index");
                AIAnimator.SetInteger("Hit Index", CurrentIndex);
                CurrentIndex++;
                if (CurrentIndex == m_AnimationProfile.NonCombatAnimations.HitList.Count + 1) CurrentIndex = 1;
                AIAnimator.SetInteger("Hit Index", CurrentIndex);
            }
            else if (EmeraldComponent.CombatComponent.CombatState && !IsBlocking)
            {
                if (EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type1)
                {
                    if (m_AnimationProfile.Type1Animations.HitList.Count == 0 && !IsBlocking)
                        return;

                    int CurrentIndex = AIAnimator.GetInteger("Hit Index");
                    AIAnimator.SetInteger("Hit Index", CurrentIndex);
                    CurrentIndex++;
                    if (CurrentIndex >= m_AnimationProfile.Type1Animations.HitList.Count + 1) CurrentIndex = 1;
                    AIAnimator.SetInteger("Hit Index", CurrentIndex);
                }
                else if (EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type2)
                {
                    if (m_AnimationProfile.Type2Animations.HitList.Count == 0 && !IsBlocking)
                        return;

                    int CurrentIndex = AIAnimator.GetInteger("Hit Index");
                    AIAnimator.SetInteger("Hit Index", CurrentIndex);
                    CurrentIndex++;
                    if (CurrentIndex >= m_AnimationProfile.Type2Animations.HitList.Count + 1) CurrentIndex = 1;
                    AIAnimator.SetInteger("Hit Index", CurrentIndex);
                }
            }

            //This helps cancel block if an AI doesn't block in time
            if (!IsBlocking && AIAnimator.GetBool("Blocking"))
            {
                AIAnimator.SetBool("Blocking", false);
            }

            //Only play a hit animation if the conditions are right. Some states are automatically excluded.
            if (!IsDodging && !IsSwitchingWeapons && !IsEquipping && !AIAnimator.GetBool("Dodge Triggered") && EmeraldComponent.HealthComponent.CurrentActiveEffects.Count == 0)
            {
                var Type1Conditions = (((int)m_AnimationProfile.Type1HitConditions) & ((int)CurrentAnimationState)) != 0;
                var Type2Conditions = (((int)m_AnimationProfile.Type2HitConditions) & ((int)CurrentAnimationState)) != 0;

                if (Type1Conditions && EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type1 || Type2Conditions && EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type2)
                {
                    AIAnimator.SetTrigger("Hit");
                    OnGetHit?.Invoke();
                }
            }

            AIAnimator.ResetTrigger("Attack");
        }

        /// <summary>
        /// Plays an attack animation depending on the EmeraldComponent.CombatComponent.CurrentAnimationIndex.
        /// </summary>
        public void PlayAttackAnimation ()
        {
            if (!EmeraldComponent.CombatComponent.CurrentAttackData.CooldownIgnored) EmeraldComponent.CombatComponent.CurrentAttackData.CooldownTimeStamp = Time.time;
            AIAnimator.SetInteger("Attack Index", EmeraldComponent.CombatComponent.CurrentAnimationIndex + 1);
            AIAnimator.SetTrigger("Attack");
        }

        /// <summary>
        /// Calculates which turn animations to use while stationary.
        /// </summary>
        public void CalculateTurnAnimations(bool ByPassConditions = false)
        {
            //Lock turning, as soon as the angle threshold is met, for 1 second. This prevents an AI from getting stuck transitioning between two turning animations.
            if (!EmeraldComponent.CombatComponent.CombatState && EmeraldComponent.MovementComponent.DestinationAdjustedAngle <= EmeraldComponent.MovementComponent.AngleToTurn && !EmeraldComponent.MovementComponent.LockTurning)
            {
                EmeraldComponent.MovementComponent.LockTurning = true;
                StartCoroutine(LockTurns());
                DisableTurning();
            }

            Vector3 DestinationDirection = EmeraldComponent.MovementComponent.DestinationDirection;

            if (ByPassConditions || CanPlayTurningAnimation(DestinationDirection))
            {
                if (Time.timeSinceLevelLoad < 1f || EmeraldComponent.MovementComponent.LockTurning && !EmeraldComponent.CombatComponent.CombatState || IsBackingUp)
                    return;

                Vector3 cross = Vector3.Cross(transform.forward, Quaternion.LookRotation(DestinationDirection, Vector3.up) * Vector3.forward);

                if (cross.y > 0.0f) //Right
                {
                    EmeraldComponent.AnimationComponent.IsTurning = true;
                    EmeraldComponent.AnimationComponent.IsTurningRight = true;
                    EmeraldComponent.AnimationComponent.IsTurningLeft = false;
                    AIAnimator.SetBool("Idle Active", false);
                    AIAnimator.SetBool("Turn Right", true);
                    AIAnimator.SetBool("Turn Left", false);
                }
                else if (cross.y < 0.0f) //Left
                {
                    EmeraldComponent.AnimationComponent.IsTurning = true;
                    EmeraldComponent.AnimationComponent.IsTurningLeft = true;
                    EmeraldComponent.AnimationComponent.IsTurningRight = false;
                    AIAnimator.SetBool("Idle Active", false);
                    AIAnimator.SetBool("Turn Left", true);
                    AIAnimator.SetBool("Turn Right", false);
                }

            }
            else if (EmeraldComponent.CombatComponent.CombatState)
            {
                EmeraldComponent.AnimationComponent.IsTurning = false;
                EmeraldComponent.AnimationComponent.IsTurningLeft = false;
                EmeraldComponent.AnimationComponent.IsTurningRight = false;
                AIAnimator.SetBool("Turn Left", false);
                AIAnimator.SetBool("Turn Right", false);
            }
        }

        /// <summary>
        /// Returns true if a turn animation can be played.
        /// </summary>
        bool CanPlayTurningAnimation (Vector3 DestinationDirection)
        {
            if (!EmeraldComponent.CombatComponent.CombatState)
            {
                return EmeraldComponent.MovementComponent.DestinationAdjustedAngle >= EmeraldComponent.MovementComponent.AngleToTurn && DestinationDirection != Vector3.zero && 
                    EmeraldComponent.MovementComponent.AIAgentActive && EmeraldComponent.m_NavMeshAgent.remainingDistance > EmeraldComponent.m_NavMeshAgent.stoppingDistance;
            }
            else
            {
                return !EmeraldComponent.CombatComponent.DeathDelayActive && EmeraldComponent.MovementComponent.DestinationAdjustedAngle >= EmeraldComponent.MovementComponent.AngleToTurn && DestinationDirection != Vector3.zero && 
                    EmeraldComponent.MovementComponent.AIAgentActive && !IsAttacking && !IsBlocking && !IsGettingHit && !IsRecoiling && !IsStrafing && !IsDodging && !IsStunned && !IsSwitchingWeapons && !IsEquipping;
            }
        }

        /// <summary>
        /// Lock turning, as soon as the angle threshold is met, for 1 second. This prevents an AI from getting stuck transitioning between two turning animations.
        /// </summary>
        IEnumerator LockTurns()
        {
            yield return new WaitForSeconds(1f);
            EmeraldComponent.MovementComponent.LockTurning = false;
        }

        void DisableTurning()
        {
            IsTurning = false;
            IsTurningLeft = false;
            IsTurningRight = false;
            AIAnimator.SetBool("Turn Right", false);
            AIAnimator.SetBool("Turn Left", false);
        }

        /// <summary>
        /// Plays a recoil animation when an AI is attacking and their target blocks.
        /// </summary>
        public void PlayRecoilAnimation ()
        {
            if (EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type1 && m_AnimationProfile.Type1Animations.Recoil.AnimationClip == null || 
                EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type2 && m_AnimationProfile.Type2Animations.Recoil.AnimationClip == null) return;

            if (EmeraldComponent.CombatTarget != null && EmeraldComponent.CurrentTargetInfo != null && EmeraldComponent.CurrentTargetInfo.CurrentICombat.IsBlocking())
            {
                AIAnimator.ResetTrigger("Attack");
                AIAnimator.SetTrigger("Recoil");
                OnRecoil?.Invoke();
            }
        }

        /// <summary>
        /// Sets the Strafe State according to the passed state bool parameter.
        /// </summary>
        public void SetStrafeState (bool State)
        {
            int Direction = Random.Range(0, 2);

            if (EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type1)
            {
                if (Direction == 0 && m_AnimationProfile.Type1Animations.StrafeLeft.AnimationClip == null) return;
                if (Direction == 1 && m_AnimationProfile.Type1Animations.StrafeRight.AnimationClip == null) return;
            }
            else if(EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type1)
            {
                if (Direction == 0 && m_AnimationProfile.Type2Animations.StrafeLeft.AnimationClip == null) return;
                if (Direction == 1 && m_AnimationProfile.Type2Animations.StrafeRight.AnimationClip == null) return;
            }

            AIAnimator.SetBool("Strafe Active", State);
            if (State) AIAnimator.SetInteger("Strafe Direction", Direction);
            if (State) AIAnimator.SetTrigger("Strafing Triggered");
        }

        /// <summary>
        /// Sets the Strafe State according to the passed state bool parameter.
        /// </summary>
        public void TriggerDodgeState()
        {
            int Direction = Random.Range(0, 3);

            //Return if the chosen dodge animation is empty.
            if (EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type1)
            {
                if (Direction == 0 && m_AnimationProfile.Type1Animations.DodgeLeft.AnimationClip == null) return;
                if (Direction == 1 && m_AnimationProfile.Type1Animations.DodgeBack.AnimationClip == null) return;
                if (Direction == 2 && m_AnimationProfile.Type1Animations.DodgeRight.AnimationClip == null) return;
            }
            else if(EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type2)
            {
                if (Direction == 0 && m_AnimationProfile.Type2Animations.DodgeLeft.AnimationClip == null) return;
                if (Direction == 1 && m_AnimationProfile.Type2Animations.DodgeBack.AnimationClip == null) return;
                if (Direction == 2 && m_AnimationProfile.Type2Animations.DodgeRight.AnimationClip == null) return;
            }

            AIAnimator.SetInteger("Dodge Direction", Direction);
            AIAnimator.SetTrigger("Dodge Triggered");
            AIAnimator.SetBool("Walk Backwards", false);
        }

        /// <summary>
        /// Sets the Block State according to the passed state bool parameter.
        /// </summary>
        public void PlayBlockAnimation (bool State)
        {
            if (EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type1 && m_AnimationProfile.Type1Animations.BlockIdle.AnimationClip == null || 
                EmeraldComponent.CombatComponent.CurrentWeaponType == EmeraldCombat.WeaponTypes.Type2 && m_AnimationProfile.Type2Animations.BlockIdle.AnimationClip == null) return;
            AIAnimator.SetBool("Blocking", State);
        }

        /// <summary>
        /// Resets all deafult Animator values before initializing another action. This is to prevent multiple triggers being active at once, which can cause actions to be missed or skipped.
        /// </summary>
        public void ResetTriggers(float Delay)
        {
            StartCoroutine(ResetTriggersInternal(Delay));
        }

        IEnumerator ResetTriggersInternal(float Delay)
        {
            yield return new WaitForSeconds(Delay);
            EmeraldComponent.AIAnimator.ResetTrigger("Hit");
            EmeraldComponent.AIAnimator.ResetTrigger("Attack");
            EmeraldComponent.AIAnimator.SetBool("Blocking", false);
            EmeraldComponent.AIAnimator.ResetTrigger("Dodge Triggered");
            EmeraldComponent.AIAnimator.ResetTrigger("Strafing Triggered");
            EmeraldComponent.AIAnimator.SetBool("Strafe Active", false);
            EmeraldComponent.AIAnimator.ResetTrigger("Attack Cancelled");
        }

        /// <summary>
        /// Returns the Animator back to its default (non-combat) state. This is called through the OnExitCombat callback.
        /// </summary>
        void ReturnToDefaultState ()
        {
            EmeraldComponent.AIAnimator.SetBool("Combat State Active", false);
            EmeraldComponent.AIAnimator.SetBool("Strafe Active", false);
            EmeraldComponent.AnimationComponent.WarningAnimationTriggered = false;
        }

        /// <summary>
        /// Plays an emote animation according to the Animation Clip parameter. Note: This function will only work if
        /// an AI is not in active combat mode.
        /// </summary>
        public void PlayEmoteAnimation(int EmoteAnimationID)
        {
            //Look through each animation in the EmoteAnimationList for the appropriate ID.
            //Once found, play the animaition of the same index as the found ID.
            for (int i = 0; i < m_AnimationProfile.EmoteAnimationList.Count; i++)
            {
                if (m_AnimationProfile.EmoteAnimationList[i].AnimationID == EmoteAnimationID)
                {
                    if (!EmeraldComponent.CombatComponent.CombatState)
                    {
                        AIAnimator.SetInteger("Emote Index", EmoteAnimationID);
                        AIAnimator.SetTrigger("Emote Trigger");
                        IsMoving = false;
                    }
                }
            }
        }

        /// <summary>
        /// Loops an emote animation according to the Animation Clip parameter until it is called to stop. Note: This function will only work if
        /// an AI is not in active combat mode.
        /// </summary>
        public void LoopEmoteAnimation(int EmoteAnimationID)
        {
            //Look through each animation in the EmoteAnimationList for the appropriate ID.
            //Once found, play the animaition of the same index as the found ID.
            for (int i = 0; i < m_AnimationProfile.EmoteAnimationList.Count; i++)
            {
                if (m_AnimationProfile.EmoteAnimationList[i].AnimationID == EmoteAnimationID)
                {
                    if (!EmeraldComponent.CombatComponent.CombatState)
                    {
                        AIAnimator.SetInteger("Emote Index", EmoteAnimationID);
                        AIAnimator.SetBool("Emote Loop", true);
                        IsMoving = false;
                    }
                }
            }
        }

        /// <summary>
        /// Loops an emote animation according to the Animation Clip parameter until it is called to stop. Note: This function will only work if
        /// an AI is not in active combat mode.
        /// </summary>
        public void StopLoopEmoteAnimation(int EmoteAnimationID)
        {
            //Look through each animation in the EmoteAnimationList for the appropriate ID.
            //Once found, play the animaition of the same index as the found ID.
            for (int i = 0; i < m_AnimationProfile.EmoteAnimationList.Count; i++)
            {
                if (m_AnimationProfile.EmoteAnimationList[i].AnimationID == EmoteAnimationID)
                {
                    if (!EmeraldComponent.CombatComponent.CombatState)
                    {
                        AIAnimator.SetInteger("Emote Index", EmoteAnimationID);
                        AIAnimator.SetBool("Emote Loop", false);
                        IsMoving = false;
                    }
                }
            }
        }

        /// <summary>
        /// Delays the call to disable the Emerald AI components until after the death animation has finished playing.
        /// </summary>
        IEnumerator DisableAnimator(float AnimationLength)
        {
            yield return new WaitForSeconds(AnimationLength);
            EmeraldComponent.AIAnimator.enabled = false;
        }
    }
}