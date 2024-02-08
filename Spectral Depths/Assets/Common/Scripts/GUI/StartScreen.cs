using UnityEngine;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using SpectralDepths.Tools;
using SpectralDepths.PLInterface;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// Simple start screen class.
	/// </summary>
	[AddComponentMenu("Spectral Depths/GUI/StartScreen")]
	public class StartScreen : TopDownMonoBehaviour
	{
		/// the level to load after the start screen
		[Tooltip("the level to load after the start screen")]
		public string NextLevel;
		/// the name of the PLSceneLoadingManager scene you want to use
		[Tooltip("the name of the PLSceneLoadingManager scene you want to use")]
		public string LoadingSceneName = "";
		/// the delay after which the level should auto skip (if less than 1s, won't autoskip)
		[Tooltip("the delay after which the level should auto skip (if less than 1s, won't autoskip)")]
		public float AutoSkipDelay = 0f;

		[Header("Fades")]
		/// the duration of the fade from black at the start of the level
		[Tooltip("the duration of the fade from black at the start of the level")]
		public float FadeInDuration = 1f;
		/// the duration of the fade to black at the end of the level
		[Tooltip("the duration of the fade to black at the end of the level")]
		public float FadeOutDuration = 1f;
		/// the tween type to use to fade the startscreen in and out 
		[Tooltip("the tween type to use to fade the startscreen in and out ")]
		public PLTweenType Tween = new PLTweenType(PLTween.PLTweenCurve.EaseInOutCubic);

		[Header("Sound Settings Bindings")]
		/// the switch used to turn the music on or off
		[Tooltip("the switch used to turn the music on or off")]
		public PLSwitch MusicSwitch;
		/// the switch used to turn the SFX on or off
		[Tooltip("the switch used to turn the SFX on or off")]
		public PLSwitch SfxSwitch;

		/// <summary>
		/// Initialization
		/// </summary>
		protected virtual void Awake()
		{	
			GUIManager.Instance.SetHUDActive (false);
			PLFadeOutEvent.Trigger(FadeInDuration, Tween);
			Cursor.visible = true;
			if (AutoSkipDelay > 1f)
			{
				FadeOutDuration = AutoSkipDelay;
				StartCoroutine (LoadFirstLevel ());
			}
		}

		/// <summary>
		/// On Start, initializes the music and sfx switches
		/// </summary>
		protected async void Start()
		{
			await Task.Delay(1);
			
			if (MusicSwitch != null)
			{
				MusicSwitch.CurrentSwitchState = PLSoundManager.Instance.settingsSo.Settings.MusicOn ? PLSwitch.SwitchStates.Right : PLSwitch.SwitchStates.Left;
				MusicSwitch.InitializeState ();
			}

			if (SfxSwitch != null)
			{
				SfxSwitch.CurrentSwitchState = PLSoundManager.Instance.settingsSo.Settings.SfxOn ? PLSwitch.SwitchStates.Right : PLSwitch.SwitchStates.Left;
				SfxSwitch.InitializeState ();
			}
		}

		/// <summary>
		/// During update we simply wait for the user to press the "jump" button.
		/// </summary>
		protected virtual void Update()
		{
			if (!Input.GetButtonDown ("Player1_Jump"))
				return;
			
			ButtonPressed ();
		}

		/// <summary>
		/// What happens when the main button is pressed
		/// </summary>
		public virtual void ButtonPressed()
		{
			PLFadeInEvent.Trigger(FadeOutDuration, Tween);
			// if the user presses the "Jump" button, we start the first level.
			StartCoroutine (LoadFirstLevel ());
		}

		/// <summary>
		/// Loads the next level.
		/// </summary>
		/// <returns>The first level.</returns>
		protected virtual IEnumerator LoadFirstLevel()
		{
			yield return new WaitForSeconds (FadeOutDuration);
			if (LoadingSceneName == "")
			{
				PLAdditiveSceneLoadingManager.LoadScene (NextLevel);	
			}
			else
			{
				PLAdditiveSceneLoadingManager.LoadScene (NextLevel, LoadingSceneName);
			}
			
		}
	}
}