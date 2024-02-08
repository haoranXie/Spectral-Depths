using UnityEngine;
using UnityEngine.Events;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// Add this component to an object, and it'll let you easily trigger UnityEvents when the event of the specified name is triggered
	/// </summary>
	public class PLGameEventListener : MonoBehaviour, PLEventListener<PLGameEvent>
	{
		[Header("PLGameEvent")] 
		/// the name of the event you want to listen for
		[Tooltip("the name of the event you want to listen for")]
		public string EventName = "Load";
		/// a UnityEvent hook you can use to call methods when the specified event gets triggered
		[Tooltip("a UnityEvent hook you can use to call methods when the specified event gets triggered")]
		public UnityEvent OnMMGameEvent;
		
		/// <summary>
		/// When a PLGameEvent happens, we trigger our UnityEvent if necessary
		/// </summary>
		/// <param name="gameEvent"></param>
		public void OnMMEvent(PLGameEvent gameEvent)
		{
			if (gameEvent.EventName == EventName)
			{
				OnMMGameEvent?.Invoke();
			}
		}

		/// <summary>
		/// On enable, we start listening for PLGameEvents. You may want to extend that to listen to other types of events.
		/// </summary>
		protected virtual void OnEnable()
		{
			this.PLEventStartListening<PLGameEvent>();
		}

		/// <summary>
		/// On disable, we stop listening for PLGameEvents. You may want to extend that to stop listening to other types of events.
		/// </summary>
		protected virtual void OnDisable()
		{
			this.PLEventStopListening<PLGameEvent>();
		}
	}	
}