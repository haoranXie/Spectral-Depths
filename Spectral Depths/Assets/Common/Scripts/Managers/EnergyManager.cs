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

namespace SpectralDepths.TopDown
{
    /// <summary>
    /// Handles everything related with the use of energy
    /// </summary>

    public class EnergyManager : PLSingleton<EnergyManager>, PLEventListener<TopDownEngineEvent>, PLEventListener<EnergyEvent>
    {
        [PLInformation("The EnergyManager is responsible for the using, tracking, and manipulating Energy",PLInformationAttribute.InformationType.Info,false)]
        [Header("Settings")]
		[Tooltip("Linked Energy Bar")]
        public PLProgressBar EnergyBar;
		public int StartingEnergy = 100;
        public int CurrentEnergy = 100;
        public int Energy { get => CurrentEnergy; set => CurrentEnergy = value; }
        public int StartEnergy { get => StartingEnergy; set => StartingEnergy = value; }
		[Header("Overdrive VFX")]
		[Tooltip("The Fullscreen effect")]
		public ScriptableRendererFeature FullScreenOverdrive;
		[Tooltip("The Fullscreen effect material")]
		public Material FullScreenOverdriveMaterial;
		[Tooltip("Start Itensity of the voranoi")]
		public float VoranoiItensityStartAmount = 2.5f;
		[Tooltip("Start Itensity of the vignette")]
		public float VignetteItensityStartAmount = 1.25f;
		[Tooltip("How long until overdrive fades out")]
		public float OverdriveFadeOutTime = 0.5f;
		[Tooltip("How long until overdrive fades out")]
		public AudioSource audioSource;
		[Tooltip("Play Overdrive Clip")]
		public AudioClip PlayOverdriveClip;
		[Tooltip("Mid Overdrive Clip")]
		public AudioClip MidOverDriveClip;
		[Tooltip("Stop Overdrive Clip")]
		public AudioClip StopOverdriveClip;
		private int _voranoiIntensity = Shader.PropertyToID("_VoranoiIntensity");
		private int _vignetteItensity = Shader.PropertyToID("_VignetteItensity");
        Coroutine FullScreenEffectCoroutine;
        private bool _isPaused = false;
        private Character _controlledCharacter;
        public float elapsedTime;
        protected override void Awake ()
        {
            base.Awake();
            CurrentEnergy = StartingEnergy;
        }

        void Update()
        {
            HandleInput();
        }

        void HandleInput()
        {
            if(_isPaused) return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(_controlledCharacter.GetComponent<CharacterAbilityOverdrive>().OverdriveCost<=CurrentEnergy)
                {
                    OverdriveCharachter();
                }
            }
        }

        private void OverdriveCharachter()
        {
            UseEnergy(_controlledCharacter.GetComponent<CharacterAbilityOverdrive>().OverdriveCost);
            ResetOverdrive();
            FullScreenEffectCoroutine = StartCoroutine(OverdriveEffect(_controlledCharacter.GetComponent<CharacterAbilityOverdrive>().OverdriveLength));
            if(PlayOverdriveClip!=null){audioSource.PlayOneShot(PlayOverdriveClip);}
            _controlledCharacter.GetComponent<CharacterAbilityOverdrive>().Overdrive();
        }


        public void UpdateEnergy(int MaxEnergy, int CurrentEnergy)
        {
            StartingEnergy = MaxEnergy;
            EnergyEvent.Trigger(EnergyEventTypes.EnergyChanged, null, Energy);
        }

        public void UseEnergy(int UsedAmountOfEnergy)
        {
            if(Energy-UsedAmountOfEnergy<0){ EnergyEvent.Trigger(EnergyEventTypes.InsufficientAmountofEnergy, null, Energy); return; }
            Energy-=UsedAmountOfEnergy;
            UpdateEnergy(StartEnergy, Energy);
        }

        public void RestoreEnergy(int RestoredAmountOfEnergy)
        {
            if(Energy+RestoredAmountOfEnergy>StartEnergy){return; }
            Energy+=RestoredAmountOfEnergy;
            UpdateEnergy(StartEnergy, Energy);
        }

