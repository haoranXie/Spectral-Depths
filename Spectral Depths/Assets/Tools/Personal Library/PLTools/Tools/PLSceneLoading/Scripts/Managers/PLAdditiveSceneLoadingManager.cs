﻿using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SpectralDepths.Tools;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace SpectralDepths.Tools
{	
	[System.Serializable]
	public class ProgressEvent : UnityEvent<float>{}

	/// <summary>
	/// A simple class used to store additive loading settings
	/// </summary>
	[Serializable]
	public class PLAdditiveSceneLoadingManagerSettings
	{
		
		/// the possible ways to unload scenes
		public enum UnloadMethods { None, ActiveScene, AllScenes };
		
		/// the name of the PLSceneLoadingManager scene you want to use when in additive mode
		[Tooltip("the name of the PLSceneLoadingManager scene you want to use when in additive mode")]
		public string LoadingSceneName = "PLAdditiveLoadingScreen";
		/// when in additive loading mode, the thread priority to apply to the loading
		[Tooltip("when in additive loading mode, the thread priority to apply to the loading")]
		public ThreadPriority ThreadPriority = ThreadPriority.High;
		/// whether or not to make additional sanity checks (better leave this to true)
		[Tooltip("whether or not to make additional sanity checks (better leave this to true)")]
		public bool SecureLoad = true;
		/// when in additive loading mode, whether or not to interpolate the progress bar's progress
		[Tooltip("when in additive loading mode, whether or not to interpolate the progress bar's progress")]
		public bool InterpolateProgress = true;
		/// when in additive loading mode, when in additive loading mode, the duration (in seconds) of the delay before the entry fade
		[Tooltip("when in additive loading mode, when in additive loading mode, the duration (in seconds) of the delay before the entry fade")]
		public float BeforeEntryFadeDelay = 0f;
		/// when in additive loading mode, the duration (in seconds) of the entry fade
		[Tooltip("when in additive loading mode, the duration (in seconds) of the entry fade")]
		public float EntryFadeDuration = 0.25f;
		/// when in additive loading mode, the duration (in seconds) of the delay before the entry fade
		[Tooltip("when in additive loading mode, the duration (in seconds) of the delay before the entry fade")]
		public float AfterEntryFadeDelay = 0.1f;
		/// when in additive loading mode, the duration (in seconds) of the delay before the exit fade
		[Tooltip("when in additive loading mode, the duration (in seconds) of the delay before the exit fade")]
		public float BeforeExitFadeDelay = 0.25f;
		/// when in additive loading mode, the duration (in seconds) of the exit fade
		[Tooltip("when in additive loading mode, the duration (in seconds) of the exit fade")]
		public float ExitFadeDuration = 0.2f;
		/// when in additive loading mode, when in additive loading mode, the tween to use to fade on entry
		[Tooltip("when in additive loading mode, when in additive loading mode, the tween to use to fade on entry")]
		public PLTweenType EntryFadeTween = null;
		/// when in additive loading mode, the tween to use to fade on exit
		[Tooltip("when in additive loading mode, the tween to use to fade on exit")]
		public PLTweenType ExitFadeTween = null;
		/// when in additive loading mode, the speed at which the loader's progress bar should move
		[Tooltip("when in additive loading mode, the speed at which the loader's progress bar should move")]
		public float ProgressBarSpeed = 5f;
		/// when in additive loading mode, the selective additive fade mode
		[Tooltip("when in additive loading mode, the selective additive fade mode")]
		public PLAdditiveSceneLoadingManager.FadeModes FadeMode = PLAdditiveSceneLoadingManager.FadeModes.FadeInThenOut;
		/// the chosen way to unload scenes (none, only the active scene, all loaded scenes)
		[Tooltip("the chosen way to unload scenes (none, only the active scene, all loaded scenes)")]
		public UnloadMethods UnloadMethod = UnloadMethods.AllScenes;
		/// the name of the anti spill scene to use when loading additively.
		/// If left empty, that scene will be automatically created, but you can specify any scene to use for that. Usually you'll want your own anti spill scene to be just an empty scene, but you can customize its lighting settings for example.
		[Tooltip("the name of the anti spill scene to use when loading additively." +
		         "If left empty, that scene will be automatically created, but you can specify any scene to use for that. Usually you'll want your own anti spill scene to be just an empty scene, but you can customize its lighting settings for example.")]
		public string AntiSpillSceneName = "";
	}
	
	/// <summary>
	/// A class to load scenes using a loading screen instead of just the default API
	/// This is a new version of the classic LoadingSceneManager (now renamed to PLSceneLoadingManager for consistency)
	/// </summary>
	public class PLAdditiveSceneLoadingManager : MonoBehaviour 
	{
		/// The possible orders in which to play fades (depends on the fade you've set in your loading screen
		public enum FadeModes { FadeInThenOut, FadeOutThenIn }

		[Header("Audio Listener")] 
		public AudioListener LoadingAudioListener;
		
		[Header("Settings")]
		/// the ID on which to trigger a fade, has to match the ID on the fader in your scene
		[Tooltip("the ID on which to trigger a fade, has to match the ID on the fader in your scene")]
		public int FaderID = 500;
		/// whether or not to output debug messages to the console
		[Tooltip("whether or not to output debug messages to the console")]
		public bool DebugMode = false;

		[Header("Progress Events")] 
		/// an event used to update progress 
		[Tooltip("an event used to update progress")]
		public ProgressEvent SetRealtimeProgressValue;
		/// an event used to update progress with interpolation
		[Tooltip("an event used to update progress with interpolation")]
		public ProgressEvent SetInterpolatedProgressValue;

		[Header("State Events")]
		/// an event that will be invoked when the load starts
		[Tooltip("an event that will be invoked when the load starts")]
		public UnityEvent OnLoadStarted;
		/// an event that will be invoked when the delay before the entry fade starts
		[Tooltip("an event that will be invoked when the delay before the entry fade starts")]
		public UnityEvent OnBeforeEntryFade;
		/// an event that will be invoked when the entry fade starts
		[Tooltip("an event that will be invoked when the entry fade starts")]
		public UnityEvent OnEntryFade;
		/// an event that will be invoked when the delay after the entry fade starts
		[Tooltip("an event that will be invoked when the delay after the entry fade starts")]
		public UnityEvent OnAfterEntryFade;
		/// an event that will be invoked when the origin scene gets unloaded
		[Tooltip("an event that will be invoked when the origin scene gets unloaded")]
		public UnityEvent OnUnloadOriginScene;
		/// an event that will be invoked when the destination scene starts loading
		[Tooltip("an event that will be invoked when the destination scene starts loading")]
		public UnityEvent OnLoadDestinationScene;
		/// an event that will be invoked when the load of the destination scene is complete
		[Tooltip("an event that will be invoked when the load of the destination scene is complete")]
		public UnityEvent OnLoadProgressComplete;
		/// an event that will be invoked when the interpolated load of the destination scene is complete
		[Tooltip("an event that will be invoked when the interpolated load of the destination scene is complete")]
		public UnityEvent OnInterpolatedLoadProgressComplete;
		/// an event that will be invoked when the delay before the exit fade starts
		[Tooltip("an event that will be invoked when the delay before the exit fade starts")]
		public UnityEvent OnBeforeExitFade;
		/// an event that will be invoked when the exit fade starts
		[Tooltip("an event that will be invoked when the exit fade starts")]
		public UnityEvent OnExitFade;
		/// an event that will be invoked when the destination scene gets activated
		[Tooltip("an event that will be invoked when the destination scene gets activated")]
		public UnityEvent OnDestinationSceneActivation;
		/// an event that will be invoked when the scene loader gets unloaded
		[Tooltip("an event that will be invoked when the scene loader gets unloaded")]
		public UnityEvent OnUnloadSceneLoader;

		protected static bool _interpolateProgress;
		protected static float _progressInterpolationSpeed;
		protected static float _beforeEntryFadeDelay;
		protected static PLTweenType _entryFadeTween;
		protected static float _entryFadeDuration;
		protected static float _afterEntryFadeDelay;
		protected static float _beforeExitFadeDelay;
		protected static PLTweenType _exitFadeTween;
		protected static float _exitFadeDuration;
		protected static FadeModes _fadeMode;
		protected static string _sceneToLoadName = "";
		protected static string _loadingScreenSceneName;
		protected static List<string> _scenesInBuild;
		protected static Scene[] _initialScenes;
		protected float _loadProgress = 0f;
		protected float _interpolatedLoadProgress;
		protected static bool _loadingInProgress = false;
		protected AsyncOperation _unloadOriginAsyncOperation;
		protected AsyncOperation _loadDestinationAsyncOperation;
		protected AsyncOperation _unloadLoadingAsyncOperation;
		protected bool _setRealtimeProgressValueIsNull;
		protected bool _setInterpolatedProgressValueIsNull;
		protected const float _asyncProgressLimit = 0.9f;
		protected PLSceneLoadingAntiSpill _antiSpill = new PLSceneLoadingAntiSpill();
		protected static string _antiSpillSceneName = "";

		/// <summary>
		/// Call this static method to load a scene from anywhere (packed settings signature)
		/// </summary>
		/// <param name="sceneToLoadName"></param>
		/// <param name="settings"></param>
		public static void LoadScene(string sceneToLoadName, PLAdditiveSceneLoadingManagerSettings settings)
		{
			LoadScene(sceneToLoadName, settings.LoadingSceneName, settings.ThreadPriority, settings.SecureLoad, settings.InterpolateProgress,
				settings.BeforeEntryFadeDelay, settings.EntryFadeDuration, settings.AfterEntryFadeDelay, settings.BeforeExitFadeDelay,
				settings.ExitFadeDuration, settings.EntryFadeTween, settings.ExitFadeTween, settings.ProgressBarSpeed, settings.FadeMode, settings.UnloadMethod, settings.AntiSpillSceneName);
		}
        
		/// <summary>
		/// Call this static method to load a scene from anywhere
		/// </summary>
		/// <param name="sceneToLoadName">Level name.</param>
		public static void LoadScene(string sceneToLoadName, string loadingSceneName = "PLAdditiveLoadingScreenFastFade", 
			ThreadPriority threadPriority = ThreadPriority.High, bool secureLoad = true,
			bool interpolateProgress = true,
			float beforeEntryFadeDelay = 0f,
			float entryFadeDuration = 0.25f,
			float afterEntryFadeDelay = 0.1f,
			float beforeExitFadeDelay = 0.25f,
			float exitFadeDuration = 0.2f, 
			PLTweenType entryFadeTween = null, PLTweenType exitFadeTween = null,
			float progressBarSpeed = 5f, 
			FadeModes fadeMode = FadeModes.FadeInThenOut,
			PLAdditiveSceneLoadingManagerSettings.UnloadMethods unloadMethod = PLAdditiveSceneLoadingManagerSettings.UnloadMethods.AllScenes,
			string antiSpillSceneName = "")
		{
			if (_loadingInProgress)
			{
				Debug.LogError("PLLoadingSceneManagerAdditive : a request to load a new scene was emitted while a scene load was already in progress");  
				return;
			}

			if (entryFadeTween == null)
			{
				entryFadeTween = new PLTweenType(PLTween.PLTweenCurve.EaseInOutCubic);
			}

			if (exitFadeTween == null)
			{
				exitFadeTween = new PLTweenType(PLTween.PLTweenCurve.EaseInOutCubic);
			}

			if (secureLoad)
			{
				_scenesInBuild = PLScene.GetScenesInBuild();
	            
				if (!_scenesInBuild.Contains(sceneToLoadName))
				{
					Debug.LogError("PLLoadingSceneManagerAdditive : impossible to load the '"+sceneToLoadName+"' scene, " +
					               "there is no such scene in the project's build settings.");
					return;
				}
				if (!_scenesInBuild.Contains(loadingSceneName))
				{
					Debug.LogError("PLLoadingSceneManagerAdditive : impossible to load the '"+loadingSceneName+"' scene, " +
					               "there is no such scene in the project's build settings.");
					return;
				}
			}

			_loadingInProgress = true;
			_initialScenes = GetScenesToUnload(unloadMethod);

			Application.backgroundLoadingPriority = threadPriority;
			_sceneToLoadName = sceneToLoadName;					
			_loadingScreenSceneName = loadingSceneName;
			_beforeEntryFadeDelay = beforeEntryFadeDelay;
			_entryFadeDuration = entryFadeDuration;
			_entryFadeTween = entryFadeTween;
			_afterEntryFadeDelay = afterEntryFadeDelay;
			_progressInterpolationSpeed = progressBarSpeed;
			_beforeExitFadeDelay = beforeExitFadeDelay;
			_exitFadeDuration = exitFadeDuration;
			_exitFadeTween = exitFadeTween;
			_fadeMode = fadeMode;
			_interpolateProgress = interpolateProgress;
			_antiSpillSceneName = antiSpillSceneName;

			SceneManager.LoadScene(_loadingScreenSceneName, LoadSceneMode.Additive);
		}
        
		private static Scene[] GetScenesToUnload(PLAdditiveSceneLoadingManagerSettings.UnloadMethods unloaded)
		{
	        
			switch (unloaded) {
				case PLAdditiveSceneLoadingManagerSettings.UnloadMethods.None:
					_initialScenes = new Scene[0];
					break;
				case PLAdditiveSceneLoadingManagerSettings.UnloadMethods.ActiveScene:
					_initialScenes = new Scene[1] {SceneManager.GetActiveScene()};
					break;
				default:
				case PLAdditiveSceneLoadingManagerSettings.UnloadMethods.AllScenes:
					_initialScenes = PLScene.GetLoadedScenes();
					break;
			}
			return _initialScenes;
		}


		/// <summary>
		/// Starts loading the new level asynchronously
		/// </summary>
		protected virtual void Awake()
		{
			Initialization();
		}

		/// <summary>
		/// Initializes timescale, computes null checks, and starts the load sequence
		/// </summary>
		protected virtual void Initialization()
		{
			PLLoadingSceneDebug("PLLoadingSceneManagerAdditive : Initialization");

			if (DebugMode)
			{
				foreach (Scene scene in _initialScenes)
				{
					PLLoadingSceneDebug("PLLoadingSceneManagerAdditive : Initial scene : " + scene.name);
				}    
			}

			_setRealtimeProgressValueIsNull = SetRealtimeProgressValue == null;
			_setInterpolatedProgressValueIsNull = SetInterpolatedProgressValue == null;
			Time.timeScale = 1f;

			if ((_sceneToLoadName == "") || (_loadingScreenSceneName == ""))
			{
				return;
			}
            
			StartCoroutine(LoadSequence());
		}

		/// <summary>
		/// Every frame, we fill the bar smoothly according to loading progress
		/// </summary>
		protected virtual void Update()
		{
			UpdateProgress();
		}

		/// <summary>
		/// Sends progress value via UnityEvents
		/// </summary>
		protected virtual void UpdateProgress()
		{
			if (!_setRealtimeProgressValueIsNull)
			{
				SetRealtimeProgressValue.Invoke(_loadProgress);
			}

			if (_interpolateProgress)
			{
				_interpolatedLoadProgress = PLMaths.Approach(_interpolatedLoadProgress, _loadProgress, Time.unscaledDeltaTime * _progressInterpolationSpeed);
				if (!_setInterpolatedProgressValueIsNull)
				{
					SetInterpolatedProgressValue.Invoke(_interpolatedLoadProgress);	
				}
			}
			else
			{
				SetInterpolatedProgressValue.Invoke(_loadProgress);	
			}
		}

		/// <summary>
		/// Loads the scene to load asynchronously.
		/// </summary>
		protected virtual IEnumerator LoadSequence()
		{
			_antiSpill?.PrepareAntiFill(_sceneToLoadName, _antiSpillSceneName);
			InitiateLoad();
			yield return ProcessDelayBeforeEntryFade();
			yield return EntryFade();
			yield return ProcessDelayAfterEntryFade();
			yield return UnloadOriginScenes();
			yield return LoadDestinationScene();
			yield return ProcessDelayBeforeExitFade();
			yield return DestinationSceneActivation();
			yield return ExitFade();
			yield return UnloadSceneLoader();
		}

		/// <summary>
		/// Initializes counters and timescale
		/// </summary>
		protected virtual void InitiateLoad()
		{
			_loadProgress = 0f;
			_interpolatedLoadProgress = 0f;
			Time.timeScale = 1f;
			SetAudioListener(false);
			PLLoadingSceneDebug("PLLoadingSceneManagerAdditive : Initiate Load");
			PLSceneLoadingManager.LoadingSceneEvent.Trigger(_sceneToLoadName, PLSceneLoadingManager.LoadingStatus.LoadStarted);
			OnLoadStarted?.Invoke();
		}

		/// <summary>
		/// Waits for the specified BeforeEntryFadeDelay duration
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator ProcessDelayBeforeEntryFade()
		{
			if (_beforeEntryFadeDelay > 0f)
			{
				PLLoadingSceneDebug("PLLoadingSceneManagerAdditive : delay before entry fade, duration : " + _beforeEntryFadeDelay);
				PLSceneLoadingManager.LoadingSceneEvent.Trigger(_sceneToLoadName, PLSceneLoadingManager.LoadingStatus.BeforeEntryFade);
				OnBeforeEntryFade?.Invoke();
				
				yield return PLCoroutine.WaitForUnscaled(_beforeEntryFadeDelay);
			}
		}

		/// <summary>
		/// Calls a fader on entry
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator EntryFade()
		{
			if (_entryFadeDuration > 0f)
			{
				PLLoadingSceneDebug("PLLoadingSceneManagerAdditive : entry fade, duration : " + _entryFadeDuration);
				PLSceneLoadingManager.LoadingSceneEvent.Trigger(_sceneToLoadName, PLSceneLoadingManager.LoadingStatus.EntryFade);
				OnEntryFade?.Invoke();
				
				if (_fadeMode == FadeModes.FadeOutThenIn)
				{
					yield return null;
					PLFadeOutEvent.Trigger(_entryFadeDuration, _entryFadeTween, FaderID, true);
				}
				else
				{
					yield return null;
					PLFadeInEvent.Trigger(_entryFadeDuration, _entryFadeTween, FaderID, true);
				}           

				yield return PLCoroutine.WaitForUnscaled(_entryFadeDuration);
			}
		}

		/// <summary>
		/// Waits for the specified AfterEntryFadeDelay
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator ProcessDelayAfterEntryFade()
		{
			if (_afterEntryFadeDelay > 0f)
			{
				PLLoadingSceneDebug("PLLoadingSceneManagerAdditive : delay after entry fade, duration : " + _afterEntryFadeDelay);
				PLSceneLoadingManager.LoadingSceneEvent.Trigger(_sceneToLoadName, PLSceneLoadingManager.LoadingStatus.AfterEntryFade);
				OnAfterEntryFade?.Invoke();
				
				yield return PLCoroutine.WaitForUnscaled(_afterEntryFadeDelay);
			}
		}

		/// <summary>
		/// Unloads the original scene(s) and waits for the unload to complete
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator UnloadOriginScenes()
		{
			foreach (Scene scene in _initialScenes)
			{
				PLLoadingSceneDebug("PLLoadingSceneManagerAdditive : unload scene " + scene.name);
				PLSceneLoadingManager.LoadingSceneEvent.Trigger(_sceneToLoadName, PLSceneLoadingManager.LoadingStatus.UnloadOriginScene);
				OnUnloadOriginScene?.Invoke();
				
				if (!scene.IsValid() || !scene.isLoaded)
				{
					Debug.LogWarning("PLLoadingSceneManagerAdditive : invalid scene : " + scene.name);
					continue;
				}
				
				_unloadOriginAsyncOperation = SceneManager.UnloadSceneAsync(scene);
				SetAudioListener(true);
				while (_unloadOriginAsyncOperation.progress < _asyncProgressLimit)
				{
					yield return null;
				}
			}
		}

		/// <summary>
		/// Loads the destination scene
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator LoadDestinationScene()
		{
			PLLoadingSceneDebug("PLLoadingSceneManagerAdditive : load destination scene");
			PLSceneLoadingManager.LoadingSceneEvent.Trigger(_sceneToLoadName, PLSceneLoadingManager.LoadingStatus.LoadDestinationScene);
			OnLoadDestinationScene?.Invoke();

			_loadDestinationAsyncOperation = SceneManager.LoadSceneAsync(_sceneToLoadName, LoadSceneMode.Additive );
			_loadDestinationAsyncOperation.completed += OnLoadOperationComplete;

			_loadDestinationAsyncOperation.allowSceneActivation = false;
            
			while (_loadDestinationAsyncOperation.progress < _asyncProgressLimit)
			{
				_loadProgress = _loadDestinationAsyncOperation.progress;
				yield return null;
			}
            
			PLLoadingSceneDebug("PLLoadingSceneManagerAdditive : load progress complete");
			PLSceneLoadingManager.LoadingSceneEvent.Trigger(_sceneToLoadName, PLSceneLoadingManager.LoadingStatus.LoadProgressComplete);
			OnLoadProgressComplete?.Invoke();

			// when the load is close to the end (it'll never reach it), we set it to 100%
			_loadProgress = 1f;

			// we wait for the bar to be visually filled to continue
			if (_interpolateProgress)
			{
				while (_interpolatedLoadProgress < 1f)
				{
					yield return null;
				}
			}			

			PLLoadingSceneDebug("PLLoadingSceneManagerAdditive : interpolated load complete");
			PLSceneLoadingManager.LoadingSceneEvent.Trigger(_sceneToLoadName, PLSceneLoadingManager.LoadingStatus.InterpolatedLoadProgressComplete);
			OnInterpolatedLoadProgressComplete?.Invoke();
		}

		/// <summary>
		/// Waits for BeforeExitFadeDelay seconds
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator ProcessDelayBeforeExitFade()
		{
			if (_beforeExitFadeDelay > 0f)
			{
				PLLoadingSceneDebug("PLLoadingSceneManagerAdditive : delay before exit fade, duration : " + _beforeExitFadeDelay);
				PLSceneLoadingManager.LoadingSceneEvent.Trigger(_sceneToLoadName, PLSceneLoadingManager.LoadingStatus.BeforeExitFade);
				OnBeforeExitFade?.Invoke();
				
				yield return PLCoroutine.WaitForUnscaled(_beforeExitFadeDelay);
			}
		}

		/// <summary>
		/// Requests a fade on exit
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator ExitFade()
		{
			SetAudioListener(false);
			if (_exitFadeDuration > 0f)
			{
				PLLoadingSceneDebug("PLLoadingSceneManagerAdditive : exit fade, duration : " + _exitFadeDuration);
				PLSceneLoadingManager.LoadingSceneEvent.Trigger(_sceneToLoadName, PLSceneLoadingManager.LoadingStatus.ExitFade);
				OnExitFade?.Invoke();
				
				if (_fadeMode == FadeModes.FadeOutThenIn)
				{
					PLFadeInEvent.Trigger(_exitFadeDuration, _exitFadeTween, FaderID, true);
				}
				else
				{
					PLFadeOutEvent.Trigger(_exitFadeDuration, _exitFadeTween, FaderID, true);
				}
				yield return PLCoroutine.WaitForUnscaled(_exitFadeDuration);
			}
		}

		/// <summary>
		/// Activates the destination scene
		/// </summary>
		protected virtual IEnumerator DestinationSceneActivation()
		{
			yield return PLCoroutine.WaitForFrames(1);
			_loadDestinationAsyncOperation.allowSceneActivation = true;
			while (_loadDestinationAsyncOperation.progress < 1.0f)
			{
				yield return null;
			}
			PLLoadingSceneDebug("PLLoadingSceneManagerAdditive : activating destination scene");
			PLSceneLoadingManager.LoadingSceneEvent.Trigger(_sceneToLoadName, PLSceneLoadingManager.LoadingStatus.DestinationSceneActivation);
			OnDestinationSceneActivation?.Invoke();
		}

		/// <summary>
		/// A method triggered when the async operation completes
		/// </summary>
		/// <param name="obj"></param>
		protected virtual void OnLoadOperationComplete(AsyncOperation obj)
		{
			SceneManager.SetActiveScene(SceneManager.GetSceneByName(_sceneToLoadName));
			PLLoadingSceneDebug("PLLoadingSceneManagerAdditive : set active scene to " + _sceneToLoadName);

		}

		/// <summary>
		/// Unloads the scene loader
		/// </summary>
		/// <returns></returns>
		protected virtual IEnumerator UnloadSceneLoader()
		{
			PLLoadingSceneDebug("PLLoadingSceneManagerAdditive : unloading scene loader");
			PLSceneLoadingManager.LoadingSceneEvent.Trigger(_sceneToLoadName, PLSceneLoadingManager.LoadingStatus.UnloadSceneLoader);
			OnUnloadSceneLoader?.Invoke();
			
			yield return null; // mandatory yield to avoid an unjustified warning
			_unloadLoadingAsyncOperation = SceneManager.UnloadSceneAsync(_loadingScreenSceneName);
			while (_unloadLoadingAsyncOperation.progress < _asyncProgressLimit)
			{
				yield return null;
			}
		}

		/// <summary>
		/// Turns the loading audio listener on or off
		/// </summary>
		/// <param name="state"></param>
		protected virtual void SetAudioListener(bool state)
		{
			if (LoadingAudioListener != null)
			{
				//LoadingAudioListener.gameObject.SetActive(state);
			}
		}

		/// <summary>
		/// On Destroy we reset our state
		/// </summary>
		protected virtual void OnDestroy()
		{
			_loadingInProgress = false;
		}

		/// <summary>
		/// A debug method used to output console messages, for this class only
		/// </summary>
		/// <param name="message"></param>
		protected virtual void PLLoadingSceneDebug(string message)
		{
			if (!DebugMode)
			{
				return;
			}
			
			string output = "";
			output += "<color=#82d3f9>[" + Time.frameCount + "]</color> ";
			output += "<color=#f9a682>[" + PLTime.FloatToTimeString(Time.time, false, true, true, true) + "]</color> ";
			output +=  message;
			Debug.Log(output);
		}
	}
}