using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A class used to receive level values from a PLRadioBroadcaster, and apply it to (almost) any value on any object
	/// </summary>
	[PLRequiresConstantRepaint]
	public class PLRadioReceiver : PLMonoBehaviour
	{
		[Header("Target")]
		/// the receiver to write the level to
		public PLPropertyReceiver Receiver;

		[Header("Channel")]
		/// whether or not this receiver should listen to the channel
		public bool CanListen = true;
		/// the Channel to listen to (has to match the one on the PLRadioBroadcaster you're listening to)
		[PLCondition("CanListen", true)]
		public int Channel = 0;

		[Header("Modifiers")]
		/// whether or not to randomize the received level, this will generate at awake a random level multiplier, to apply to the level
		public bool RandomizeLevel = false;
		/// if random, the min bound of the random multiplier
		[PLCondition("RandomizeLevel", true)]
		public float MinRandomLevelMultiplier = 0f;
		/// if random, the max bound of the random multiplier
		[PLCondition("RandomizeLevel", true)]
		public float MaxRandomLevelMultiplier = 1f;

		protected bool _listeningToEvents = false;
		protected float _randomLevelMultiplier = 1f;
		protected float _lastLevel;

		/// <summary>
		/// On Awake, starts listening and generates a random level multiplier if needed
		/// </summary>
		protected virtual void Awake()
		{
			Receiver.Initialization(this.gameObject);

			if (!_listeningToEvents && CanListen)
			{
				StartListening();
			}

			GenerateRandomLevelMultiplier();
		}

		public virtual void GenerateRandomLevelMultiplier()
		{
			if (RandomizeLevel)
			{
				_randomLevelMultiplier = Random.Range(MinRandomLevelMultiplier, MaxRandomLevelMultiplier);
			}
		}

		/// <summary>
		/// Sets the level on the receiver
		/// </summary>
		/// <param name="newLevel"></param>
		public virtual void SetLevel(float newLevel)
		{
			Receiver.SetLevel(newLevel);
		}

		/// <summary>
		/// When getting a radio level event, we make sure it's the right channel, and apply it if needed
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="level"></param>
		protected virtual void OnRadioLevelEvent(int channel, float level)
		{
			if (channel != Channel)
			{
				return;
			}
			if (RandomizeLevel)
			{
				level *= _randomLevelMultiplier;
			}
			SetLevel(level);
		}

		/// <summary>
		/// Stops listening to events on destroy
		/// </summary>
		protected virtual void OnDestroy()
		{
			_listeningToEvents = false;
			StopListening();
		}

		/// <summary>
		/// Starts listening for events
		/// </summary>
		public virtual void StartListening()
		{
			_listeningToEvents = true;
			PLRadioLevelEvent.Register(OnRadioLevelEvent);
		}

		/// <summary>
		/// Stops listening for events
		/// </summary>
		public virtual void StopListening()
		{
			_listeningToEvents = false;
			PLRadioLevelEvent.Unregister(OnRadioLevelEvent);
		}
	}
}