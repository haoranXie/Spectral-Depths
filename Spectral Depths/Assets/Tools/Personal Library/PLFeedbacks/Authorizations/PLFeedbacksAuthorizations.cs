using SpectralDepths.FeedbacksForThirdParty;
using SpectralDepths.Tools;

namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// Add this class to an empty object in your scene and it will prevent any unchecked feedback in its inspector from playing
	/// </summary>
	public partial class PLFeedbacksAuthorizations : PLMonoBehaviour
	{
		[PLInspectorGroup("Animation", true, 16)] [PLInspectorButton("ToggleAnimation")]
		public bool ToggleAnimationButton;

		public bool AnimationParameter = true;
		public bool AnimatorSpeed = true;

		[PLInspectorGroup("Audio", true, 17)] [PLInspectorButton("ToggleAudio")]
		public bool ToggleAudioButton;

		public bool AudioFilterDistortion = true;
		public bool AudioFilterEcho = true;
		public bool AudioFilterHighPass = true;
		public bool AudioFilterLowPass = true;
		public bool AudioFilterReverb = true;
		public bool AudioMixerSnapshotTransition = true;
		public bool AudioSource = true;
		public bool AudioSourcePitch = true;
		public bool AudioSourceStereoPan = true;
		public bool AudioSourceVolume = true;
		public bool PLPlaylist = true;
		public bool PLSoundManagerAllSoundsControl = true;
		public bool PLSoundManagerSaveAndLoad = true;
		public bool PLSoundManagerSound = true;
		public bool PLSoundManagerSoundControl = true;
		public bool PLSoundManagerSoundFade = true;
		public bool PLSoundManagerTrackControl = true;
		public bool PLSoundManagerTrackFade = true;
		public bool Sound = true;

		[PLInspectorGroup("Camera", true, 18)] [PLInspectorButton("ToggleCamera")]
		public bool ToggleCameraButton;

		public bool CameraShake = true;
		public bool CameraZoom = true;
		#if PL_CINEMACHINE
		public bool CinemachineImpulse = true;
		public bool CinemachineImpulseClear = true;
		public bool CinemachineImpulseSource = true;
		public bool CinemachineTransition = true;
		#endif
		public bool ClippingPlanes = true;
		public bool Fade = true;
		public bool FieldOfView = true;
		public bool Flash = true;
		public bool OrthographicSize = true;

		[PLInspectorGroup("Debug", true, 19)] [PLInspectorButton("ToggleDebug")]
		public bool ToggleDebugButton;

		public bool Comment = true;
		public bool Log = true;

		[PLInspectorGroup("Events", true, 20)] [PLInspectorButton("ToggleEvents")]
		public bool ToggleEventsButton;

		public bool PLGameEvent = true;
		public bool UnityEvents = true;

		[PLInspectorGroup("GameObject", true, 47)] [PLInspectorButton("ToggleGameObject")]
		public bool ToggleGameObjectButton;

		public bool Broadcast = true;
		public bool Collider = true;
		public bool Collider2D = true;
		public bool DestroyTargetObject = true;
		public bool EnableBehaviour = true;
		public bool FloatController = true;
		public bool InstantiateObject = true;
		public bool PLRadioSignal = true;
		public bool Rigidbody = true;
		public bool Rigidbody2D = true;
		public bool SetActive = true;

		
		#if MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
		[PLInspectorGroup("Haptics", true, 22)] [PLInspectorButton("ToggleHaptics")]
		public bool ToggleHapticsButton;

		public bool HapticClip = true;
		public bool HapticContinuous = true;
		public bool HapticControl = true;
		public bool HapticEmphasis = true;
		public bool HapticPreset = true;
		#endif

		[PLInspectorGroup("Light", true, 23)] [PLInspectorButton("ToggleLight")]
		public bool ToggleLightButton;

		public bool Light = true;

		[PLInspectorGroup("Loop", true, 24)] [PLInspectorButton("ToggleLoop")]
		public bool ToggleLoopButton;

		public bool Looper = true;
		public bool LooperStart = true;

		[PLInspectorGroup("Particles", true, 25)] [PLInspectorButton("ToggleParticles")]
		public bool ToggleParticlesButton;

		public bool ParticlesInstantiation = true;
		public bool ParticlesPlay = true;

		[PLInspectorGroup("Pause", true, 26)] [PLInspectorButton("TogglePause")]
		public bool TogglePauseButton;

		public bool HoldingPause = true;
		public bool Pause = true;

		[PLInspectorGroup("Post Process", true, 27)] [PLInspectorButton("TogglePostProcess")]
		public bool TogglePostProcessButton;

		public bool Bloom = true;
		public bool ChromaticAberration = true;
		public bool ColorGrading = true;
		public bool DepthOfField = true;
		public bool GlobalPPVolumeAutoBlend = true;
		public bool LensDistortion = true;
		public bool PPMovingFilter = true;
		public bool Vignette = true;

		[PLInspectorGroup("Flicker", true, 28)] [PLInspectorButton("ToggleFlicker")]
		public bool ToggleFlickerButton;

		public bool Flicker = true;
		public bool Fog = true;
		public bool Material = true;
		public bool PLBlink = true;
		public bool ShaderGlobal = true;
		public bool ShaderController = true;
		public bool Skybox = true;
		public bool SpriteRenderer = true;
		public bool TextureOffset = true;
		public bool TextureScale = true;

		[PLInspectorGroup("Scene", true, 29)] [PLInspectorButton("ToggleScene")]
		public bool ToggleSceneButton;

		public bool LoadScene = true;
		public bool UnloadScene = true;

		[PLInspectorGroup("Time", true, 31)] [PLInspectorButton("ToggleTime")]
		public bool ToggleTimeButton;

		public bool FreezeFrame = true;
		public bool TimescaleModifier = true;

		[PLInspectorGroup("Transform", true, 32)] [PLInspectorButton("ToggleTransform")]
		public bool ToggleTransformButton;

		public bool Destination = true;
		public bool Position = true;
		public bool PositionShake = true;
		public bool RotatePositionAround = true;
		public bool Rotation = true;
		public bool RotationShake = true;
		public bool Scale = true;
		public bool ScaleShake = true;
		public bool SquashAndStretch = true;
		public bool Wiggle = true;

		[PLInspectorGroup("UI", true, 33)] [PLInspectorButton("ToggleUI")]
		public bool ToggleUiButton;

		public bool CanvasGroup = true;
		public bool CanvasGroupBlocksRaycasts = true;
		public bool FloatingText = true;
		public bool Graphic = true;
		public bool GraphicCrossFade = true;
		public bool Image = true;
		public bool ImageAlpha = true;
		public bool ImageFill = true;
		public bool ImageRaycastTarget = true;
		public bool ImageTextureOffset = true;
		public bool ImageTextureScale = true;
		public bool RectTransformAnchor = true;
		public bool RectTransformOffset = true;
		public bool RectTransformPivot = true;
		public bool RectTransformSizeDelta = true;
		public bool Text = true;
		public bool TextColor = true;
		public bool TextFontSize = true;
		public bool VideoPlayer = true;
		
		[PLInspectorGroup("TextMesh Pro", true, 30)] [PLInspectorButton("ToggleTextMeshPro")]
		public bool ToggleTextMeshProButton;

		#if PL_TEXTMESHPRO
		public bool TMPAlpha = true;
		public bool TMPCharacterSpacing = true;
		public bool TMPColor = true;
		public bool TMPCountTo = true;
		public bool TMPDilate = true;
		public bool TMPFontSize = true;
		public bool TMPLineSpacing = true;
		public bool TMPOutlineColor = true;
		public bool TMPOutlineWidth = true;
		public bool TMPParagraphSpacing = true;
		public bool TMPSoftness = true;
		public bool TMPText = true;
		public bool TMPTextReveal = true;
		public bool TMPWordSpacing = true;
		#endif
		
		#region ToggleMethods
		
		private void ToggleAnimation()
		{
			AnimationParameter = !AnimationParameter;
			AnimatorSpeed = !AnimatorSpeed;
		}

		private void ToggleAudio()
		{
			AudioFilterDistortion = !AudioFilterDistortion;
			AudioFilterEcho = !AudioFilterEcho;
			AudioFilterHighPass = !AudioFilterHighPass;
			AudioFilterLowPass = !AudioFilterLowPass;
			AudioFilterReverb = !AudioFilterReverb;
			AudioMixerSnapshotTransition = !AudioMixerSnapshotTransition;
			AudioSource = !AudioSource;
			AudioSourcePitch = !AudioSourcePitch;
			AudioSourceStereoPan = !AudioSourceStereoPan;
			AudioSourceVolume = !AudioSourceVolume;
			PLPlaylist = !PLPlaylist;
			PLSoundManagerAllSoundsControl = !PLSoundManagerAllSoundsControl;
			PLSoundManagerSaveAndLoad = !PLSoundManagerSaveAndLoad;
			PLSoundManagerSound = !PLSoundManagerSound;
			PLSoundManagerSoundControl = !PLSoundManagerSoundControl;
			PLSoundManagerSoundFade = !PLSoundManagerSoundFade;
			PLSoundManagerTrackControl = !PLSoundManagerTrackControl;
			PLSoundManagerTrackFade = !PLSoundManagerTrackFade;
			Sound = !Sound;
		}

		private void ToggleCamera()
		{
			CameraShake = !CameraShake;
			CameraZoom = !CameraZoom;
			#if PL_CINEMACHINE
			CinemachineImpulse = !CinemachineImpulse;
			CinemachineImpulseClear = !CinemachineImpulseClear;
			CinemachineImpulseSource = !CinemachineImpulseSource;
			CinemachineTransition = !CinemachineTransition;
			#endif
			ClippingPlanes = !ClippingPlanes;
			Fade = !Fade;
			FieldOfView = !FieldOfView;
			Flash = !Flash;
			OrthographicSize = !OrthographicSize;
		}

		private void ToggleDebug()
		{
			Comment = !Comment;
			Log = !Log;
		}

		private void ToggleEvents()
		{
			PLGameEvent = !PLGameEvent;
			UnityEvents = !UnityEvents;
		}

		private void ToggleGameObject()
		{
			Broadcast = !Broadcast;
			Collider = !Collider;
			Collider2D = !Collider2D;
			DestroyTargetObject = !DestroyTargetObject;
			EnableBehaviour = !EnableBehaviour;
			FloatController = !FloatController;
			InstantiateObject = !InstantiateObject;
			PLRadioSignal = !PLRadioSignal;
			Rigidbody = !Rigidbody;
			Rigidbody2D = !Rigidbody2D;
			SetActive = !SetActive;
		}
		
		#if MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
		private void ToggleHaptics()
		{
			HapticClip = !HapticClip;
			HapticContinuous = !HapticContinuous;
			HapticControl = !HapticControl;
			HapticEmphasis = !HapticEmphasis;
			HapticPreset = !HapticPreset;
		}
		#endif

		private void ToggleLight()
		{
			Light = !Light;
		}

		private void ToggleLoop()
		{
			Looper = !Looper;
			LooperStart = !LooperStart;
		}

		private void ToggleParticles()
		{
			ParticlesInstantiation = !ParticlesInstantiation;
			ParticlesPlay = !ParticlesPlay;
		}

		private void TogglePause()
		{
			HoldingPause = !HoldingPause;
			Pause = !Pause;
		}

		#if PL_POSTPROCESSING
		private void TogglePostProcess()
		{
			Bloom = !Bloom;
			ChromaticAberration = !ChromaticAberration;
			ColorGrading = !ColorGrading;
			DepthOfField = !DepthOfField;
			GlobalPPVolumeAutoBlend = !GlobalPPVolumeAutoBlend;
			LensDistortion = !LensDistortion;
			PPMovingFilter = !PPMovingFilter;
			Vignette = !Vignette;
		}
		#endif

		private void ToggleFlicker()
		{
			Flicker = !Flicker;
			Fog = !Fog;
			Material = !Material;
			PLBlink = !PLBlink;
			ShaderGlobal = !ShaderGlobal;
			ShaderController = !ShaderController;
			Skybox = !Skybox;
			SpriteRenderer = !SpriteRenderer;
			TextureOffset = !TextureOffset;
			TextureScale = !TextureScale;
		}

		private void ToggleScene()
		{
			LoadScene = !LoadScene;
			UnloadScene = !UnloadScene;
		}

		private void ToggleTime()
		{
			FreezeFrame = !FreezeFrame;
			TimescaleModifier = !TimescaleModifier;
		}

		private void ToggleTransform()
		{
			Destination = !Destination;
			Position = !Position;
			PositionShake = !PositionShake;
			RotatePositionAround = !RotatePositionAround;
			Rotation = !Rotation;
			RotationShake = !RotationShake;
			Scale = !Scale;
			ScaleShake = !ScaleShake;
			SquashAndStretch = !SquashAndStretch;
			Wiggle = !Wiggle;
		}

		private void ToggleUI()
		{
			CanvasGroup = !CanvasGroup;
			CanvasGroupBlocksRaycasts = !CanvasGroupBlocksRaycasts;
			FloatingText = !FloatingText;
			Graphic = !Graphic;
			GraphicCrossFade = !GraphicCrossFade;
			Image = !Image;
			ImageAlpha = !ImageAlpha;
			ImageFill = !ImageFill;
			ImageRaycastTarget = !ImageRaycastTarget;
			ImageTextureOffset = !ImageTextureOffset;
			ImageTextureScale = !ImageTextureScale;
			RectTransformAnchor = !RectTransformAnchor;
			RectTransformOffset = !RectTransformOffset;
			RectTransformPivot = !RectTransformPivot;
			RectTransformSizeDelta = !RectTransformSizeDelta;
			Text = !Text;
			TextColor = !TextColor;
			TextFontSize = !TextFontSize;
			VideoPlayer = !VideoPlayer;
		}
		
		#if PL_TEXTMESHPRO
		private void ToggleTextMeshPro()
		{
			TMPAlpha = !TMPAlpha;
			TMPCharacterSpacing = !TMPCharacterSpacing;
			TMPColor = !TMPColor;
			TMPCountTo = !TMPCountTo;
			TMPDilate = !TMPDilate;
			TMPFontSize = !TMPFontSize;
			TMPLineSpacing = !TMPLineSpacing;
			TMPOutlineColor = !TMPOutlineColor;
			TMPOutlineWidth = !TMPOutlineWidth;
			TMPParagraphSpacing = !TMPParagraphSpacing;
			TMPSoftness = !TMPSoftness;
			TMPText = !TMPText;
			TMPTextReveal = !TMPTextReveal;
			TMPWordSpacing = !TMPWordSpacing;
		}
		#endif
		
		#endregion

		private void Start()
		{
			
			PLF_Animation.FeedbackTypeAuthorized = AnimationParameter;
			PLFeedbackAnimation.FeedbackTypeAuthorized = AnimationParameter;
			  
			PLF_AnimatorSpeed.FeedbackTypeAuthorized = AnimatorSpeed;
			PLFeedbackAnimatorSpeed.FeedbackTypeAuthorized = AnimatorSpeed;
			  
			PLF_AudioFilterDistortion.FeedbackTypeAuthorized = AudioFilterDistortion;
			PLFeedbackAudioFilterDistortion.FeedbackTypeAuthorized = AudioFilterDistortion;
			  
			PLF_AudioFilterEcho.FeedbackTypeAuthorized = AudioFilterEcho;
			PLFeedbackAudioFilterEcho.FeedbackTypeAuthorized = AudioFilterEcho;
			  
			PLF_AudioFilterHighPass.FeedbackTypeAuthorized = AudioFilterHighPass;
			PLFeedbackAudioFilterHighPass.FeedbackTypeAuthorized = AudioFilterHighPass;
			  
			PLF_AudioFilterLowPass.FeedbackTypeAuthorized = AudioFilterLowPass;
			PLFeedbackAudioFilterLowPass.FeedbackTypeAuthorized = AudioFilterLowPass;
			  
			PLF_AudioFilterReverb.FeedbackTypeAuthorized = AudioFilterReverb;
			PLFeedbackAudioFilterReverb.FeedbackTypeAuthorized = AudioFilterReverb;
			  
			PLF_AudioMixerSnapshotTransition.FeedbackTypeAuthorized = AudioMixerSnapshotTransition;
			PLFeedbackAudioMixerSnapshotTransition.FeedbackTypeAuthorized = AudioMixerSnapshotTransition;
			  
			PLF_AudioSource.FeedbackTypeAuthorized = AudioSource;
			PLFeedbackAudioSource.FeedbackTypeAuthorized = AudioSource;
			  
			PLF_AudioSourcePitch.FeedbackTypeAuthorized = AudioSourcePitch;
			PLFeedbackAudioSourcePitch.FeedbackTypeAuthorized = AudioSourcePitch;
			  
			PLF_AudioSourceStereoPan.FeedbackTypeAuthorized = AudioSourceStereoPan;
			PLFeedbackAudioSourceStereoPan.FeedbackTypeAuthorized = AudioSourceStereoPan;
			  
			PLF_AudioSourceVolume.FeedbackTypeAuthorized = AudioSourceVolume;
			PLFeedbackAudioSourceVolume.FeedbackTypeAuthorized = AudioSourceVolume;
		  	
			PLF_Playlist.FeedbackTypeAuthorized = PLPlaylist;
			PLFeedbackPlaylist.FeedbackTypeAuthorized = PLPlaylist;
			  
			PLF_MMSoundManagerAllSoundsControl.FeedbackTypeAuthorized = PLSoundManagerAllSoundsControl;
			PLFeedbackMMSoundManagerAllSoundsControl.FeedbackTypeAuthorized = PLSoundManagerAllSoundsControl;
		  
			PLF_MMSoundManagerSaveLoad.FeedbackTypeAuthorized = PLSoundManagerSaveAndLoad;
			PLFeedbackMMSoundManagerSaveLoad.FeedbackTypeAuthorized = PLSoundManagerSaveAndLoad;
			  
			PLF_MMSoundManagerSound.FeedbackTypeAuthorized = PLSoundManagerSound;
			PLFeedbackMMSoundManagerSound.FeedbackTypeAuthorized = PLSoundManagerSound;
			  
			PLF_MMSoundManagerSoundControl.FeedbackTypeAuthorized = PLSoundManagerSoundControl;
			PLFeedbackMMSoundManagerSoundControl.FeedbackTypeAuthorized = PLSoundManagerSoundControl;
			  
			PLF_MMSoundManagerSoundFade.FeedbackTypeAuthorized = PLSoundManagerSoundFade;
			PLFeedbackMMSoundManagerSoundFade.FeedbackTypeAuthorized = PLSoundManagerSoundFade;
			  
			PLF_MMSoundManagerTrackControl.FeedbackTypeAuthorized = PLSoundManagerTrackControl;
			PLFeedbackMMSoundManagerTrackControl.FeedbackTypeAuthorized = PLSoundManagerTrackControl;
			  
			PLF_MMSoundManagerTrackFade.FeedbackTypeAuthorized = PLSoundManagerTrackFade;
			PLFeedbackMMSoundManagerTrackFade.FeedbackTypeAuthorized = PLSoundManagerTrackFade;
			  
			PLF_Sound.FeedbackTypeAuthorized = Sound;
			PLFeedbackSound.FeedbackTypeAuthorized = Sound;
			  
			PLF_CameraShake.FeedbackTypeAuthorized = CameraShake;
			PLFeedbackCameraShake.FeedbackTypeAuthorized = CameraShake;
			  
			PLF_CameraZoom.FeedbackTypeAuthorized = CameraZoom;
			PLFeedbackCameraZoom.FeedbackTypeAuthorized = CameraZoom;
		  
			#if PL_CINEMACHINE
			PLF_CinemachineImpulse.FeedbackTypeAuthorized = CinemachineImpulse;
			PLFeedbackCinemachineImpulse.FeedbackTypeAuthorized = CinemachineImpulse;
			  
			PLF_CinemachineImpulseClear.FeedbackTypeAuthorized = CinemachineImpulseClear;
			PLFeedbackCinemachineImpulseClear.FeedbackTypeAuthorized = CinemachineImpulseClear;
			  
			PLF_CinemachineImpulseSource.FeedbackTypeAuthorized = CinemachineImpulseSource;
			  
			PLF_CinemachineTransition.FeedbackTypeAuthorized = CinemachineTransition;
			PLFeedbackCinemachineTransition.FeedbackTypeAuthorized = CinemachineTransition;
			#endif
		  
			PLF_CameraClippingPlanes.FeedbackTypeAuthorized = ClippingPlanes;
			PLFeedbackCameraClippingPlanes.FeedbackTypeAuthorized = ClippingPlanes;
			  
			PLF_Fade.FeedbackTypeAuthorized = Fade;
			PLFeedbackFade.FeedbackTypeAuthorized = Fade;
		  
			PLF_CameraFieldOfView.FeedbackTypeAuthorized = FieldOfView;
			PLFeedbackCameraFieldOfView.FeedbackTypeAuthorized = FieldOfView;
			  
			PLF_Flash.FeedbackTypeAuthorized = Flash;
			PLFeedbackFlash.FeedbackTypeAuthorized = Flash;
		  
			PLF_CameraOrthographicSize.FeedbackTypeAuthorized = OrthographicSize;
			PLFeedbackCameraOrthographicSize.FeedbackTypeAuthorized = OrthographicSize;
			  
			PLF_DebugComment.FeedbackTypeAuthorized = Comment;
			PLFeedbackDebugComment.FeedbackTypeAuthorized = Comment;
			  
			PLF_DebugLog.FeedbackTypeAuthorized = Log;
			PLFeedbackDebugLog.FeedbackTypeAuthorized = Log;
			  
			PLF_MMGameEvent.FeedbackTypeAuthorized = PLGameEvent;
			PLFeedbackMMGameEvent.FeedbackTypeAuthorized = PLGameEvent;
		  
			PLF_Events.FeedbackTypeAuthorized = UnityEvents;
			PLFeedbackEvents.FeedbackTypeAuthorized = UnityEvents;
		  
			PLF_Broadcast.FeedbackTypeAuthorized = Broadcast;
			PLFeedbackBroadcast.FeedbackTypeAuthorized = Broadcast;
		  
			PLF_Collider.FeedbackTypeAuthorized = Collider;
			PLFeedbackCollider.FeedbackTypeAuthorized = Collider;
			  
			PLF_Collider2D.FeedbackTypeAuthorized = Collider2D;
			PLFeedbackCollider2D.FeedbackTypeAuthorized = Collider2D;
			  
			PLF_Destroy.FeedbackTypeAuthorized = DestroyTargetObject;
			PLFeedbackDestroy.FeedbackTypeAuthorized = DestroyTargetObject;
			  
			PLF_Enable.FeedbackTypeAuthorized = EnableBehaviour;
			PLFeedbackEnable.FeedbackTypeAuthorized = EnableBehaviour;
			  
			PLF_FloatController.FeedbackTypeAuthorized = FloatController;
			PLFeedbackFloatController.FeedbackTypeAuthorized = FloatController;
			  
			PLF_InstantiateObject.FeedbackTypeAuthorized = InstantiateObject;
			PLFeedbackInstantiateObject.FeedbackTypeAuthorized = InstantiateObject;
		  
			PLF_RadioSignal.FeedbackTypeAuthorized = PLRadioSignal;
			PLFeedbackRadioSignal.FeedbackTypeAuthorized = PLRadioSignal;
		  
			PLF_Rigidbody.FeedbackTypeAuthorized = Rigidbody;
			PLFeedbackRigidbody.FeedbackTypeAuthorized = Rigidbody;
			  
			PLF_Rigidbody2D.FeedbackTypeAuthorized = Rigidbody2D;
			PLFeedbackRigidbody2D.FeedbackTypeAuthorized = Rigidbody2D;
			  
			PLF_SetActive.FeedbackTypeAuthorized = SetActive;
			PLFeedbackSetActive.FeedbackTypeAuthorized = SetActive;
		  
			#if MOREMOUNTAINS_NICEVIBRATIONS_INSTALLED
			PLF_Haptics.FeedbackTypeAuthorized = HapticClip;
			PLFeedbackHaptics.FeedbackTypeAuthorized = HapticClip;

			PLF_NVContinuous.FeedbackTypeAuthorized = HapticContinuous;
			PLFeedbackNVContinuous.FeedbackTypeAuthorized = HapticContinuous;
			  
			PLF_NVControl.FeedbackTypeAuthorized = HapticControl;
			PLFeedbackNVControl.FeedbackTypeAuthorized = HapticControl;
			  
			PLF_NVEmphasis.FeedbackTypeAuthorized = HapticEmphasis;
			PLFeedbackNVEmphasis.FeedbackTypeAuthorized = HapticEmphasis;
			  
			PLF_NVPreset.FeedbackTypeAuthorized = HapticPreset;
			PLFeedbackNVPreset.FeedbackTypeAuthorized = HapticPreset;
			#endif
  
			PLF_Light.FeedbackTypeAuthorized = Light;
			PLFeedbackLight.FeedbackTypeAuthorized = Light;
		  
			PLF_Looper.FeedbackTypeAuthorized = Looper;
			PLFeedbackLooper.FeedbackTypeAuthorized = Looper;
			  
			PLF_LooperStart.FeedbackTypeAuthorized = LooperStart;
			PLFeedbackLooperStart.FeedbackTypeAuthorized = LooperStart;
			  
			PLF_ParticlesInstantiation.FeedbackTypeAuthorized = ParticlesInstantiation;
			PLFeedbackParticlesInstantiation.FeedbackTypeAuthorized = ParticlesInstantiation;
			  
			PLF_Particles.FeedbackTypeAuthorized = ParticlesPlay;
			PLFeedbackParticles.FeedbackTypeAuthorized = ParticlesPlay;
			  
			PLF_HoldingPause.FeedbackTypeAuthorized = HoldingPause;
			PLFeedbackHoldingPause.FeedbackTypeAuthorized = HoldingPause;
		  
			PLF_Pause.FeedbackTypeAuthorized = Pause;
			PLFeedbackPause.FeedbackTypeAuthorized = Pause;

			PLF_Flicker.FeedbackTypeAuthorized = Flicker;
			PLFeedbackFlicker.FeedbackTypeAuthorized = Flicker;
			  
			PLF_Fog.FeedbackTypeAuthorized = Fog;
			PLFeedbackFog.FeedbackTypeAuthorized = Fog;
			  
			PLF_Material.FeedbackTypeAuthorized = Material;
			PLFeedbackMaterial.FeedbackTypeAuthorized = Material;
		  
			PLF_Blink.FeedbackTypeAuthorized = PLBlink;
			PLFeedbackBlink.FeedbackTypeAuthorized = PLBlink;
			  
			PLF_ShaderGlobal.FeedbackTypeAuthorized = ShaderGlobal;
			PLFeedbackShaderGlobal.FeedbackTypeAuthorized = ShaderGlobal;
			  
			PLF_ShaderController.FeedbackTypeAuthorized = ShaderController;
			PLFeedbackShaderController.FeedbackTypeAuthorized = ShaderController;
			  
			PLF_Skybox.FeedbackTypeAuthorized = Skybox;
			PLFeedbackSkybox.FeedbackTypeAuthorized = Skybox;
			  
			PLF_SpriteRenderer.FeedbackTypeAuthorized = SpriteRenderer;
			PLFeedbackSpriteRenderer.FeedbackTypeAuthorized = SpriteRenderer;
			  
			PLF_TextureOffset.FeedbackTypeAuthorized = TextureOffset;
			PLFeedbackTextureOffset.FeedbackTypeAuthorized = TextureOffset;
			  
			PLF_TextureScale.FeedbackTypeAuthorized = TextureScale;
			PLFeedbackTextureScale.FeedbackTypeAuthorized = TextureScale;
			  
			PLF_LoadScene.FeedbackTypeAuthorized = LoadScene;
			PLFeedbackLoadScene.FeedbackTypeAuthorized = LoadScene;
		  
			PLF_UnloadScene.FeedbackTypeAuthorized = UnloadScene;
			PLFeedbackUnloadScene.FeedbackTypeAuthorized = UnloadScene;
		  
			PLF_FreezeFrame.FeedbackTypeAuthorized = FreezeFrame;
			PLFeedbackFreezeFrame.FeedbackTypeAuthorized = FreezeFrame;
			  
			PLF_TimescaleModifier.FeedbackTypeAuthorized = TimescaleModifier;
			PLFeedbackTimescaleModifier.FeedbackTypeAuthorized = TimescaleModifier;
			  
			PLF_DestinationTransform.FeedbackTypeAuthorized = Destination;
			PLFeedbackDestinationTransform.FeedbackTypeAuthorized = Destination;
			  
			PLF_Position.FeedbackTypeAuthorized = Position;
			PLFeedbackPosition.FeedbackTypeAuthorized = Position;
			  
			PLF_PositionShake.FeedbackTypeAuthorized = PositionShake;
			  
			PLF_RotatePositionAround.FeedbackTypeAuthorized = RotatePositionAround;
			  
			PLF_Rotation.FeedbackTypeAuthorized = Rotation;
			PLFeedbackRotation.FeedbackTypeAuthorized = Rotation;
			  
			PLF_RotationShake.FeedbackTypeAuthorized = RotationShake;
			  
			PLF_Scale.FeedbackTypeAuthorized = Scale;
			PLFeedbackScale.FeedbackTypeAuthorized = Scale;
			  
			PLF_ScaleShake.FeedbackTypeAuthorized = ScaleShake;
			  
			PLF_SquashAndStretch.FeedbackTypeAuthorized = SquashAndStretch;
			PLFeedbackSquashAndStretch.FeedbackTypeAuthorized = SquashAndStretch;
			  
			PLF_Wiggle.FeedbackTypeAuthorized = Wiggle;
			PLFeedbackWiggle.FeedbackTypeAuthorized = Wiggle;
		  
			PLF_CanvasGroup.FeedbackTypeAuthorized = CanvasGroup;
			PLFeedbackCanvasGroup.FeedbackTypeAuthorized = CanvasGroup;
		  
			PLF_CanvasGroupBlocksRaycasts.FeedbackTypeAuthorized = CanvasGroupBlocksRaycasts;
			PLFeedbackCanvasGroupBlocksRaycasts.FeedbackTypeAuthorized = CanvasGroupBlocksRaycasts;
			  
			PLF_FloatingText.FeedbackTypeAuthorized = FloatingText;
			PLFeedbackFloatingText.FeedbackTypeAuthorized = FloatingText;
			  
			PLF_Graphic.FeedbackTypeAuthorized = Graphic;
			  
			PLF_GraphicCrossFade.FeedbackTypeAuthorized = GraphicCrossFade;
			  
			PLF_Image.FeedbackTypeAuthorized = Image;
			PLFeedbackImage.FeedbackTypeAuthorized = Image;
			  
			PLF_ImageAlpha.FeedbackTypeAuthorized = ImageAlpha;
			PLFeedbackImageAlpha.FeedbackTypeAuthorized = ImageAlpha;
			  
			PLF_ImageFill.FeedbackTypeAuthorized = ImageFill;
			  
			PLF_ImageRaycastTarget.FeedbackTypeAuthorized = ImageRaycastTarget;
			PLFeedbackImageRaycastTarget.FeedbackTypeAuthorized = ImageRaycastTarget;
			  
			PLF_ImageTextureOffset.FeedbackTypeAuthorized = ImageTextureOffset;
			  
			PLF_ImageTextureScale.FeedbackTypeAuthorized = ImageTextureScale;
			
			PLF_RectTransformAnchor.FeedbackTypeAuthorized = RectTransformAnchor;
			PLFeedbackRectTransformAnchor.FeedbackTypeAuthorized = RectTransformAnchor;
		  
			PLF_RectTransformOffset.FeedbackTypeAuthorized = RectTransformOffset;
			PLFeedbackRectTransformOffset.FeedbackTypeAuthorized = RectTransformOffset;
		  
			PLF_RectTransformPivot.FeedbackTypeAuthorized = RectTransformPivot;
			PLFeedbackRectTransformPivot.FeedbackTypeAuthorized = RectTransformPivot;
		  
			PLF_RectTransformSizeDelta.FeedbackTypeAuthorized = RectTransformSizeDelta;
			PLFeedbackRectTransformSizeDelta.FeedbackTypeAuthorized = RectTransformSizeDelta;
		  
			PLF_Text.FeedbackTypeAuthorized = Text;
			PLFeedbackText.FeedbackTypeAuthorized = Text;
			  
			PLF_TextColor.FeedbackTypeAuthorized = TextColor;
			PLFeedbackTextColor.FeedbackTypeAuthorized = TextColor;
		  
			PLF_TextFontSize.FeedbackTypeAuthorized = TextFontSize;
			PLFeedbackTextFontSize.FeedbackTypeAuthorized = TextFontSize;
		  
			PLF_VideoPlayer.FeedbackTypeAuthorized = VideoPlayer;
			PLFeedbackVideoPlayer.FeedbackTypeAuthorized = VideoPlayer;
			
			#if PL_POSTPROCESSING
			PLF_Bloom.FeedbackTypeAuthorized = Bloom;
			PLFeedbackBloom.FeedbackTypeAuthorized = Bloom;
			  
			PLF_ChromaticAberration.FeedbackTypeAuthorized = ChromaticAberration;
			PLFeedbackChromaticAberration.FeedbackTypeAuthorized = ChromaticAberration;
			  
			PLF_ColorGrading.FeedbackTypeAuthorized = ColorGrading;
			PLFeedbackColorGrading.FeedbackTypeAuthorized = ColorGrading;
			  
			PLF_DepthOfField.FeedbackTypeAuthorized = DepthOfField;
			PLFeedbackDepthOfField.FeedbackTypeAuthorized = DepthOfField;
		  
			PLF_GlobalPPVolumeAutoBlend.FeedbackTypeAuthorized = GlobalPPVolumeAutoBlend;
			PLFeedbackGlobalPPVolumeAutoBlend.FeedbackTypeAuthorized = GlobalPPVolumeAutoBlend;
			  
			PLF_LensDistortion.FeedbackTypeAuthorized = LensDistortion;
			PLFeedbackLensDistortion.FeedbackTypeAuthorized = LensDistortion;
			  
			PLF_Vignette.FeedbackTypeAuthorized = Vignette;
			PLFeedbackVignette.FeedbackTypeAuthorized = Vignette;
			  
			PLF_PPMovingFilter.FeedbackTypeAuthorized = PPMovingFilter;
			PLFeedbackPPMovingFilter.FeedbackTypeAuthorized = PPMovingFilter;
			#endif
			
			#if PL_HDRP
			
			PLF_Bloom_HDRP.FeedbackTypeAuthorized = Bloom;
			PLF_ChromaticAberration_HDRP.FeedbackTypeAuthorized = ChromaticAberration;
			PLF_LensDistortion_HDRP.FeedbackTypeAuthorized = LensDistortion;
			PLF_ColorAdjustments_HDRP.FeedbackTypeAuthorized = ColorGrading;
			PLF_LensDistortion_HDRP.FeedbackTypeAuthorized = LensDistortion;
			PLF_Vignette_HDRP.FeedbackTypeAuthorized = Vignette;
			
			#endif
			
			#if PL_URP
			
			PLF_Bloom_URP.FeedbackTypeAuthorized = Bloom;
			PLF_ChromaticAberration_URP.FeedbackTypeAuthorized = ChromaticAberration;
			PLF_LensDistortion_URP.FeedbackTypeAuthorized = LensDistortion;
			PLF_ColorAdjustments_URP.FeedbackTypeAuthorized = ColorGrading;
			PLF_LensDistortion_URP.FeedbackTypeAuthorized = LensDistortion;
			PLF_Vignette_URP.FeedbackTypeAuthorized = Vignette;
			
			#endif
			
			#if PL_TEXTMESHPRO
			PLF_TMPAlpha.FeedbackTypeAuthorized = TMPAlpha;
			PLFeedbackTMPAlpha.FeedbackTypeAuthorized = TMPAlpha;
			  
			PLF_TMPCharacterSpacing.FeedbackTypeAuthorized = TMPCharacterSpacing;
			PLFeedbackTMPCharacterSpacing.FeedbackTypeAuthorized = TMPCharacterSpacing;
			  
			PLF_TMPColor.FeedbackTypeAuthorized = TMPColor;
			PLFeedbackTMPColor.FeedbackTypeAuthorized = TMPColor;
			  
			PLF_TMPCountTo.FeedbackTypeAuthorized = TMPCountTo;
			  
			PLF_TMPDilate.FeedbackTypeAuthorized = TMPDilate;
			PLFeedbackTMPDilate.FeedbackTypeAuthorized = TMPDilate;
			  
			PLF_TMPFontSize.FeedbackTypeAuthorized = TMPFontSize;
			PLFeedbackTMPFontSize.FeedbackTypeAuthorized = TMPFontSize;
			  
			PLF_TMPLineSpacing.FeedbackTypeAuthorized = TMPLineSpacing;
			PLFeedbackTMPLineSpacing.FeedbackTypeAuthorized = TMPLineSpacing;
			  
			PLF_TMPOutlineColor.FeedbackTypeAuthorized = TMPOutlineColor;
			PLFeedbackTMPOutlineColor.FeedbackTypeAuthorized = TMPOutlineColor;
			  
			PLF_TMPOutlineWidth.FeedbackTypeAuthorized = TMPOutlineWidth;
			PLFeedbackTMPOutlineWidth.FeedbackTypeAuthorized = TMPOutlineWidth;
			  
			PLF_TMPParagraphSpacing.FeedbackTypeAuthorized = TMPParagraphSpacing;
			PLFeedbackTMPParagraphSpacing.FeedbackTypeAuthorized = TMPParagraphSpacing;
			  
			PLF_TMPSoftness.FeedbackTypeAuthorized = TMPSoftness;
			PLFeedbackTMPSoftness.FeedbackTypeAuthorized = TMPSoftness;
		  
			PLF_TMPText.FeedbackTypeAuthorized = TMPText;
			PLFeedbackTMPText.FeedbackTypeAuthorized = TMPText;
			  
			PLF_TMPTextReveal.FeedbackTypeAuthorized = TMPTextReveal;
			PLFeedbackTMPTextReveal.FeedbackTypeAuthorized = TMPTextReveal;
			#endif
		}
	}

}