using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpectralDepths.Tools
{	
	/// <summary>
	/// A very simple class, meant to be used within a PLSceneLoading screen, to update the fill amount of an Image
	/// based on loading progress
	/// </summary>
	public class PLSceneLoadingImageProgress : MonoBehaviour
	{
		protected Image _image;

		/// <summary>
		/// On Awake we store our Image
		/// </summary>
		protected virtual void Awake()
		{
			_image = this.gameObject.GetComponent<Image>();
		}
        
		/// <summary>
		/// Meant to be called by the PLSceneLoadingManager, turns the progress of a load into fill amount
		/// </summary>
		/// <param name="newValue"></param>
		public virtual void SetProgress(float newValue)
		{
			_image.fillAmount = newValue;
		}
	}
}