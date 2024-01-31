using UnityEngine;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// Add this component to an object and it'll persist across scenes 
	/// </summary>
	public class PLDontDestroyOnLoad : MonoBehaviour
	{
		/// <summary>
		/// On Awake we make sure our object will not destroy on the next scene load
		/// </summary>
		protected void Awake()
		{
			DontDestroyOnLoad(this.gameObject);
		}
	}    
}