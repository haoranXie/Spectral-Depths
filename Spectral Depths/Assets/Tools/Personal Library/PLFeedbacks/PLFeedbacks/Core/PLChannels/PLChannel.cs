using System;
using UnityEngine;

namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// The possible modes used to identify a channel, either via an int or a PLChannel scriptable object
	/// </summary>
	public enum PLChannelModes
	{
		Int,
		PLChannel
	}
	
	/// <summary>
	/// A data structure used to pass channel information
	/// </summary>
	[Serializable]
	public class PLChannelData
	{
		public PLChannelModes PLChannelMode;
		public int Channel;
		public PLChannel PLChannelDefinition;

		public PLChannelData(PLChannelModes mode, int channel, PLChannel channelDefinition)
		{
			PLChannelMode = mode;
			Channel = channel;
			PLChannelDefinition = channelDefinition;
		}
	}

	/// <summary>
	/// Extensions class for PLChannelData
	/// </summary>
	public static class PLChannelDataExtensions
	{
		public static PLChannelData Set(this PLChannelData data, PLChannelModes mode, int channel, PLChannel channelDefinition)
		{
			data.PLChannelMode = mode;
			data.Channel = channel;
			data.PLChannelDefinition = channelDefinition;
			return data;
		}
	}
	
	/// <summary>
	/// A scriptable object you can create assets from, to identify Channels, used mostly (but not only) in feedbacks and shakers,
	/// to determine a channel of communication, usually between emitters and receivers
	/// </summary>
	[CreateAssetMenu(menuName = "SpectralDepths/PLChannel", fileName = "PLChannel")]
	public class PLChannel : ScriptableObject
	{
		public static bool Match(PLChannelData dataA, PLChannelData dataB)
		{
			if (dataA.PLChannelMode != dataB.PLChannelMode)
			{
				return false;
			}

			if (dataA.PLChannelMode == PLChannelModes.Int)
			{
				return dataA.Channel == dataB.Channel;
			}
			else
			{
				return dataA.PLChannelDefinition == dataB.PLChannelDefinition;
			}
		}
		public static bool Match(PLChannelData dataA, PLChannelModes modeB, int channelB, PLChannel channelDefinitionB)
		{
			if (dataA == null)
			{
				return true;
			}
			
			if (dataA.PLChannelMode != modeB)
			{
				return false;
			}

			if (dataA.PLChannelMode == PLChannelModes.Int)
			{
				return dataA.Channel == channelB;
			}
			else
			{
				return dataA.PLChannelDefinition == channelDefinitionB;
			}
		}
	}
}