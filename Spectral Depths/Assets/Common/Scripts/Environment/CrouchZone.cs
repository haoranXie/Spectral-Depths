using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Tools;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// Add this zone to a trigger collider and it'll automatically trigger a crouch on your 3D character on entry
	/// </summary>
	[RequireComponent(typeof(Collider))]
	[AddComponentMenu("Spectral Depths/Environment/Crouch Zone")]
	public class CrouchZone : TopDownMonoBehaviour
	{
		protected CharacterCrouch _characterCrouch;

		/// <summary>
		/// On start we make sure our collider is set to trigger
		/// </summary>
		protected virtual void Start()
		{
			this.gameObject.PLGetComponentNoAlloc<Collider>().isTrigger = true;
		}

		/// <summary>
		/// On enter we force crouch if we can
		/// </summary>
		/// <param name="collider"></param>
		protected virtual void OnTriggerEnter(Collider collider)
		{
			_characterCrouch = collider.gameObject.PLGetComponentNoAlloc<Character>()?.FindAbility<CharacterCrouch>();
			if (_characterCrouch != null)
			{
				_characterCrouch.StartForcedCrouch();
			}
		}

		/// <summary>
		/// On exit we stop force crouching
		/// </summary>
		/// <param name="collider"></param>
		protected virtual void OnTriggerExit(Collider collider)
		{
			_characterCrouch = collider.gameObject.PLGetComponentNoAlloc<Character>()?.FindAbility<CharacterCrouch>();
			if (_characterCrouch != null)
			{
				_characterCrouch.StopForcedCrouch();
			}
		}
	}
}