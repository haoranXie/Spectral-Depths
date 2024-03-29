using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Remoting.Messaging;
using System.Diagnostics.Tracing;
using UnityEngine.Rendering;
using UnityEngine.Pool;
using System.Runtime.CompilerServices;
using EmeraldAI;
using SpectralDepths.Tools;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.Universal;
using SpectralDepths.Feedbacks;
using System;
using TMPro;

namespace SpectralDepths.TopDown
{
    /// <summary>
    /// Handles the usage, logic, and management of the special abilities usable by the manager of an expedition
    /// </summary>

    public class ManagerAbilities : PLSingleton<ManagerAbilities>, PLEventListener<EnergyEvent>, PLEventListener<StrainEvent>
    {
        [PLInformation("ManagerAbilities handles the usage, logic, and management of the special abilities usable by the manager of an expedition",PLInformationAttribute.InformationType.Info,false)]
        [Header("Settings")]
		[Tooltip("Linked CurrentEnergy Bar")]
        public PLProgressBar EnergyBar;
		public int MaxEnergy = 100;
        public int CurrentEnergy = 100;
		[Tooltip("Linked Strain Bar")]
        public PLProgressBar StrainBar;
        public Image StrainBarFrame;
        public TextMeshProUGUI StrainText;
		public float MaxStrain = 100;
        public float CurrentStrain = 0;
		[Tooltip("Time after not using strain before strain begins to decrease")]
        public float StrainCooldownTime = 2.5f;
		[Tooltip("How fast strain decreases after the strain cooldown time is elapsed")]
        public float BaseStrainDecreaseRate = 5f;
        [Header("Strain Ability Settings")]
		[Tooltip("How fast should strategy mode cause strain to increase")]
        public float StrategyModeStrainSpeed = 5f;
		[Header("UI Effects")]
        public Sprite StrainBarFrameDefault;
        public Sprite StrainBarFrameMaxed;
        public Color StrainBarFrameMaxedColor;
		[Header("Strategy Mode Settings")]
		[Tooltip("whether or not to slow time during overdrive")]
		public bool SlowTime = false;
		/// the new timescale to apply
		[PLCondition("SlowTime", true)]
		[Tooltip("the new timescale to apply")]
		public float TimeScale = 0.5f;
		/// the duration to apply the new timescale for
		[PLCondition("SlowTime", true)]
		[Tooltip("the duration to apply the new timescale for")]
		public float Duration = 5f;
		/// whether or not the timescale should be lerped
		[Tooltip("whether or not the timescale should be lerped")]
		[PLCondition("SlowTime", true)]
		public bool LerpTimeScale = true;
		/// the speed at which to lerp the timescale
		[Tooltip("the speed at which to lerp the timescale")]
		[PLCondition("SlowTime", true)]
		public float LerpSpeed = 5f;
		[Header("Strategy Mode VFX")]
		[Tooltip("The Fullscreen effect")]
		public ScriptableRendererFeature FullScreenStrategyMode;
		[Tooltip("The Fullscreen effect material")]
		public Material FullScreenStrategyModeMaterial;
		[Tooltip("Start Itensity of the voranoi")]
		public float VoranoiItensityStartAmount = 3f;
		[Tooltip("Start Itensity of the vignette")]
		public float VignetteItensityStartAmount = 1.25f;
		[Tooltip("How long until overdrive fades out")]
		public float StrategyModeFadeOutTime = 0.5f;
		[Tooltip("How long until overdrive fades out")]
		public AudioSource audioSource;
		[Tooltip("Play Overdrive Clip")]
		public AudioClip PlayOverdriveClip;
		[Tooltip("Mid Overdrive Clip")]
		public AudioClip MidStrategyModeClip;
		[Tooltip("Stop Overdrive Clip")]
		public AudioClip StopOverdriveClip;
		private int _voranoiIntensity = Shader.PropertyToID("_VoranoiIntensity");
		private int _vignetteItensity = Shader.PropertyToID("_VignetteItensity");
        Coroutine FullScreenEffectCoroutine;
        private bool _isPaused = false;
        public float elapsedTime;
        Coroutine SoundCoroutine;
        bool _strategyMode;
        float _strainIncreaseSpeed = 0;
        float _strainDecreaseSpeed = 0;
        Coroutine _strainDecreaseCoroutine;
        bool _regenStrain = false;
        protected override void Awake ()
        {
            base.Awake();
            CurrentEnergy = MaxEnergy;
            _strainDecreaseSpeed = BaseStrainDecreaseRate;
            StrainText.text = 0.ToString();
        }

        void Update()
        {
            HandleInput();
            HandleStrain();
        }

