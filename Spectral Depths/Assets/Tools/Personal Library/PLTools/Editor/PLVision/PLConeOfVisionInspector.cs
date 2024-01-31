using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using System.Collections.Generic;
using UnityEditor;

namespace SpectralDepths.Tools
{
	[CustomEditor(typeof(PLConeOfVision), true)]
	public class PLConeOfVisionInspector : Editor
	{
		protected PLConeOfVision _coneOfVision;

		protected virtual void OnSceneGUI()
		{
			// draws a circle around the character to represent the cone of vision's radius
			_coneOfVision = (PLConeOfVision)target;

			Handles.color = Color.yellow;
			Handles.DrawWireArc(_coneOfVision.Center, Vector3.up, Vector3.forward, 360f, _coneOfVision.VisionRadius);

			// draws two lines to mark the vision angle
			Vector3 visionAngleLeft = PLMaths.DirectionFromAngle(-_coneOfVision.VisionAngle / 2f, _coneOfVision.EulerAngles.y);
			Vector3 visionAngleRight = PLMaths.DirectionFromAngle(_coneOfVision.VisionAngle / 2f, _coneOfVision.EulerAngles.y);

			Handles.DrawLine(_coneOfVision.Center, _coneOfVision.Center + visionAngleLeft * _coneOfVision.VisionRadius);
			Handles.DrawLine(_coneOfVision.Center, _coneOfVision.Center + visionAngleRight * _coneOfVision.VisionRadius);

			foreach (Transform visibleTarget in _coneOfVision.VisibleTargets)
			{
				Handles.color = PLColors.Orange;
				Handles.DrawLine(_coneOfVision.Center, visibleTarget.position);
			}
		}
	}
}