		private IEnumerator OverdriveEffect(float OverdriveLength)
        {
            if(FullScreenEffectCoroutine!=null){StopCoroutine(FullScreenEffectCoroutine);}
            PlayWithFadeIn(MidOverDriveClip,1f);
			FullScreenOverdriveMaterial.SetFloat(_voranoiIntensity, 0);
			FullScreenOverdriveMaterial.SetFloat(_vignetteItensity, 0);
			FullScreenOverdrive.SetActive(true);
			elapsedTime = 0f;
			while(elapsedTime <= OverdriveFadeOutTime)
			{
				elapsedTime += Time.unscaledDeltaTime;

                float lerpedVoranoi = Mathf.Lerp(0f, VoranoiItensityStartAmount, (elapsedTime/OverdriveFadeOutTime));
                float lerpedVignette = Mathf.Lerp(0f, VignetteItensityStartAmount, (elapsedTime/OverdriveFadeOutTime));

                FullScreenOverdriveMaterial.SetFloat(_voranoiIntensity, lerpedVoranoi);
                FullScreenOverdriveMaterial.SetFloat(_vignetteItensity, lerpedVignette);

				yield return null;
			}
            elapsedTime = 0f;
			while(elapsedTime <= OverdriveLength-OverdriveFadeOutTime)
			{
				elapsedTime += Time.unscaledDeltaTime;

				yield return null;
			}
            FullScreenEffectCoroutine = StartCoroutine(FadeOutOverdriveEffect());
			//We wait for the overdrive to finish
		}

        public IEnumerator FadeOutOverdriveEffect()
        {
			elapsedTime = 0f;
			while(elapsedTime <= OverdriveFadeOutTime)
			{
				elapsedTime += Time.unscaledDeltaTime;

                float lerpedVoranoi = Mathf.Lerp(VoranoiItensityStartAmount, 0f, (elapsedTime/OverdriveFadeOutTime));
                float lerpedVignette = Mathf.Lerp(VignetteItensityStartAmount, 0f, (elapsedTime/OverdriveFadeOutTime));

                FullScreenOverdriveMaterial.SetFloat(_voranoiIntensity, lerpedVoranoi);
                FullScreenOverdriveMaterial.SetFloat(_vignetteItensity, lerpedVignette);

				yield return null;
			}
            if(StopOverdriveClip!=null){audioSource.PlayOneShot(StopOverdriveClip);}
            FullScreenOverdrive.SetActive(false);
        }

        public virtual void OnMMEvent(TopDownEngineEvent engineEvent)
        {
            switch (engineEvent.EventType)
            {
                case TopDownEngineEventTypes.TurnOffOverdrive:
                    ResetOverdrive();
                    break;
                case TopDownEngineEventTypes.NewControlledCharacter:
                    _controlledCharacter = engineEvent.OriginCharacter;
                    break;
                case TopDownEngineEventTypes.NotControlledCharacter:
                    _controlledCharacter = null;
                    break;
            }       
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

        public void ResetOverdrive()
        {
            if(FullScreenEffectCoroutine!=null){StopCoroutine(FullScreenEffectCoroutine);FullScreenEffectCoroutine = StartCoroutine(FadeOutOverdriveEffect());}
            PLTimeScaleEvent.Trigger(PLTimeScaleMethods.Unfreeze, 1f, 0f, false, 0f, false);
            
            if(_controlledCharacter!=null){
                if(_controlledCharacter.GetComponent<CharacterAbilityOverdrive>()._overdrived){
                    return;
                } 
            }
            
            ResetSound();
        }
		protected virtual void OnEnable()
		{
			this.PLEventStartListening<TopDownEngineEvent> ();
			this.PLEventStartListening<EnergyEvent> ();
		}

		/// <summary>
		/// OnDisable, we stop listening to events.
		/// </summary>
		protected virtual void OnDisable()
		{
			this.PLEventStopListening<TopDownEngineEvent> ();
			this.PLEventStopListening<EnergyEvent> ();
        }



        public void PlayWithFadeIn(AudioClip clip, float fadeInDuration)
        {
            StartCoroutine(FadeIn(clip, fadeInDuration));
        }
        public void StopWithFadeOut(AudioClip clip, float fadeOutDuration)
        {
            StartCoroutine(FadeOut(clip, fadeOutDuration));
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
                StopWithFadeOut(MidOverDriveClip, 1f);
            }        
        }
    }

}