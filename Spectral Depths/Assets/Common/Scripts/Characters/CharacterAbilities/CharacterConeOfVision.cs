using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using SpectralDepths.InventoryEngine;
using System.Collections.Generic;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// An ability that casts a cone of vision around the character.
	/// </summary>
	[RequireComponent(typeof(PLConeOfVision))]
	[AddComponentMenu("Spectral Depths/Character/Abilities/Character Cone of Vision")]
	public class CharacterConeOfVision : TopDownMonoBehaviour
	{
		protected PLConeOfVision _coneOfVision;
		protected CharacterOrientation3D _characterOrientation;

		/// <summary>
		/// On awake, we grab our components
		/// </summary>
		protected virtual void Awake()
		{
			_characterOrientation = this.gameObject.GetComponentInParent<CharacterOrientation3D>();
			_coneOfVision = this.gameObject.GetComponent<PLConeOfVision>();
		}

		/// <summary>
		/// On update, we update our cone of vision
		/// </summary>
		protected virtual void Update()
		{
			UpdateDirection();   
		}

		/// <summary>
		/// Sends the character orientation's angle to the cone of vision
		/// </summary>
		protected virtual void UpdateDirection()
		{
			if (_characterOrientation == null)
			{
				_coneOfVision.SetDirectionAndAngles(this.transform.forward, this.transform.eulerAngles);              
			}
			else
			{
				_coneOfVision.SetDirectionAndAngles(_characterOrientation.ModelDirection, _characterOrientation.ModelAngles);              
			}
		}
	}
}