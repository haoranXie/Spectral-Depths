using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SpectralDepths.Tools;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A class used to open a URL specified in its inspector
	/// </summary>
	[AddComponentMenu("Spectral Depths/Tools/Utilities/PLOpenURL")]
	public class PLOpenURL : MonoBehaviour 
	{
		/// the URL to open when calling OpenURL()
		public string DestinationURL;

		/// <summary>
		/// Opens the URL specified in the DestinationURL field
		/// </summary>
		public virtual void OpenURL()
		{
			Application.OpenURL(DestinationURL);
		}		
	}
}