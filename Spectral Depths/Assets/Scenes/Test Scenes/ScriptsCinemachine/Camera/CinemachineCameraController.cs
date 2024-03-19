using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
#if PL_CINEMACHINE
using Cinemachine;
#endif

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// A class that handles camera follow for Cinemachine powered cameras
	/// </summary>
	public class CinemachineCameraController : TopDownMonoBehaviour, PLEventListener<PLCameraEvent>, PLEventListener<TopDownEngineEvent>
	{
		/// True if the camera should follow the player
		public bool FollowsPlayer { get; set; }
		/// Whether or not this camera should follow a player
		[Tooltip("Whether or not this camera should follow a player")]
		public bool FollowsAPlayer = true;
		/// Whether to confine this camera to the level bounds, as defined in the LevelManager
		[Tooltip("Whether to confine this camera to the level bounds, as defined in the LevelManager")]
		public bool ConfineCameraToLevelBounds = true;
		/// If this is true, this confiner will listen to set confiner events
		[Tooltip("If this is true, this confiner will listen to set confiner events")]
		public bool ListenToSetConfinerEvents = true;
		[PLReadOnly]
		/// the target character this camera should follow
		[Tooltip("the target character this camera should follow")]
		public Character TargetCharacter;

		#if PL_CINEMACHINE
		protected CinemachineVirtualCamera _virtualCamera;
		protected CinemachineConfiner _confiner;
		#endif

		/// <summary>
		/// On Awake we grab our components
		/// </summary>
		protected virtual void Awake()
		{
			#if PL_CINEMACHINE
			_virtualCamera = GetComponent<CinemachineVirtualCamera>();
			_confiner = GetComponent<CinemachineConfiner>();
			#endif
		}

		/// <summary>
		/// On Start we assign our bounding volume
		/// </summary>
		protected virtual void Start()
		{
			#if PL_CINEMACHINE
			if ((_confiner != null) && ConfineCameraToLevelBounds && LevelManager.HasInstance)
			{
				_confiner.m_BoundingVolume = LevelManager.Instance.BoundsCollider;
			}
			#endif
		}

		public virtual void SetTarget(Character character)
		{
			TargetCharacter = character;
		}

		/// <summary>
		/// Starts following the LevelManager's main player
		/// </summary>
		public virtual void StartFollowing()
		{
			if (!FollowsAPlayer) { return; }
			FollowsPlayer = true;
			#if PL_CINEMACHINE
			_virtualCamera.Follow = TargetCharacter.CameraTarget.transform;
			_virtualCamera.enabled = true;
			#endif
		}

		/// <summary>
		/// Stops following any target
		/// </summary>
		public virtual void StopFollowing()
		{
			if (!FollowsAPlayer) { return; }
			FollowsPlayer = false;
			#if PL_CINEMACHINE
			_virtualCamera.Follow = null;
			_virtualCamera.enabled = false;
			#endif
		}

		public virtual void OnMMEvent(PLCameraEvent cameraEvent)
		{
			#if PL_CINEMACHINE
			switch (cameraEvent.EventType)
			{
				case PLCameraEventTypes.SetTargetCharacter:
					SetTarget(cameraEvent.TargetCharacter);
					break;

				case PLCameraEventTypes.SetConfiner:                    
					if (_confiner != null && ListenToSetConfinerEvents)
					{
						_confiner.m_BoundingVolume = cameraEvent.Bounds;
					}
					break;

				case PLCameraEventTypes.StartFollowing:
					if (cameraEvent.TargetCharacter != null)
					{
						if (cameraEvent.TargetCharacter != TargetCharacter)
						{
							return;
						}
					}
					StartFollowing();
					break;

				case PLCameraEventTypes.StopFollowing:
					if (cameraEvent.TargetCharacter != null)
					{
						if (cameraEvent.TargetCharacter != TargetCharacter)
						{
							return;
						}
					}
					StopFollowing();
					break;

				case PLCameraEventTypes.RefreshPosition:
					StartCoroutine(RefreshPosition());
					break;

				case PLCameraEventTypes.ResetPriorities:
					_virtualCamera.Priority = 0;
					break;
			}
			#endif
		}

		protected virtual IEnumerator RefreshPosition()
		{
			#if PL_CINEMACHINE
			_virtualCamera.enabled = false;
			#endif
			yield return null;
			StartFollowing();
		}

		public virtual void OnMMEvent(TopDownEngineEvent topdownEngineEvent)
		{
			if (topdownEngineEvent.EventType == TopDownEngineEventTypes.CharacterSwitch)
			{
				SetTarget(LevelManager.Instance.Players[0]);
				StartFollowing();
			}

			if (topdownEngineEvent.EventType == TopDownEngineEventTypes.CharacterSwap)
			{
				SetTarget(LevelManager.Instance.Players[0]);
				PLCameraEvent.Trigger(PLCameraEventTypes.RefreshPosition);
			}
		}

		protected virtual void OnEnable()
		{
			this.PLEventStartListening<PLCameraEvent>();
			this.PLEventStartListening<TopDownEngineEvent>();
		}

		protected virtual void OnDisable()
		{
			this.PLEventStopListening<PLCameraEvent>();
			this.PLEventStopListening<TopDownEngineEvent>();
		}
	}
}