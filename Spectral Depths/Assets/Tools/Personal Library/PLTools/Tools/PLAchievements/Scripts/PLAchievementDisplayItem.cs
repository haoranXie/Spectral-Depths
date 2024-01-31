using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SpectralDepths.Tools;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// This class is used to display an achievement. Add it to a prefab containing all the required elements listed below.
	/// </summary>
	[AddComponentMenu("Spectral Depths/Tools/Achievements/PLAchievementDisplayItem")]
	public class PLAchievementDisplayItem : MonoBehaviour 
	{		
		public Image BackgroundLocked;
		public Image BackgroundUnlocked;
		public Image Icon;
		public Text Title;
		public Text Description;
		public PLProgressBar ProgressBarDisplay;	
	}
}