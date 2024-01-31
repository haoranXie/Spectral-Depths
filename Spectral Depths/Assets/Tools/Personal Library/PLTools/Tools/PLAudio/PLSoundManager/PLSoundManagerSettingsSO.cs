using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A class to save sound settings (music on or off, sfx on or off)
	/// </summary>
	[Serializable]
	[CreateAssetMenu(menuName = "SpectralDepths/Audio/PLSoundManagerSettings")]
	public class PLSoundManagerSettingsSO : ScriptableObject
	{
		[Header("Audio Mixer")] 
		/// the audio mixer to use when playing sounds 
		[Tooltip("the audio mixer to use when playing sounds")]
		public AudioMixer TargetAudioMixer;
		/// the master group
		[Tooltip("the master group")]
		public AudioMixerGroup MasterAudioMixerGroup;
		/// the group on which to play all music sounds
		[Tooltip("the group on which to play all music sounds")]
		public AudioMixerGroup MusicAudioMixerGroup;
		/// the group on which to play all sound effects
		[Tooltip("the group on which to play all sound effects")]
		public AudioMixerGroup SfxAudioMixerGroup;
		/// the group on which to play all UI sounds
		[Tooltip("the group on which to play all UI sounds")]
		public AudioMixerGroup UIAudioMixerGroup;
		/// the multiplier to apply when converting normalized volume values to audio mixer values
		[Tooltip("the multiplier to apply when converting normalized volume values to audio mixer values")]
		public float MixerValuesMultiplier = 20;
        
		[Header("Settings Unfold")]
		/// the full settings for this PLSoundManager
		[Tooltip("the full settings for this PLSoundManager")]
		public PLSoundManagerSettings Settings;

		protected const string _saveFolderName = "PLSoundManager/";
		protected const string _saveFileName = "mmsound.settings";
    
		#region SaveAndLoad
        
		/// <summary>
		/// Saves the sound settings to file
		/// </summary>
		public virtual void SaveSoundSettings()
		{
			PLSaveLoadManager.Save(this.Settings, _saveFileName, _saveFolderName);
		}

		/// <summary>
		/// Loads the sound settings from file (if found)
		/// </summary>
		public virtual void LoadSoundSettings()
		{
			if (Settings.OverrideMixerSettings)
			{
				PLSoundManagerSettings settings =
					(PLSoundManagerSettings) PLSaveLoadManager.Load(typeof(PLSoundManagerSettings), _saveFileName,
						_saveFolderName);
				
				if (settings != null)
				{
					this.Settings = settings;
					ApplyTrackVolumes();
				}

				PLSoundManagerEvent.Trigger(PLSoundManagerEventTypes.SettingsLoaded);
			}
		}

		/// <summary>
		/// Resets the sound settings by destroying the save file
		/// </summary>
		public virtual void ResetSoundSettings()
		{
			PLSaveLoadManager.DeleteSave(_saveFileName, _saveFolderName);
		}
        
		#endregion
        
		#region Volume

		/// <summary>
		/// sets the volume of the selected track to the value passed in parameters
		/// </summary>
		/// <param name="track"></param>
		/// <param name="volume"></param>
		public virtual void SetTrackVolume(PLSoundManager.PLSoundManagerTracks track, float volume)
		{
			if (volume <= 0f)
			{
				volume = PLSoundManagerSettings._minimalVolume;
			}
            
			switch (track)
			{
				case PLSoundManager.PLSoundManagerTracks.Master:
					TargetAudioMixer.SetFloat(Settings.MasterVolumeParameter, NormalizedToMixerVolume(volume));
					Settings.MasterVolume = volume;
					break;
				case PLSoundManager.PLSoundManagerTracks.Music:
					TargetAudioMixer.SetFloat(Settings.MusicVolumeParameter, NormalizedToMixerVolume(volume));
					Settings.MusicVolume = volume;
					break;
				case PLSoundManager.PLSoundManagerTracks.Sfx:
					TargetAudioMixer.SetFloat(Settings.SfxVolumeParameter, NormalizedToMixerVolume(volume));
					Settings.SfxVolume = volume;
					break;
				case PLSoundManager.PLSoundManagerTracks.UI:
					TargetAudioMixer.SetFloat(Settings.UIVolumeParameter, NormalizedToMixerVolume(volume));
					Settings.UIVolume = volume;
					break;
			}

			if (Settings.AutoSave)
			{
				SaveSoundSettings();
			}
		}

		/// <summary>
		/// Returns the volume of the specified track
		/// </summary>
		/// <param name="track"></param>
		/// <returns></returns>
		public virtual float GetTrackVolume(PLSoundManager.PLSoundManagerTracks track)
		{
			float volume = 1f;
			switch (track)
			{
				case PLSoundManager.PLSoundManagerTracks.Master:
					TargetAudioMixer.GetFloat(Settings.MasterVolumeParameter, out volume);
					break;
				case PLSoundManager.PLSoundManagerTracks.Music:
					TargetAudioMixer.GetFloat(Settings.MusicVolumeParameter, out volume);
					break;
				case PLSoundManager.PLSoundManagerTracks.Sfx:
					TargetAudioMixer.GetFloat(Settings.SfxVolumeParameter, out volume);
					break;
				case PLSoundManager.PLSoundManagerTracks.UI:
					TargetAudioMixer.GetFloat(Settings.UIVolumeParameter, out volume);
					break;
			}

			return MixerVolumeToNormalized(volume);
		}

		/// <summary>
		/// assigns the volume of each track to the settings values
		/// </summary>
		public virtual void GetTrackVolumes()
		{
			Settings.MasterVolume = GetTrackVolume(PLSoundManager.PLSoundManagerTracks.Master);
			Settings.MusicVolume = GetTrackVolume(PLSoundManager.PLSoundManagerTracks.Music);
			Settings.SfxVolume = GetTrackVolume(PLSoundManager.PLSoundManagerTracks.Sfx);
			Settings.UIVolume = GetTrackVolume(PLSoundManager.PLSoundManagerTracks.UI);
		}

		/// <summary>
		/// applies volume to all tracks and saves if needed
		/// </summary>
		protected virtual void ApplyTrackVolumes()
		{
			if (Settings.OverrideMixerSettings)
			{
				TargetAudioMixer.SetFloat(Settings.MasterVolumeParameter, NormalizedToMixerVolume(Settings.MasterVolume));
				TargetAudioMixer.SetFloat(Settings.MusicVolumeParameter, NormalizedToMixerVolume(Settings.MusicVolume));
				TargetAudioMixer.SetFloat(Settings.SfxVolumeParameter, NormalizedToMixerVolume(Settings.SfxVolume));
				TargetAudioMixer.SetFloat(Settings.UIVolumeParameter, NormalizedToMixerVolume(Settings.UIVolume));

				if (!Settings.MasterOn) { TargetAudioMixer.SetFloat(Settings.MasterVolumeParameter, -80f); }
				if (!Settings.MusicOn) { TargetAudioMixer.SetFloat(Settings.MusicVolumeParameter, -80f); }
				if (!Settings.SfxOn) { TargetAudioMixer.SetFloat(Settings.SfxVolumeParameter, -80f); }
				if (!Settings.UIOn) { TargetAudioMixer.SetFloat(Settings.UIVolumeParameter, -80f); }

				if (Settings.AutoSave)
				{
					SaveSoundSettings();
				}
			}
		}
        
		/// <summary>
		/// Converts a normalized volume to the mixer group db scale
		/// </summary>
		/// <param name="normalizedVolume"></param>
		/// <returns></returns>
		public virtual float NormalizedToMixerVolume(float normalizedVolume)
		{
			return Mathf.Log10(normalizedVolume) * MixerValuesMultiplier;
		}

		/// <summary>
		/// Converts mixer volume to a normalized value
		/// </summary>
		/// <param name="mixerVolume"></param>
		/// <returns></returns>
		public virtual float MixerVolumeToNormalized(float mixerVolume)
		{
			return (float)Math.Pow(10, (mixerVolume / MixerValuesMultiplier));
		}
        
		#endregion Volume
	}
}