using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using System.Collections.Generic;
using UnityEditor;

namespace SpectralDepths.Tools
{
	[CustomEditor(typeof(PLConeOfVision2D), true)]
	public class PLConeOfVision2DInspector : Editor
	{
		protected PLConeOfVision2D _coneOfVision;

		protected virtual void OnSceneGUI()
		{
			// draws a circle around the character to represent the cone of vision's radius
			_coneOfVision = (PLConeOfVision2D)target;

			Handles.color = Color.yellow;
			Handles.DrawWireArc(_coneOfVision.transform.position, -Vector3.forward, Vector3.up, 360f, _coneOfVision.VisionRadius);

			// draws two lines to mark the vision angle
			Vector3 visionAngleLeft = PLMaths.DirectionFromAngle2D(-_coneOfVision.VisionAngle / 2f, _coneOfVision.EulerAngles.y);
			Vector3 visionAngleRight = PLMaths.DirectionFromAngle2D(_coneOfVision.VisionAngle / 2f, _coneOfVision.EulerAngles.y);

			Handles.DrawLine(_coneOfVision.transform.position, _coneOfVision.transform.position + visionAngleLeft * _coneOfVision.VisionRadius);
			Handles.DrawLine(_coneOfVision.transform.position, _coneOfVision.transform.position + visionAngleRight * _coneOfVision.VisionRadius);

			foreach (Transform visibleTarget in _coneOfVision.VisibleTargets)
			{
				Handles.color = PLColors.Orange;
				Handles.DrawLine(_coneOfVision.transform.position, visibleTarget.position);
			}
		}
	}
}