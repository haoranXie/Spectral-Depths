using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A simple yet powerful sound manager, that will let you play sounds with an event based approach and performance in mind.
	/// 
	/// Features :
	/// 
	/// - Play/stop/pause/resume/free sounds
	/// - Full control : loop, volume, pitch, pan, spatial blend, bypasses, priority, reverb, doppler level, spread, rolloff mode, distance
	/// - 2D & 3D spatial support
	/// - Built-in pooling, automatically recycle a set of audio sources for maximum performance
	/// - Built in audio mixer and groups, with ready-made tracks (Master, Music, SFX, UI), and options to play on more groups if needed
	/// - Stop/pause/resume/free entire tracks
	/// - Stop/pause/resume/free all sounds at once
	/// - Mute / set volume entire tracks
	/// - Save and load settings, with auto save / auto load mechanics built-in
	/// - Fade in/out sounds
	/// - Fade in/out tracks
	/// - Solo mode : play a sound with one or all tracks muted, then unmute them automatically afterwards
	/// - PlayOptions struct
	/// - Option to have sounds persist across scene loads and from scene to scene
	/// - Inspector controls for tracks (volume, mute, unmute, play, pause, stop, resume, free, number of sounds)
	/// - PLSfxEvents
	/// - PLSoundManagerEvents : mute track, control track, save, load, reset, stop persistent sounds 
	/// </summary>
	[AddComponentMenu("Spectral Depths/Tools/Audio/PLSoundManager")]
	public class PLSoundManager : PLPersistentSingleton<PLSoundManager>, 
		PLEventListener<PLSoundManagerTrackEvent>, 
		PLEventListener<PLSoundManagerEvent>,
		PLEventListener<PLSoundManagerSoundControlEvent>,
		PLEventListener<PLSoundManagerSoundFadeEvent>,
		PLEventListener<PLSoundManagerAllSoundsControlEvent>,
		PLEventListener<PLSoundManagerTrackFadeEvent>
	{
		/// the possible ways to manage a track
		public enum PLSoundManagerTracks { Sfx, Music, UI, Master, Other}
        
		[Header("Settings")]
		/// the current sound settings 
		[Tooltip("the current sound settings ")]
		public PLSoundManagerSettingsSO settingsSo;

		[Header("Pool")]
		/// the size of the AudioSource pool, a reserve of ready-to-use sources that will get recycled. Should be approximately equal to the maximum amount of sounds that you expect to be playing at once 
		[Tooltip("the size of the AudioSource pool, a reserve of ready-to-use sources that will get recycled. Should be approximately equal to the maximum amount of sounds that you expect to be playing at once")]
		public int AudioSourcePoolSize = 10;
		/// whether or not the pool can expand (create new audiosources on demand). In a perfect world you'd want to avoid this, and have a sufficiently big pool, to avoid costly runtime creations.
		[Tooltip("whether or not the pool can expand (create new audiosources on demand). In a perfect world you'd want to avoid this, and have a sufficiently big pool, to avoid costly runtime creations.")]
		public bool PoolCanExpand = true;
        
		protected PLSoundManagerAudioPool _pool;
		protected GameObject _tempAudioSourceGameObject;
		protected PLSoundManagerSound _sound;
		protected List<PLSoundManagerSound> _sounds; 
		protected AudioSource _tempAudioSource;
		protected Dictionary<AudioSource, Coroutine> _fadeSoundCoroutines;
		protected Dictionary<PLSoundManagerTracks, Coroutine> _fadeTrackCoroutines;

		#region Initialization

		/// <summary>
		/// On Awake we initialize our manager
		/// </summary>
		protected override void Awake()
		{
			base.Awake();
			InitializeSoundManager();
		}
        
		/// <summary>
		/// On Start we load and apply our saved settings if needed.
		/// This is done on Start and not Awake because of a bug in Unity's AudioMixer API
		/// </summary>
		protected virtual void Start()
		{
			if ((settingsSo != null) && (settingsSo.Settings.AutoLoad))
			{
				settingsSo.LoadSoundSettings();    
			}
		}

		/// <summary>
		/// Initializes the pool, fills it, registers to the scene loaded event
		/// </summary>
		protected virtual void InitializeSoundManager()
		{
			if (_pool == null)
			{
				_pool = new PLSoundManagerAudioPool();    
			}
			_sounds = new List<PLSoundManagerSound>();
			_pool.FillAudioSourcePool(AudioSourcePoolSize, this.transform);
			_fadeSoundCoroutines = new Dictionary<AudioSource, Coroutine>();
			_fadeTrackCoroutines = new Dictionary<PLSoundManagerTracks, Coroutine>();
		}
        
		#endregion
        
		#region PlaySound

		/// <summary>
		/// Plays a sound, separate options object signature
		/// </summary>
		/// <param name="audioClip"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public virtual AudioSource PlaySound(AudioClip audioClip, PLSoundManagerPlayOptions options)
		{
			return PlaySound(audioClip, options.MmSoundManagerTrack, options.Location,
				options.Loop, options.Volume, options.ID,
				options.Fade, options.FadeInitialVolume, options.FadeDuration, options.FadeTween,
				options.Persistent,
				options.RecycleAudioSource, options.AudioGroup,
				options.Pitch, options.PanStereo, options.SpatialBlend,
				options.SoloSingleTrack, options.SoloAllTracks, options.AutoUnSoloOnEnd,
				options.BypassEffects, options.BypassListenerEffects, options.BypassReverbZones, options.Priority,
				options.ReverbZoneMix,
				options.DopplerLevel, options.Spread, options.RolloffMode, options.MinDistance, options.MaxDistance, 
				options.DoNotAutoRecycleIfNotDonePlaying, options.PlaybackTime, options.PlaybackDuration, options.AttachToTransform,
				options.UseSpreadCurve, options.SpreadCurve, options.UseCustomRolloffCurve, options.CustomRolloffCurve,
				options.UseSpatialBlendCurve, options.SpatialBlendCurve, options.UseReverbZoneMixCurve, options.ReverbZoneMixCurve
			);
		}

		/// <summary>
		/// Plays a sound, signature with all options
		/// </summary>
		/// <param name="audioClip"></param>
		/// <param name="mmSoundManagerTrack"></param>
		/// <param name="location"></param>
		/// <param name="loop"></param>
		/// <param name="volume"></param>
		/// <param name="ID"></param>
		/// <param name="fade"></param>
		/// <param name="fadeInitialVolume"></param>
		/// <param name="fadeDuration"></param>
		/// <param name="fadeTween"></param>
		/// <param name="persistent"></param>
		/// <param name="recycleAudioSource"></param>
		/// <param name="audioGroup"></param>
		/// <param name="pitch"></param>
		/// <param name="panStereo"></param>
		/// <param name="spatialBlend"></param>
		/// <param name="soloSingleTrack"></param>
		/// <param name="soloAllTracks"></param>
		/// <param name="autoUnSoloOnEnd"></param>
		/// <param name="bypassEffects"></param>
		/// <param name="bypassListenerEffects"></param>
		/// <param name="bypassReverbZones"></param>
		/// <param name="priority"></param>
		/// <param name="reverbZoneMix"></param>
		/// <param name="dopplerLevel"></param>
		/// <param name="spread"></param>
		/// <param name="rolloffMode"></param>
		/// <param name="minDistance"></param>
		/// <param name="maxDistance"></param>
		/// <returns></returns>
		public virtual AudioSource PlaySound(AudioClip audioClip, PLSoundManagerTracks mmSoundManagerTrack, Vector3 location, 
			bool loop = false, float volume = 1.0f, int ID = 0,
			bool fade = false, float fadeInitialVolume = 0f, float fadeDuration = 1f, PLTweenType fadeTween = null,
			bool persistent = false,
			AudioSource recycleAudioSource = null, AudioMixerGroup audioGroup = null,
			float pitch = 1f, float panStereo = 0f, float spatialBlend = 0.0f,  
			bool soloSingleTrack = false, bool soloAllTracks = false, bool autoUnSoloOnEnd = false,  
			bool bypassEffects = false, bool bypassListenerEffects = false, bool bypassReverbZones = false, int priority = 128, float reverbZoneMix = 1f,
			float dopplerLevel = 1f, int spread = 0, AudioRolloffMode rolloffMode = AudioRolloffMode.Logarithmic, float minDistance = 1f, float maxDistance = 500f,
			bool doNotAutoRecycleIfNotDonePlaying = false, float playbackTime = 0f, float playbackDuration = 0f, Transform attachToTransform = null,
			bool useSpreadCurve = false, AnimationCurve spreadCurve = null, bool useCustomRolloffCurve = false, AnimationCurve customRolloffCurve = null,
			bool useSpatialBlendCurve = false, AnimationCurve spatialBlendCurve = null, bool useReverbZoneMixCurve = false, AnimationCurve reverbZoneMixCurve = null
		)
		{
			if (this == null) { return null; }
			if (!audioClip) { return null; }
            
			// audio source setup ---------------------------------------------------------------------------------
            
			// we reuse an audiosource if one is passed in parameters
			AudioSource audioSource = recycleAudioSource;   
            
			if (audioSource == null)
			{
				// we pick an idle audio source from the pool if possible
				audioSource = _pool.GetAvailableAudioSource(PoolCanExpand, this.transform);
				if ((audioSource != null) && (!loop))
				{
					recycleAudioSource = audioSource;
					// we destroy the host after the clip has played (if it not tag for reusability.
					StartCoroutine(_pool.AutoDisableAudioSource(audioClip.length / Mathf.Abs(pitch), audioSource, audioClip, doNotAutoRecycleIfNotDonePlaying, playbackTime, playbackDuration));
				}
			}

			// we create an audio source if needed
			if (audioSource == null)
			{
				_tempAudioSourceGameObject = new GameObject("PLAudio_"+audioClip.name);
				SceneManager.MoveGameObjectToScene(_tempAudioSourceGameObject, this.gameObject.scene);
				audioSource = _tempAudioSourceGameObject.AddComponent<AudioSource>();
			}
            
			// audio source settings ---------------------------------------------------------------------------------
            
			audioSource.transform.position = location;
			audioSource.clip = audioClip;
			audioSource.pitch = pitch;
			audioSource.spatialBlend = spatialBlend;
			audioSource.panStereo = panStereo;
			audioSource.loop = loop;
			audioSource.bypassEffects = bypassEffects;
			audioSource.bypassListenerEffects = bypassListenerEffects;
			audioSource.bypassReverbZones = bypassReverbZones;
			audioSource.priority = priority;
			audioSource.reverbZoneMix = reverbZoneMix;
			audioSource.dopplerLevel = dopplerLevel;
			audioSource.spread = spread;
			audioSource.rolloffMode = rolloffMode;
			audioSource.minDistance = minDistance;
			audioSource.maxDistance = maxDistance;
			audioSource.time = playbackTime; 
			
			// curves
			if (useSpreadCurve) { audioSource.SetCustomCurve(AudioSourceCurveType.Spread, spreadCurve); }
			if (useCustomRolloffCurve) { audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, customRolloffCurve); }
			if (useSpatialBlendCurve) { audioSource.SetCustomCurve(AudioSourceCurveType.SpatialBlend, spatialBlendCurve); }
			if (useReverbZoneMixCurve) { audioSource.SetCustomCurve(AudioSourceCurveType.ReverbZoneMix, reverbZoneMixCurve); }
			
			// attaching to target
			if (attachToTransform != null)
			{
				PLFollowTarget followTarget = audioSource.gameObject.PLGetComponentNoAlloc<PLFollowTarget>();
				if (followTarget == null)
				{
					followTarget = audioSource.gameObject.AddComponent<PLFollowTarget>();
				}
				followTarget.Target = attachToTransform;
				followTarget.InterpolatePosition = false;
				followTarget.InterpolateRotation = false;
				followTarget.InterpolateScale = false;
				followTarget.FollowRotation = false;
				followTarget.FollowScale = false;
				followTarget.enabled = true;
			}
            
			// track and volume ---------------------------------------------------------------------------------
            
			if (settingsSo != null)
			{
				audioSource.outputAudioMixerGroup = settingsSo.MasterAudioMixerGroup;
				switch (mmSoundManagerTrack)
				{
					case PLSoundManagerTracks.Master:
						audioSource.outputAudioMixerGroup = settingsSo.MasterAudioMixerGroup;
						break;
					case PLSoundManagerTracks.Music:
						audioSource.outputAudioMixerGroup = settingsSo.MusicAudioMixerGroup;
						break;
					case PLSoundManagerTracks.Sfx:
						audioSource.outputAudioMixerGroup = settingsSo.SfxAudioMixerGroup;
						break;
					case PLSoundManagerTracks.UI:
						audioSource.outputAudioMixerGroup = settingsSo.UIAudioMixerGroup;
						break;
				}
			}
			if (audioGroup) { audioSource.outputAudioMixerGroup = audioGroup; }
			audioSource.volume = volume;  
            
			// we start playing the sound
			audioSource.Play();
            
			// we destroy the host after the clip has played if it was a one time AS.
			if (!loop && !recycleAudioSource)
			{
				float destroyDelay = (playbackDuration > 0) ? playbackDuration : audioClip.length - playbackTime;
				Destroy(_tempAudioSourceGameObject, destroyDelay);
			}
            
			// we fade the sound in if needed
			if (fade)
			{
				FadeSound(audioSource, fadeDuration, fadeInitialVolume, volume, fadeTween);
			}
            
			// we handle soloing
			if (soloSingleTrack)
			{
				MuteSoundsOnTrack(mmSoundManagerTrack, true, 0f);
				audioSource.mute = false;
				if (autoUnSoloOnEnd)
				{
					MuteSoundsOnTrack(mmSoundManagerTrack, false, audioClip.length);
				}
			}
			else if (soloAllTracks)
			{
				MuteAllSounds();
				audioSource.mute = false;
				if (autoUnSoloOnEnd)
				{
					StartCoroutine(MuteAllSoundsCoroutine(audioClip.length - playbackTime, false));
				}
			}
            
			// we prepare for storage
			_sound.ID = ID;
			_sound.Track = mmSoundManagerTrack;
			_sound.Source = audioSource;
			_sound.Persistent = persistent;
			_sound.PlaybackTime = playbackTime;
			_sound.PlaybackDuration = playbackDuration;

			// we check if that audiosource is already being tracked in _sounds
			bool alreadyIn = false;
			for (int i = 0; i < _sounds.Count; i++)
			{
				if (_sounds[i].Source == audioSource)
				{
					_sounds[i] = _sound;
					alreadyIn = true;
				}
			}

			if (!alreadyIn)
			{
				_sounds.Add(_sound);    
			}

			// we return the audiosource reference
			return audioSource;
		}
        
		#endregion

		#region SoundControls

		/// <summary>
		/// Pauses the specified audiosource
		/// </summary>
		/// <param name="source"></param>
		public virtual void PauseSound(AudioSource source)
		{
			source.Pause();
		}

		/// <summary>
		/// resumes play on the specified audio source
		/// </summary>
		/// <param name="source"></param>
		public virtual void ResumeSound(AudioSource source)
		{
			source.Play();
		}
        
		/// <summary>
		/// Stops the specified audio source
		/// </summary>
		/// <param name="source"></param>
		public virtual void StopSound(AudioSource source)
		{
			source.Stop();
		}
        
		/// <summary>
		/// Frees a specific sound, stopping it and returning it to the pool
		/// </summary>
		/// <param name="source"></param>
		public virtual void FreeSound(AudioSource source)
		{
			source.Stop();
			if (!_pool.FreeSound(source))
			{
				Destroy(source.gameObject);    
			}
		}

		#endregion
        
		#region TrackControls
        
		/// <summary>
		/// Mutes an entire track
		/// </summary>
		/// <param name="track"></param>
		public virtual void MuteTrack(PLSoundManagerTracks track)
		{
			ControlTrack(track, ControlTrackModes.Mute, 0f);
		}

		/// <summary>
		/// Unmutes an entire track
		/// </summary>
		/// <param name="track"></param>
		public virtual void UnmuteTrack(PLSoundManagerTracks track)
		{
			ControlTrack(track, ControlTrackModes.Unmute, 0f);
		}

		/// <summary>
		/// Sets the volume of an entire track
		/// </summary>
		/// <param name="track"></param>
		/// <param name="volume"></param>
		public virtual void SetTrackVolume(PLSoundManagerTracks track, float volume)
		{
			ControlTrack(track, ControlTrackModes.SetVolume, volume);
		}

		/// <summary>
		/// Returns the current volume of a track
		/// </summary>
		/// <param name="track"></param>
		/// <param name="volume"></param>
		public virtual float GetTrackVolume(PLSoundManagerTracks track, bool mutedVolume)
		{
			switch (track)
			{
				case PLSoundManagerTracks.Master:
					if (mutedVolume)
					{
						return settingsSo.Settings.MutedMasterVolume;
					}
					else
					{
						return settingsSo.Settings.MasterVolume;
					}
				case PLSoundManagerTracks.Music:
					if (mutedVolume)
					{
						return settingsSo.Settings.MutedMusicVolume;
					}
					else
					{
						return settingsSo.Settings.MusicVolume;
					}
				case PLSoundManagerTracks.Sfx:
					if (mutedVolume)
					{
						return settingsSo.Settings.MutedSfxVolume;
					}
					else
					{
						return settingsSo.Settings.SfxVolume;
					}
				case PLSoundManagerTracks.UI:
					if (mutedVolume)
					{
						return settingsSo.Settings.MutedUIVolume;
					}
					else
					{
						return settingsSo.Settings.UIVolume;
					}
			}

			return 1f;
		}
        
		/// <summary>
		/// Pauses all sounds on a track
		/// </summary>
		/// <param name="track"></param>
		public virtual void PauseTrack(PLSoundManagerTracks track)
		{
			foreach (PLSoundManagerSound sound in _sounds)
			{
				if (sound.Track == track)
				{
					sound.Source.Pause();
				}
			}    
		}

		/// <summary>
		/// Plays or resumes all sounds on a track
		/// </summary>
		/// <param name="track"></param>
		public virtual void PlayTrack(PLSoundManagerTracks track)
		{
			foreach (PLSoundManagerSound sound in _sounds)
			{
				if (sound.Track == track)
				{
					sound.Source.Play();
				}
			}    
		}

		/// <summary>
		/// Stops all sounds on a track
		/// </summary>
		/// <param name="track"></param>
		public virtual void StopTrack(PLSoundManagerTracks track)
		{
			foreach (PLSoundManagerSound sound in _sounds)
			{
				if (sound.Track == track)
				{
					sound.Source.Stop();
				}
			}
		}

		/// <summary>
		/// Returns true if sounds are currently playing on that track
		/// </summary>
		/// <param name="track"></param>
		public virtual bool HasSoundsPlaying(PLSoundManagerTracks track)
		{
			foreach (PLSoundManagerSound sound in _sounds)
			{
				if ((sound.Track == track) && (sound.Source.isPlaying))
				{
					return true;
				}
			}
			return false;
		}
		
		/// <summary>
		/// Returns a list of PLSoundManagerSounds for the specified track
		/// </summary>
		/// <param name="track">the track on which to grab the playing sounds</param>
		/// <returns></returns>
		public virtual List<PLSoundManagerSound> GetSoundsPlaying(PLSoundManagerTracks track)
		{
			List<PLSoundManagerSound> soundsPlaying = new List<PLSoundManagerSound>();
			foreach (PLSoundManagerSound sound in _sounds)
			{
				if ((sound.Track == track) && (sound.Source.isPlaying))
				{
					soundsPlaying.Add(sound);
				}
			}
			return soundsPlaying;
		}
        
		/// <summary>
		/// Stops all sounds on a track, and returns them to the pool
		/// </summary>
		/// <param name="track"></param>
		public virtual void FreeTrack(PLSoundManagerTracks track)
		{
			foreach (PLSoundManagerSound sound in _sounds)
			{
				if (sound.Track == track)
				{
					sound.Source.Stop();
					sound.Source.gameObject.SetActive(false);
				}
			}
		}
        
		/// <summary>
		/// Mutes the music track, QoL method ready to bind to a UnityEvent
		/// </summary>
		public virtual void MuteMusic() { MuteTrack(PLSoundManagerTracks.Music); }
        
		/// <summary>
		/// Unmutes the music track, QoL method ready to bind to a UnityEvent
		/// </summary>
		public virtual void UnmuteMusic() { UnmuteTrack(PLSoundManagerTracks.Music); }
        
		/// <summary>
		/// Mutes the sfx track, QoL method ready to bind to a UnityEvent
		/// </summary>
		public virtual void MuteSfx() { MuteTrack(PLSoundManagerTracks.Sfx); }
        
        
		/// <summary>
		/// Unmutes the sfx track, QoL method ready to bind to a UnityEvent
		/// </summary>
		public virtual void UnmuteSfx() { UnmuteTrack(PLSoundManagerTracks.Sfx); }
        
		/// <summary>
		/// Mutes the UI track, QoL method ready to bind to a UnityEvent
		/// </summary>
		public virtual void MuteUI() { MuteTrack(PLSoundManagerTracks.UI); }
        
		/// <summary>
		/// Unmutes the UI track, QoL method ready to bind to a UnityEvent
		/// </summary>
		public virtual void UnmuteUI() { UnmuteTrack(PLSoundManagerTracks.UI); }
        
		/// <summary>
		/// Mutes the master track, QoL method ready to bind to a UnityEvent
		/// </summary>
		public virtual void MuteMaster() { MuteTrack(PLSoundManagerTracks.Master); }
        
		/// <summary>
		/// Unmutes the master track, QoL method ready to bind to a UnityEvent
		/// </summary>
		public virtual void UnmuteMaster() { UnmuteTrack(PLSoundManagerTracks.Master); }
        
        
		/// <summary>
		/// Sets the volume of the Music track to the specified value, QoL method, ready to bind to a UnityEvent
		/// </summary>
		public virtual void SetVolumeMusic(float newVolume) { SetTrackVolume(PLSoundManagerTracks.Music, newVolume);}
		/// <summary>
		/// Sets the volume of the SFX track to the specified value, QoL method, ready to bind to a UnityEvent
		/// </summary>
		public virtual void SetVolumeSfx(float newVolume) { SetTrackVolume(PLSoundManagerTracks.Sfx, newVolume);}
		/// <summary>
		/// Sets the volume of the UI track to the specified value, QoL method, ready to bind to a UnityEvent
		/// </summary>
		public virtual void SetVolumeUI(float newVolume) { SetTrackVolume(PLSoundManagerTracks.UI, newVolume);}
		/// <summary>
		/// Sets the volume of the Master track to the specified value, QoL method, ready to bind to a UnityEvent
		/// </summary>
		public virtual void SetVolumeMaster(float newVolume) { SetTrackVolume(PLSoundManagerTracks.Master, newVolume);}

		/// <summary>
		/// Returns true if the specified track is muted, false otherwise
		/// </summary>
		/// <param name="track"></param>
		/// <returns></returns>
		public virtual bool IsMuted(PLSoundManagerTracks track)
		{
			switch (track)
			{
				case PLSoundManagerTracks.Master:
					return !settingsSo.Settings.MasterOn; 
				case PLSoundManagerTracks.Music:
					return !settingsSo.Settings.MusicOn;
				case PLSoundManagerTracks.Sfx:
					return !settingsSo.Settings.SfxOn;
				case PLSoundManagerTracks.UI:
					return !settingsSo.Settings.UIOn;
			}
			return false;
		}
        
		/// <summary>
		/// A method that will let you mute/unmute a track, or set it to a specified volume
		/// </summary>
		public enum ControlTrackModes { Mute, Unmute, SetVolume }
		protected virtual void ControlTrack(PLSoundManagerTracks track, ControlTrackModes trackMode, float volume = 0.5f)
		{
			string target = "";
			float savedVolume = 0f; 
            
			switch (track)
			{
				case PLSoundManagerTracks.Master:
					target = settingsSo.Settings.MasterVolumeParameter;
					if (trackMode == ControlTrackModes.Mute) { settingsSo.TargetAudioMixer.GetFloat(target, out settingsSo.Settings.MutedMasterVolume); settingsSo.Settings.MasterOn = false; }
					else if (trackMode == ControlTrackModes.Unmute) { savedVolume = settingsSo.Settings.MutedMasterVolume; settingsSo.Settings.MasterOn = true; }
					break;
				case PLSoundManagerTracks.Music:
					target = settingsSo.Settings.MusicVolumeParameter;
					if (trackMode == ControlTrackModes.Mute) { settingsSo.TargetAudioMixer.GetFloat(target, out settingsSo.Settings.MutedMusicVolume);  settingsSo.Settings.MusicOn = false; }
					else if (trackMode == ControlTrackModes.Unmute) { savedVolume = settingsSo.Settings.MutedMusicVolume;  settingsSo.Settings.MusicOn = true; }
					break;
				case PLSoundManagerTracks.Sfx:
					target = settingsSo.Settings.SfxVolumeParameter;
					if (trackMode == ControlTrackModes.Mute) { settingsSo.TargetAudioMixer.GetFloat(target, out settingsSo.Settings.MutedSfxVolume);  settingsSo.Settings.SfxOn = false; }
					else if (trackMode == ControlTrackModes.Unmute) { savedVolume = settingsSo.Settings.MutedSfxVolume;  settingsSo.Settings.SfxOn = true; }
					break;
				case PLSoundManagerTracks.UI:
					target = settingsSo.Settings.UIVolumeParameter;
					if (trackMode == ControlTrackModes.Mute) { settingsSo.TargetAudioMixer.GetFloat(target, out settingsSo.Settings.MutedUIVolume);  settingsSo.Settings.UIOn = false; }
					else if (trackMode == ControlTrackModes.Unmute) { savedVolume = settingsSo.Settings.MutedUIVolume;  settingsSo.Settings.UIOn = true; }
					break;
			}

			switch (trackMode)
			{
				case ControlTrackModes.Mute:
					settingsSo.SetTrackVolume(track, 0f);
					break;
				case ControlTrackModes.Unmute:
					settingsSo.SetTrackVolume(track, settingsSo.MixerVolumeToNormalized(savedVolume));
					break;
				case ControlTrackModes.SetVolume:
					settingsSo.SetTrackVolume(track, volume);
					break;
			}

			settingsSo.GetTrackVolumes();

			if (settingsSo.Settings.AutoSave)
			{
				settingsSo.SaveSoundSettings();
			}
		}
        
		#endregion

		#region Fades
        
		/// <summary>
		/// Fades an entire track over the specified duration towards the desired finalVolume
		/// </summary>
		/// <param name="track"></param>
		/// <param name="duration"></param>
		/// <param name="initialVolume"></param>
		/// <param name="finalVolume"></param>
		/// <param name="tweenType"></param>
		public virtual void FadeTrack(PLSoundManagerTracks track, float duration, float initialVolume = 0f, float finalVolume = 1f, PLTweenType tweenType = null)
		{
			Coroutine coroutine = StartCoroutine(FadeTrackCoroutine(track, duration, initialVolume, finalVolume, tweenType));
			_fadeTrackCoroutines[track] = coroutine;
		}
        
		/// <summary>
		/// Fades a target sound towards a final volume over time
		/// </summary>
		/// <param name="source"></param>
		/// <param name="duration"></param>
		/// <param name="initialVolume"></param>
		/// <param name="finalVolume"></param>
		/// <param name="tweenType"></param>
		public virtual void FadeSound(AudioSource source, float duration, float initialVolume, float finalVolume, PLTweenType tweenType)
		{
			Coroutine coroutine = StartCoroutine(FadeCoroutine(source, duration, initialVolume, finalVolume, tweenType));
			_fadeSoundCoroutines[source] = coroutine;
		}

		/// <summary>
		/// Stops any fade currently happening on the specified track
		/// </summary>
		/// <param name="track"></param>
		public virtual void StopFadeTrack(PLSoundManagerTracks track)
		{
			Coroutine outCoroutine;
			if (_fadeTrackCoroutines.TryGetValue(track, out outCoroutine))
			{
				StopCoroutine(outCoroutine);
				_fadeTrackCoroutines.Remove(track);
			}
		}

		/// <summary>
		/// Stops any fade currently happening on the specified sound
		/// </summary>
		/// <param name="source"></param>
		public virtual void StopFadeSound(AudioSource source)
		{
			Coroutine outCoroutine;
			if (_fadeSoundCoroutines.TryGetValue(source, out outCoroutine))
			{
				StopCoroutine(outCoroutine);
				_fadeSoundCoroutines.Remove(source);
			}
		}

		/// <summary>
		/// Fades an entire track over time
		/// </summary>
		/// <param name="track"></param>
		/// <param name="duration"></param>
		/// <param name="initialVolume"></param>
		/// <param name="finalVolume"></param>
		/// <param name="tweenType"></param>
		/// <returns></returns>
		protected virtual IEnumerator FadeTrackCoroutine(PLSoundManagerTracks track, float duration, float initialVolume, float finalVolume, PLTweenType tweenType)
		{
			float startedAt = Time.unscaledTime;
			if (tweenType == null)
			{
				tweenType = new PLTweenType(PLTween.PLTweenCurve.EaseInOutQuartic);
			}
			while (Time.unscaledTime - startedAt <= duration)
			{
				float elapsedTime = Time.unscaledTime - startedAt;
				float newVolume = PLTween.Tween(elapsedTime, 0f, duration, initialVolume, finalVolume, tweenType);
				settingsSo.SetTrackVolume(track, newVolume);
				yield return null;
			}
			settingsSo.SetTrackVolume(track, finalVolume);
		}

		/// <summary>
		/// Fades an audiosource's volume over time
		/// </summary>
		/// <param name="source"></param>
		/// <param name="duration"></param>
		/// <param name="initialVolume"></param>
		/// <param name="finalVolume"></param>
		/// <param name="tweenType"></param>
		/// <returns></returns>
		protected virtual IEnumerator FadeCoroutine(AudioSource source, float duration, float initialVolume, float finalVolume, PLTweenType tweenType)
		{
			float startedAt = Time.unscaledTime;
			if (tweenType == null)
			{
				tweenType = new PLTweenType(PLTween.PLTweenCurve.EaseInOutQuartic);
			}
			while (Time.unscaledTime - startedAt <= duration)
			{
				float elapsedTime = Time.unscaledTime - startedAt;
				float newVolume = PLTween.Tween(elapsedTime, 0f, duration, initialVolume, finalVolume, tweenType);
				source.volume = newVolume;
				yield return null;
			}
			source.volume = finalVolume;
		}
        
		#endregion

		#region Solo

		/// <summary>
		/// Mutes all sounds playing on a specific track
		/// </summary>
		/// <param name="track"></param>
		/// <param name="mute"></param>
		/// <param name="delay"></param>
		public virtual void MuteSoundsOnTrack(PLSoundManagerTracks track, bool mute, float delay = 0f)
		{
			StartCoroutine(MuteSoundsOnTrackCoroutine(track, mute, delay));
		}
        
		/// <summary>
		/// Mutes all sounds playing on the PLSoundManager
		/// </summary>
		/// <param name="mute"></param>
		public virtual void MuteAllSounds(bool mute = true)
		{
			StartCoroutine(MuteAllSoundsCoroutine(0f, mute));
		}

		/// <summary>
		/// Mutes all sounds on the specified track after an optional delay
		/// </summary>
		/// <param name="track"></param>
		/// <param name="mute"></param>
		/// <param name="delay"></param>
		/// <returns></returns>
		protected virtual IEnumerator MuteSoundsOnTrackCoroutine(PLSoundManagerTracks track, bool mute, float delay)
		{
			if (delay > 0)
			{
				yield return PLCoroutine.WaitForUnscaled(delay);    
			}
            
			foreach (PLSoundManagerSound sound in _sounds)
			{
				if (sound.Track == track)
				{
					sound.Source.mute = mute;
				}
			}
		}

		/// <summary>
		/// Mutes all sounds after an optional delay
		/// </summary>
		/// <param name="delay"></param>
		/// <param name="mute"></param>
		/// <returns></returns>
		protected  virtual IEnumerator MuteAllSoundsCoroutine(float delay, bool mute = true)
		{
			if (delay > 0)
			{
				yield return PLCoroutine.WaitForUnscaled(delay);    
			}
			foreach (PLSoundManagerSound sound in _sounds)
			{
				sound.Source.mute = mute;
			}   
		}

		#endregion

		#region Find

		/// <summary>
		/// Returns an audio source played with the specified ID, if one is found
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		public virtual AudioSource FindByID(int ID)
		{
			foreach (PLSoundManagerSound sound in _sounds)
			{
				if (sound.ID == ID)
				{
					return sound.Source;
				}
			}

			return null;
		}

		/// <summary>
		/// Returns an audio source played with the specified ID, if one is found
		/// </summary>
		/// <param name="ID"></param>
		/// <returns></returns>
		public virtual AudioSource FindByClip(AudioClip clip)
		{
			foreach (PLSoundManagerSound sound in _sounds)
			{
				if (sound.Source.clip == clip)
				{
					return sound.Source;
				}
			}

			return null;
		}

		#endregion

		#region AllSoundsControls

		/// <summary>
		/// Pauses all sounds playing on the PLSoundManager
		/// </summary>
		public virtual void PauseAllSounds()
		{
			foreach (PLSoundManagerSound sound in _sounds)
			{
				sound.Source.Pause();
			}    
		}

		/// <summary>
		/// Plays all sounds playing on the PLSoundManager
		/// </summary>
		public virtual void PlayAllSounds()
		{
			foreach (PLSoundManagerSound sound in _sounds)
			{
				sound.Source.Play();
			}    
		}

		/// <summary>
		/// Stops all sounds playing on the PLSoundManager
		/// </summary>
		public virtual void StopAllSounds()
		{
			foreach (PLSoundManagerSound sound in _sounds)
			{
				sound.Source.Stop();
			}
		}

		/// <summary>
		/// Stops all sounds and returns them to the pool
		/// </summary>
		public virtual void FreeAllSounds()
		{
			foreach (PLSoundManagerSound sound in _sounds)
			{
				if (sound.Source != null)
				{
					FreeSound(sound.Source);    
				}
			}
		}
        
		/// <summary>
		/// Stops all sounds except the persistent ones, and returns them to the pool
		/// </summary>
		public virtual void FreeAllSoundsButPersistent()
		{
			foreach (PLSoundManagerSound sound in _sounds)
			{
				if ((!sound.Persistent) && (sound.Source != null))
				{
					FreeSound(sound.Source);
				}
			}
		}

		/// <summary>
		/// Stops all looping sounds and returns them to the pool
		/// </summary>
		public virtual void FreeAllLoopingSounds()
		{
			foreach (PLSoundManagerSound sound in _sounds)
			{
				if ((sound.Source.loop) && (sound.Source != null))
				{
					FreeSound(sound.Source);
				}
			}
		}

		#endregion

		#region Events
        
		/// <summary>
		/// Registered on enable, triggers every time a new scene is loaded
		/// At which point we free all sounds except the persistent ones
		/// </summary>
		protected virtual void OnSceneLoaded(Scene arg0, LoadSceneMode loadSceneMode)
		{
			FreeAllSoundsButPersistent();
		}

		public virtual void OnMMEvent(PLSoundManagerTrackEvent soundManagerTrackEvent)
		{
			switch (soundManagerTrackEvent.TrackEventType)
			{
				case PLSoundManagerTrackEventTypes.MuteTrack:
					MuteTrack(soundManagerTrackEvent.Track);
					break;
				case PLSoundManagerTrackEventTypes.UnmuteTrack:
					UnmuteTrack(soundManagerTrackEvent.Track);
					break;
				case PLSoundManagerTrackEventTypes.SetVolumeTrack:
					SetTrackVolume(soundManagerTrackEvent.Track, soundManagerTrackEvent.Volume);
					break;
				case PLSoundManagerTrackEventTypes.PlayTrack:
					PlayTrack(soundManagerTrackEvent.Track);
					break;
				case PLSoundManagerTrackEventTypes.PauseTrack:
					PauseTrack(soundManagerTrackEvent.Track);
					break;
				case PLSoundManagerTrackEventTypes.StopTrack:
					StopTrack(soundManagerTrackEvent.Track);
					break;
				case PLSoundManagerTrackEventTypes.FreeTrack:
					FreeTrack(soundManagerTrackEvent.Track);
					break;
			}
		}
        
		public virtual void OnMMEvent(PLSoundManagerEvent soundManagerEvent)
		{
			switch (soundManagerEvent.EventType)
			{
				case PLSoundManagerEventTypes.SaveSettings:
					SaveSettings();
					break;
				case PLSoundManagerEventTypes.LoadSettings:
					settingsSo.LoadSoundSettings();
					break;
				case PLSoundManagerEventTypes.ResetSettings:
					settingsSo.ResetSoundSettings();
					break;
			}
		}

		/// <summary>
		/// Save sound settings to file
		/// </summary>
		public virtual void SaveSettings()
		{
			settingsSo.SaveSoundSettings();
		}

		/// <summary>
		/// Loads sound settings from file
		/// </summary>
		public virtual void LoadSettings()
		{
			settingsSo.LoadSoundSettings();
		}

		/// <summary>
		/// Deletes any saved sound settings
		/// </summary>
		public virtual void ResetSettings()
		{
			settingsSo.ResetSoundSettings();
		}
        
		public virtual void OnMMEvent(PLSoundManagerSoundControlEvent soundControlEvent)
		{
			if (soundControlEvent.TargetSource == null)
			{
				_tempAudioSource = FindByID(soundControlEvent.SoundID);    
			}
			else
			{
				_tempAudioSource = soundControlEvent.TargetSource;
			}

			if (_tempAudioSource != null)
			{
				switch (soundControlEvent.PLSoundManagerSoundControlEventType)
				{
					case PLSoundManagerSoundControlEventTypes.Pause:
						PauseSound(_tempAudioSource);
						break;
					case PLSoundManagerSoundControlEventTypes.Resume:
						ResumeSound(_tempAudioSource);
						break;
					case PLSoundManagerSoundControlEventTypes.Stop:
						StopSound(_tempAudioSource);
						break;
					case PLSoundManagerSoundControlEventTypes.Free:
						FreeSound(_tempAudioSource);
						break;
				}
			}
		}
        
		public virtual void OnMMEvent(PLSoundManagerTrackFadeEvent trackFadeEvent)
		{
			switch (trackFadeEvent.Mode)
			{
				case PLSoundManagerTrackFadeEvent.Modes.PlayFade:
					FadeTrack(trackFadeEvent.Track, trackFadeEvent.FadeDuration, settingsSo.GetTrackVolume(trackFadeEvent.Track), trackFadeEvent.FinalVolume, trackFadeEvent.FadeTween);
					break;
				case PLSoundManagerTrackFadeEvent.Modes.StopFade:
					StopFadeTrack(trackFadeEvent.Track);
					break;
			}
		}
        
		public virtual void OnMMEvent(PLSoundManagerSoundFadeEvent soundFadeEvent)
		{
			_tempAudioSource = FindByID(soundFadeEvent.SoundID);
			switch (soundFadeEvent.Mode)
			{
				case PLSoundManagerSoundFadeEvent.Modes.PlayFade:
					if (_tempAudioSource != null)
					{
						FadeSound(_tempAudioSource, soundFadeEvent.FadeDuration, _tempAudioSource.volume, soundFadeEvent.FinalVolume,
							soundFadeEvent.FadeTween);
					}
					break;
				case PLSoundManagerSoundFadeEvent.Modes.StopFade:
					StopFadeSound(_tempAudioSource);
					break;
			}
		}
        
		public virtual void OnMMEvent(PLSoundManagerAllSoundsControlEvent allSoundsControlEvent)
		{
			switch (allSoundsControlEvent.EventType)
			{
				case PLSoundManagerAllSoundsControlEventTypes.Pause:
					PauseAllSounds();
					break;
				case PLSoundManagerAllSoundsControlEventTypes.Play:
					PlayAllSounds();
					break;
				case PLSoundManagerAllSoundsControlEventTypes.Stop:
					StopAllSounds();
					break;
				case PLSoundManagerAllSoundsControlEventTypes.Free:
					FreeAllSounds();
					break;
				case PLSoundManagerAllSoundsControlEventTypes.FreeAllButPersistent:
					FreeAllSoundsButPersistent();
					break;
				case PLSoundManagerAllSoundsControlEventTypes.FreeAllLooping:
					FreeAllLoopingSounds();
					break;
			}
		}

		public virtual void OnMMSfxEvent(AudioClip clipToPlay, AudioMixerGroup audioGroup = null, float volume = 1f, float pitch = 1f, int priority = 128)
		{
			PLSoundManagerPlayOptions options = PLSoundManagerPlayOptions.Default;
			options.Location = this.transform.position;
			options.AudioGroup = audioGroup;
			options.Volume = volume;
			options.Pitch = pitch;
			if (priority >= 0)
			{
				options.Priority = Mathf.Min(priority, 256);
			}
			options.MmSoundManagerTrack = PLSoundManagerTracks.Sfx;
			options.Loop = false;
            
			PlaySound(clipToPlay, options);
		}

		public virtual AudioSource OnMMSoundManagerSoundPlayEvent(AudioClip clip, PLSoundManagerPlayOptions options)
		{
			return PlaySound(clip, options);
		}
        
		/// <summary>
		/// On enable we start listening for events
		/// </summary>
		protected virtual void OnEnable()
		{
			PLSfxEvent.Register(OnMMSfxEvent);
			PLSoundManagerSoundPlayEvent.Register(OnMMSoundManagerSoundPlayEvent);
			this.PLEventStartListening<PLSoundManagerEvent>();
			this.PLEventStartListening<PLSoundManagerTrackEvent>();
			this.PLEventStartListening<PLSoundManagerSoundControlEvent>();
			this.PLEventStartListening<PLSoundManagerTrackFadeEvent>();
			this.PLEventStartListening<PLSoundManagerSoundFadeEvent>();
			this.PLEventStartListening<PLSoundManagerAllSoundsControlEvent>();
            
			SceneManager.sceneLoaded += OnSceneLoaded;
		}

		/// <summary>
		/// On disable we stop listening for events
		/// </summary>
		protected virtual void OnDisable()
		{
			if (_enabled)
			{
				PLSfxEvent.Unregister(OnMMSfxEvent);
				PLSoundManagerSoundPlayEvent.Unregister(OnMMSoundManagerSoundPlayEvent);
				this.PLEventStopListening<PLSoundManagerEvent>();
				this.PLEventStopListening<PLSoundManagerTrackEvent>();
				this.PLEventStopListening<PLSoundManagerSoundControlEvent>();
				this.PLEventStopListening<PLSoundManagerTrackFadeEvent>();
				this.PLEventStopListening<PLSoundManagerSoundFadeEvent>();
				this.PLEventStopListening<PLSoundManagerAllSoundsControlEvent>();
            
				SceneManager.sceneLoaded -= OnSceneLoaded;
			}
		}
        
		#endregion
	}    
}