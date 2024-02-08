using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// Add this class to a GameObject to have it play a background music when instanciated.
	/// Careful : only one background music will be played at a time.
	/// </summary>
	[AddComponentMenu("Spectral Depths/Sound/PersistentBackgroundMusic")]
	public class PersistentBackgroundMusic : PLPersistentSingleton<PersistentBackgroundMusic>
	{
		/// the background music clip to use as persistent background music
		[Tooltip("the background music clip to use as persistent background music")]
		public AudioClip SoundClip;
		/// whether or not the music should loop
		[Tooltip("whether or not the music should loop")]
		public bool Loop = true;
        
		protected AudioSource _source;
		protected PersistentBackgroundMusic _otherBackgroundMusic;

		protected virtual void OnEnable()
		{
			_otherBackgroundMusic = (PersistentBackgroundMusic)FindObjectOfType(typeof(PersistentBackgroundMusic));
			if ((_otherBackgroundMusic != null) && (_otherBackgroundMusic != this) )
			{
				_otherBackgroundMusic.enabled = false;
			}
		}

		/// <summary>
		/// Gets the AudioSource associated to that GameObject, and asks the GameManager to play it.
		/// </summary>
		protected virtual void Start()
		{
			PLSoundManagerPlayOptions options = PLSoundManagerPlayOptions.Default;
			options.Loop = Loop;
			options.Location = Vector3.zero;
			options.MmSoundManagerTrack = PLSoundManager.PLSoundManagerTracks.Music;
			options.Persistent = true;
            
			PLSoundManagerSoundPlayEvent.Trigger(SoundClip, options);
		}
	}
}