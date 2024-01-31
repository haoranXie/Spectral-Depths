using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A simple class used to control properties on a transform
	/// </summary>
	[AddComponentMenu("Spectral Depths/Tools/Property Controllers/TransformController")]
	public class TransformController : MonoBehaviour
	{
		[Header("Position")]
		public bool ControlPositionX;
		[PLCondition("ControlPositionX", true)]
		public float PositionX;
		public bool ControlPositionY;
		[PLCondition("ControlPositionY", true)]
		public float PositionY;
		public bool ControlPositionZ;
		[PLCondition("ControlPositionZ", true)]
		public float PositionZ;

		[Header("Local Position")]
		public bool ControlLocalPositionX;
		[PLCondition("ControlLocalPositionX", true)]
		public float LocalPositionX;
		public bool ControlLocalPositionY;
		[PLCondition("ControlLocalPositionY", true)]
		public float LocalPositionY;
		public bool ControlLocalPositionZ;
		[PLCondition("ControlLocalPositionZ", true)]
		public float LocalPositionZ;

		[Header("Rotation")]
		public bool ControlRotationX;
		[PLCondition("ControlRotationX", true)]
		public float RotationX;
		public bool ControlRotationY;
		[PLCondition("ControlRotationY", true)]
		public float RotationY;
		public bool ControlRotationZ;
		[PLCondition("ControlRotationZ", true)]
		public float RotationZ;

		[Header("Local Rotation")]
		public bool ControlLocalRotationX;
		[PLCondition("ControlLocalRotationX", true)]
		public float LocalRotationX;
		public bool ControlLocalRotationY;
		[PLCondition("ControlLocalRotationY", true)]
		public float LocalRotationY;
		public bool ControlLocalRotationZ;
		[PLCondition("ControlLocalRotationZ", true)]
		public float LocalRotationZ;

		[Header("Scale")]
		public bool ControlScaleX;
		[PLCondition("ControlScaleX", true)]
		public float ScaleX;
		public bool ControlScaleY;
		[PLCondition("ControlScaleY", true)]
		public float ScaleY;
		public bool ControlScaleZ;
		[PLCondition("ControlScaleZ", true)]
		public float ScaleZ;

		protected Vector3 _position;
		protected Vector3 _localPosition;
		protected Vector3 _rotation;
		protected Vector3 _localRotation;
		protected Vector3 _scale;

		/// <summary>
		/// At update, modifies the requested properties
		/// </summary>
		protected virtual void Update()
		{
			_position = this.transform.position;
			_localPosition = this.transform.localPosition;
			_rotation = this.transform.eulerAngles;
			_localRotation = this.transform.localEulerAngles;
			_scale = this.transform.localScale;

			if (ControlPositionX) { _position.x = PositionX; this.transform.position = _position; }
			if (ControlPositionY) { _position.y = PositionY; this.transform.position = _position; }
			if (ControlPositionZ) { _position.z = PositionZ; this.transform.position = _position; }

			if (ControlLocalPositionX) { _localPosition.x = LocalPositionX; this.transform.localPosition = _localPosition; }
			if (ControlLocalPositionY) { _localPosition.y = LocalPositionY; this.transform.localPosition = _localPosition; }
			if (ControlLocalPositionZ) { _localPosition.z = LocalPositionZ; this.transform.localPosition = _localPosition; }

			if (ControlRotationX) { _rotation.x = RotationX; this.transform.eulerAngles = _rotation; }
			if (ControlRotationY) { _rotation.y = RotationY; this.transform.eulerAngles = _rotation; }
			if (ControlRotationZ) { _rotation.z = RotationZ; this.transform.eulerAngles = _rotation; }

			if (ControlLocalRotationX) { _localRotation.x = LocalRotationX; this.transform.localEulerAngles = _localRotation; }
			if (ControlLocalRotationY) { _localRotation.y = LocalRotationY; this.transform.localEulerAngles = _localRotation; }
			if (ControlLocalRotationZ) { _localRotation.z = LocalRotationZ; this.transform.localEulerAngles = _localRotation; }

			if (ControlScaleX) { _scale.x = ScaleX; this.transform.localScale = _scale; }
			if (ControlScaleY) { _scale.y = ScaleY; this.transform.localScale = _scale; }
			if (ControlScaleZ) { _scale.z = ScaleZ; this.transform.localScale = _scale; }
		}
	}
}