using System;
using System.Collections;
using System.Collections.Generic;
using SpectralDepths.Tools;
using UnityEngine;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// This class stores PLSoundManager settings and lets you tweak them from the PLSoundManagerSettingsSO's inspector
	/// </summary>
	[Serializable]
	public class PLSoundManagerSettings
	{
		public const float _minimalVolume = 0.0001f;
		public const float _maxVolume = 10f;
		public const float _defaultVolume = 1f;
        
		[Header("Audio Mixer Control")] 
		/// whether or not the settings described below should override the ones defined in the AudioMixer 
		[Tooltip("whether or not the settings described below should override the ones defined in the AudioMixer")]
		public bool OverrideMixerSettings = true;

		[Header("Audio Mixer Exposed Parameters")]
		/// the name of the exposed MasterVolume parameter in the AudioMixer
		[Tooltip("the name of the exposed MasterVolume parameter in the AudioMixer")]
		public string MasterVolumeParameter = "MasterVolume";
		/// the name of the exposed MusicVolume parameter in the AudioMixer
		[Tooltip("the name of the exposed MusicVolume parameter in the AudioMixer")]
		public string MusicVolumeParameter = "MusicVolume";
		/// the name of the exposed SfxVolume parameter in the AudioMixer
		[Tooltip("the name of the exposed SfxVolume parameter in the AudioMixer")]
		public string SfxVolumeParameter = "SfxVolume";
		/// the name of the exposed UIVolume parameter in the AudioMixer
		[Tooltip("the name of the exposed UIVolume parameter in the AudioMixer")]
		public string UIVolumeParameter = "UIVolume";
        
		[Header("Master")]
		/// the master volume
		[Range(_minimalVolume,_maxVolume)]
		[Tooltip("the master volume")]
		[PLReadOnly]
		public float MasterVolume = _defaultVolume;
		/// whether the master track is active at the moment or not
		[Tooltip("whether the master track is active at the moment or not")]
		[PLReadOnly] 
		public bool MasterOn = true;
		/// the volume of the master track before it was muted
		[Tooltip("the volume of the master track before it was muted")]
		[PLReadOnly] 
		public float MutedMasterVolume;

		[Header("Music")]
		/// the music volume
		[Range(_minimalVolume,_maxVolume)]
		[Tooltip("the music volume")]
		[PLReadOnly]
		public float MusicVolume = _defaultVolume; 
		/// whether the music track is active at the moment or not
		[Tooltip("whether the music track is active at the moment or not")]
		[PLReadOnly] 
		public bool MusicOn = true;
		/// the volume of the music track before it was muted
		[Tooltip("the volume of the music track before it was muted")]
		[PLReadOnly] 
		public float MutedMusicVolume;
        
		[Header("Sound Effects")]
		/// the sound fx volume
		[Range(_minimalVolume,_maxVolume)]
		[Tooltip("the sound fx volume")]
		[PLReadOnly]
		public float SfxVolume = _defaultVolume;
		/// whether the SFX track is active at the moment or not
		[Tooltip("whether the SFX track is active at the moment or not")]
		[PLReadOnly] 
		public bool SfxOn = true;
		/// the volume of the SFX track before it was muted
		[Tooltip("the volume of the SFX track before it was muted")]
		[PLReadOnly] 
		public float MutedSfxVolume;
        
		[Header("UI")]
		/// the UI sounds volume
		[Range(_minimalVolume,_maxVolume)]
		[Tooltip("the UI sounds volume")]
		[PLReadOnly]
		public float UIVolume = _defaultVolume;
		/// whether the UI track is active at the moment or not
		[Tooltip("whether the UI track is active at the moment or not")]
		[PLReadOnly] 
		public bool UIOn = true;
		/// the volume of the UI track before it was muted
		[Tooltip("the volume of the UI track before it was muted")]
		[PLReadOnly] 
		public float MutedUIVolume;
        
		[Header("Save & Load")]
		/// whether or not the PLSoundManager should automatically load settings when starting
		[Tooltip("whether or not the PLSoundManager should automatically load settings when starting")]
		public bool AutoLoad = true;
		/// whether or not each change in the settings should be automaticall saved. If not, you'll have to call a save PLSoundManager event for settings to be saved.
		[Tooltip("whether or not each change in the settings should be automaticall saved. If not, you'll have to call a save PLSoundManager event for settings to be saved.")]
		public bool AutoSave = false;
	}
}