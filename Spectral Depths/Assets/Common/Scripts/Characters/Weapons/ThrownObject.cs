using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;

namespace SpectralDepths.TopDown
{	
	/// <summary>
	/// A thrown object type of projectile, useful for grenades and such
	/// </summary>
	[RequireComponent(typeof(Rigidbody2D))]
	[AddComponentMenu("Spectral Depths/Weapons/ThrownObject")]
	public class ThrownObject : Projectile 
	{
		protected Vector2 _throwingForce;
		protected bool _forceApplied = false;

		/// <summary>
		/// On init, we grab our rigidbody
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			_rigidBody2D = this.GetComponent<Rigidbody2D>();
		}

		/// <summary>
		/// On enable, we reset the object's speed
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			_forceApplied = false;
		}

		/// <summary>
		/// Handles the projectile's movement, every frame
		/// </summary>
		public override void Movement()
		{
			if (!_forceApplied && (Direction != Vector3.zero))
			{
				_throwingForce = Direction * Speed;
				_rigidBody2D.AddForce (_throwingForce);
				_forceApplied = true;
			}
		}
	}
}