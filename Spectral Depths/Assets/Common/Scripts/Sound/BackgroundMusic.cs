using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// Add this class to a GameObject to have it play a background music when instanciated.
	/// </summary>
	[AddComponentMenu("Spectral Depths/Sound/BackgroundMusic")]
	public class BackgroundMusic : TopDownMonoBehaviour
	{
		/// the background music
		[Tooltip("the audio clip to use as background music")]
		public AudioClip SoundClip;
		/// whether or not the music should loop
		[Tooltip("whether or not the music should loop")]
		public bool Loop = true;
		/// the ID to create this background music with
		[Tooltip("the ID to create this background music with")]
		public int ID = 255;


		/// <summary>
		/// Gets the AudioSource associated to that GameObject, and asks the GameManager to play it.
		/// </summary>
		protected virtual void Start()
		{
			PLSoundManagerPlayOptions options = PLSoundManagerPlayOptions.Default;
			options.ID = ID;
			options.Loop = Loop;
			options.Location = Vector3.zero;
			options.MmSoundManagerTrack = PLSoundManager.PLSoundManagerTracks.Music;
            
			PLSoundManagerSoundPlayEvent.Trigger(SoundClip, options);
		}
	}
}