using UnityEngine;
using SpectralDepths.Tools;
#if PL_CINEMACHINE
using Cinemachine;
#endif

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// Automatically grabs a Cinemachine camera group and assigns LevelManager's players on load and makes a Cinemachine Virtual Camera follow that target
	/// </summary>
	public class MultiplayerCameraGroupTarget : TopDownMonoBehaviour, PLEventListener<PLGameEvent>, PLEventListener<TopDownEngineEvent>
	{
		#if PL_CINEMACHINE
		[Header("Multiplayer Camera Group Target")]
		/// the virtual camera that will follow the group target
		[Tooltip("the virtual camera that will follow the group target")]
		public CinemachineVirtualCamera TargetCamera;

		protected CinemachineTargetGroup _targetGroup;
		#endif

		/// <summary>
		/// On Awake we grab our target group component
		/// </summary>
		protected virtual void Awake()
		{
			#if PL_CINEMACHINE
			_targetGroup = this.gameObject.GetComponent<CinemachineTargetGroup>();
			#endif
		}

		/// <summary>
		/// On load, we bind the characters to the target group and have the virtual cam follow that target group
		/// </summary>
		/// <param name="gameEvent"></param>
		public virtual void OnMMEvent(PLGameEvent gameEvent)
		{
			#if PL_CINEMACHINE
			if (gameEvent.EventName == "Load")
			{
				if (_targetGroup == null)
				{
					return;
				}

				int i = 0;
				_targetGroup.m_Targets = new CinemachineTargetGroup.Target[LevelManager.Instance.Players.Count];

				foreach (Character character in LevelManager.Instance.Players)
				{
					CinemachineTargetGroup.Target target = new CinemachineTargetGroup.Target();
					target.weight = 1;
					target.radius = 0;
					target.target = character.transform;

					_targetGroup.m_Targets[i] = target;
					i++;
				}

				TargetCamera.Follow = this.transform;
			}
			#endif
		}

		public virtual void OnMMEvent(TopDownEngineEvent tdEvent)
		{
			#if PL_CINEMACHINE
			if (tdEvent.EventType == TopDownEngineEventTypes.PlayerDeath)
			{
				int i = 0;
				foreach (Character character in LevelManager.Instance.Players)
				{
					if (character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
					{
						_targetGroup.m_Targets[i].weight = 0f;
					}
					i++;
				}
			}
			#endif
		}

		/// <summary>
		/// Starts listening for game events
		/// </summary>
		protected virtual void OnEnable()
		{
			this.PLEventStartListening<PLGameEvent>();
			this.PLEventStartListening<TopDownEngineEvent>();
		}

		/// <summary>
		/// Stops listening for game events
		/// </summary>
		protected virtual void OnDisable()
		{
			this.PLEventStopListening<PLGameEvent>();
			this.PLEventStopListening<TopDownEngineEvent>();
		}
	}
}