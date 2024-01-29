using UnityEngine;
using System.Collections;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// Adds this class to particles to force their sorting layer
	/// </summary>
	[AddComponentMenu("Spectral Depths/Tools/Particles/PLVisibleParticle")]
	public class PLVisibleParticle : MonoBehaviour {

		/// <summary>
		/// Sets the particle system's renderer to the Visible Particles sorting layer
		/// </summary>
		protected virtual void Start () 
		{
			GetComponent<ParticleSystem>().GetComponent<Renderer>().sortingLayerName = "VisibleParticles";
		}		
	}
}