        void HandleInput()
        {
            if(_isPaused) return;

            if (Input.GetKeyDown(KeyCode.Space))
            {

                //If we're entering into Strategy Mode
                if(!_strategyMode)
                {
                    
                    _strategyMode = true;
                    _strainIncreaseSpeed+=StrategyModeStrainSpeed;
                    if(GameRTSController.Instance!=null)
                    {
                        GameRTSController.Instance.ShowAllCharacterIndicators();
                        GameRTSController.Instance.NeverHideIndicators = true;
                    } 
                    if(CurrentStrain<MaxStrain)
                    {
                        StrategyModeOn();                  
                    }
                }
                //If we're exiting Strategy Mode
                else if(_strategyMode)
                {
                    if(GameRTSController.Instance!=null)
                    {
                        GameRTSController.Instance.NeverHideIndicators = false;
                        GameRTSController.Instance.HideAllCharacterIndicators();
                    } 
                    if(Time.timeScale!=1 || FullScreenEffectCoroutine!=null) ResetStrategyMode();
                    _strategyMode = false;
                    _strainIncreaseSpeed-=StrategyModeStrainSpeed;
                }
            }
        }

        void HandleStrain()
        {
            //Strain is not being used
            if(_strainIncreaseSpeed==0)
            {
                if(_strainDecreaseCoroutine == null) _strainDecreaseCoroutine = StartCoroutine(StrainCountdown(StrainCooldownTime));
                if(CurrentStrain>0 && _regenStrain)
                {
                    DecreaseStrain(Time.deltaTime * _strainDecreaseSpeed);
                }
                if(CurrentStrain<MaxStrain)
                {
                    StrainBarFrame.sprite = StrainBarFrameDefault;
                    StrainBarFrame.color = Color.white;
                }
            }
            //If Strain is being used
            else if(_strainIncreaseSpeed>0)
            {
                if(_strainDecreaseCoroutine != null) _strainDecreaseCoroutine = null;
                _regenStrain = false;
                if(CurrentStrain<=MaxStrain)
                {
                    IncreaseStrain(Time.deltaTime * _strainIncreaseSpeed);
                }
                if(CurrentStrain==MaxStrain)
                {
                    if(Time.timeScale!=1) ResetStrategyMode();
                    if(StrainBarFrame.sprite != StrainBarFrameMaxed){StrainBarFrame.sprite = StrainBarFrameMaxed; StrainBarFrame.color = StrainBarFrameMaxedColor;}
                    _strainIncreaseSpeed = 0;
                    _strategyMode = false;
                }
            }
        }   

        void IncreaseStrain(float amount)
        {
            CurrentStrain+=amount;
            CurrentStrain = Mathf.Clamp(CurrentStrain, 0f, MaxStrain);
            StrainBar.UpdateBar(Mathf.RoundToInt(CurrentStrain), 0f, Mathf.RoundToInt(MaxStrain));
            StrainText.text = Mathf.RoundToInt(CurrentStrain).ToString();
        }

        void DecreaseStrain(float amount)
        {
            CurrentStrain-=amount;
            CurrentStrain = Mathf.Clamp(CurrentStrain, 0f, MaxStrain);
            StrainBar.UpdateBar(Mathf.RoundToInt(CurrentStrain), 0f, Mathf.RoundToInt(MaxStrain));
            StrainText.text = Mathf.RoundToInt(CurrentStrain).ToString();
        }

        private void StrategyModeOn()
        {
            if(Time.timeScale!=1 || FullScreenEffectCoroutine!=null) ResetStrategyMode();
            PLTimeScaleEvent.Trigger(PLTimeScaleMethods.For, TimeScale, Duration, LerpTimeScale, LerpSpeed, true); 
            FullScreenEffectCoroutine = StartCoroutine(StrategyModeEffect());
            if(PlayOverdriveClip!=null){audioSource.PlayOneShot(PlayOverdriveClip);}
        }
        /// <summary>
        /// How long it takes before strain begins to decrease after not spending it
        /// </summary>
        IEnumerator StrainCountdown(float timeBeforeStrainDecrease)
        {
            // Fade in
            float timer = 0f;
            while (timer < timeBeforeStrainDecrease)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            _regenStrain = true;
        }

        public void UpdateEnergy(int MaxEnergy, int CurrentEnergy)
        {
            this.MaxEnergy = MaxEnergy;
            EnergyEvent.Trigger(EnergyEventTypes.EnergyChanged, null, CurrentEnergy);
        }

        public void UseEnergy(int UsedAmountOfEnergy)
        {
            if(CurrentEnergy-UsedAmountOfEnergy<0){ EnergyEvent.Trigger(EnergyEventTypes.InsufficientAmountofEnergy, null, CurrentEnergy); return; }
            CurrentEnergy-=UsedAmountOfEnergy;
            UpdateEnergy(MaxEnergy, CurrentEnergy);
        }

        public void RestoreEnergy(int RestoredAmountOfEnergy)
        {
            if(CurrentEnergy+RestoredAmountOfEnergy>MaxEnergy){return; }
            CurrentEnergy+=RestoredAmountOfEnergy;
            UpdateEnergy(MaxEnergy, CurrentEnergy);
        }

