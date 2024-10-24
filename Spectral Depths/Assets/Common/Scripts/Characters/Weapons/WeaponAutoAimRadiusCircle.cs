using System.Collections;
using System.Collections.Generic;
using SpectralDepths.Tools;
using UnityEngine;

namespace SpectralDepths.TopDown
{
	/// <summary>
	/// This class will automatically draw a circle to match the radius of the auto aim weapon if there's one
	/// </summary>
	[RequireComponent(typeof(LineRenderer))]
	[AddComponentMenu("Spectral Depths/Weapons/Weapon Auto Aim Radius Circle")]
	public class WeaponAutoAimRadiusCircle : PLLineRendererCircle
	{
		[Header("Weapon Radius")]
		public CharacterHandleWeapon TargetHandleWeaponAbility;

		/// <summary>
		/// On initialization, hooks itself to weapon changes
		/// </summary>
		protected override void Initialization()
		{
			base.Initialization();
			_line = gameObject.GetComponent<LineRenderer>();
			_line.enabled = false;
            
			if (TargetHandleWeaponAbility != null)
			{
				TargetHandleWeaponAbility.OnWeaponChanged += OnWeaponChange;
			}
		}
        
		/// <summary>
		/// When the weapon changes, if it has auto aim, draws a circle around it
		/// </summary>
		void OnWeaponChange()
		{
			if (TargetHandleWeaponAbility.CurrentWeapon == null)
			{
				return;
			}
			WeaponAutoAim autoAim = TargetHandleWeaponAbility.CurrentWeapon.GetComponent<WeaponAutoAim>();
			_line.enabled = (autoAim != null);
            
			if (autoAim != null)
			{
				HorizontalRadius = autoAim.ScanRadius;
				VerticalRadius = autoAim.ScanRadius;
			}
			DrawCircle();
		}

		/// <summary>
		/// On disables we unhook from our delegate
		/// </summary>
		void OnDisable()
		{
			if (TargetHandleWeaponAbility != null)
			{
				TargetHandleWeaponAbility.OnWeaponChanged -= OnWeaponChange;
			}
		}
	}
}