using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SpectralDepths.Tools;
using UnityEngine.EventSystems;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// Add this helper to an object and focus will be set to it on Enable
	/// </summary>
	[AddComponentMenu("Spectral Depths/Tools/GUI/PLGetFocusOnEnable")]
	public class PLGetFocusOnEnable : MonoBehaviour
	{
		protected virtual void OnEnable()
		{
			EventSystem.current.SetSelectedGameObject(this.gameObject, null);
		}
	}
}