		private IEnumerator StrategyModeEffect()
        {
            if(FullScreenEffectCoroutine!=null){StopCoroutine(FullScreenEffectCoroutine);}
            if(SoundCoroutine!=null){StopCoroutine(SoundCoroutine);}
            PlayWithFadeIn(MidStrategyModeClip,1f);
			FullScreenStrategyModeMaterial.SetFloat(_voranoiIntensity, 0);
			FullScreenStrategyModeMaterial.SetFloat(_vignetteItensity, 0);
			FullScreenStrategyMode.SetActive(true);
			elapsedTime = 0f;
			while(elapsedTime <= StrategyModeFadeOutTime)
			{
				elapsedTime += Time.unscaledDeltaTime;

                float lerpedVoranoi = Mathf.Lerp(0f, VoranoiItensityStartAmount, (elapsedTime/StrategyModeFadeOutTime));
                float lerpedVignette = Mathf.Lerp(0f, VignetteItensityStartAmount, (elapsedTime/StrategyModeFadeOutTime));

                FullScreenStrategyModeMaterial.SetFloat(_voranoiIntensity, lerpedVoranoi);
                FullScreenStrategyModeMaterial.SetFloat(_vignetteItensity, lerpedVignette);

				yield return null;
			}
		}

        public IEnumerator FadeOutStrategyModeEffect()
        {
			elapsedTime = 0f;
			while(elapsedTime <= StrategyModeFadeOutTime)
			{
				elapsedTime += Time.unscaledDeltaTime;

                float lerpedVoranoi = Mathf.Lerp(VoranoiItensityStartAmount, 0f, (elapsedTime/StrategyModeFadeOutTime));
                float lerpedVignette = Mathf.Lerp(VignetteItensityStartAmount, 0f, (elapsedTime/StrategyModeFadeOutTime));

                FullScreenStrategyModeMaterial.SetFloat(_voranoiIntensity, lerpedVoranoi);
                FullScreenStrategyModeMaterial.SetFloat(_vignetteItensity, lerpedVignette);

				yield return null;
			}
            if(StopOverdriveClip!=null){audioSource.PlayOneShot(StopOverdriveClip);}
            FullScreenStrategyMode.SetActive(false);
        }
        
        public virtual void OnMMEvent(EnergyEvent engineEvent)
        {
            switch (engineEvent.EventType)
            {
                case EnergyEventTypes.UseEnergy:
                    UseEnergy(engineEvent.AmountOfEnergy);
                    break;
                case EnergyEventTypes.RecoverEnergy:
                    RestoreEnergy(engineEvent.AmountOfEnergy);
                    break;
            }       
        }

        public virtual void OnMMEvent(StrainEvent engineEvent)
        {
            switch (engineEvent.EventType)
            {
                case StrainEventTypes.IncreaseStrain:
                    break;
            }       
        }


        public void ResetStrategyMode()
        {
            if(FullScreenEffectCoroutine!=null){StopCoroutine(FullScreenEffectCoroutine);FullScreenEffectCoroutine = StartCoroutine(FadeOutStrategyModeEffect());}
            if(Time.timeScale!=1)PLTimeScaleEvent.Trigger(PLTimeScaleMethods.Unfreeze, 1f, 0f, false, 0f, false);
            ResetSound();
        }
		protected virtual void OnEnable()
		{
			this.PLEventStartListening<EnergyEvent> ();
			this.PLEventStartListening<StrainEvent> ();
		}

		/// <summary>
		/// OnDisable, we stop listening to events.
		/// </summary>
		protected virtual void OnDisable()
		{
			this.PLEventStopListening<EnergyEvent> ();
			this.PLEventStartListening<StrainEvent> ();
        }



        public void PlayWithFadeIn(AudioClip clip, float fadeInDuration)
        {
            if(SoundCoroutine!=null){StopCoroutine(SoundCoroutine);}
            SoundCoroutine = StartCoroutine(FadeIn(clip, fadeInDuration));
        }
        public void StopWithFadeOut(AudioClip clip, float fadeOutDuration)
        {
            if(SoundCoroutine!=null){StopCoroutine(SoundCoroutine);}
            SoundCoroutine = StartCoroutine(FadeOut(clip, fadeOutDuration));
        }
        IEnumerator FadeIn(AudioClip clip, float fadeInDuration)
        {
            // Fade in
            float timer = 0f;
            audioSource.clip = clip;
            audioSource.Play();
            while (timer < fadeInDuration)
            {
                timer += Time.unscaledDeltaTime;
                audioSource.volume = Mathf.Lerp(0f, 1f, timer / fadeInDuration);
                yield return null;
            }
        }
        IEnumerator FadeOut(AudioClip clip, float fadeOutDuration)
        {
            // Fade out
            float timer = 0f;
            while (timer < fadeOutDuration)
            {
                timer += Time.unscaledDeltaTime;
                audioSource.volume = Mathf.Lerp(1f, 0f, timer / fadeOutDuration);
                yield return null;
            }
            audioSource.Stop();
        }



        public void ResetSound()
        {
            if(audioSource.isPlaying)
            {
                StopWithFadeOut(MidStrategyModeClip, 1f);
            }        
        }
    }

}