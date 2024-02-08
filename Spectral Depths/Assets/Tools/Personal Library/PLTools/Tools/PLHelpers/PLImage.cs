using UnityEngine;
using System.Collections;

namespace SpectralDepths.Tools
{	
	/// <summary>
	/// Image helpers
	/// </summary>

	public class PLImage : MonoBehaviour 
	{
		/// <summary>
		/// Coroutine used to make the character's sprite flicker (when hurt for example).
		/// </summary>
		public static IEnumerator Flicker(Renderer renderer, Color initialColor, Color flickerColor, float flickerSpeed, float flickerDuration)
		{
			if (renderer==null)
			{
				yield break;
			}

			if (!renderer.material.HasProperty("_Color"))
			{
				yield break;
			}

			if (initialColor == flickerColor)
			{
				yield break;
			}

			float flickerStop = Time.time + flickerDuration;

			while (Time.time<flickerStop)
			{
				renderer.material.color = flickerColor;
				yield return PLCoroutine.WaitFor(flickerSpeed);
				renderer.material.color = initialColor;
				yield return PLCoroutine.WaitFor(flickerSpeed);
			}

			renderer.material.color = initialColor;        
		}
	}
}