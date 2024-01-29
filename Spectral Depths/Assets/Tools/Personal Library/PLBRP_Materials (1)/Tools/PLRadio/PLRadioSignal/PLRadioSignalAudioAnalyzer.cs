using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A class used to expose a beat level from a target PLAudioAnalyzer, to be broadcasted by a PLAudioBroadcaster
	/// </summary>
	public class PLRadioSignalAudioAnalyzer : PLRadioSignal
	{
		[Header("Audio Analyzer")]
		/// the PLAudioAnalyzer to read the value on
		public PLAudioAnalyzer TargetAnalyzer;
		/// the ID of the beat to listen to
		public int BeatID;

		/// <summary>
		/// On Shake, we output our beat value
		/// </summary>
		protected override void Shake()
		{
			base.Shake();
			CurrentLevel = TargetAnalyzer.Beats[BeatID].CurrentValue * GlobalMultiplier;
		}
	}
}