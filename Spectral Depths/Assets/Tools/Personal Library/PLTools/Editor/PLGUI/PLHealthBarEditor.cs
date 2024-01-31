using System;
using UnityEngine;
using SpectralDepths.Tools;
using UnityEditor;
using UnityEngine.UI;

namespace SpectralDepths.Tools
{	
	[CanEditMultipleObjects]
	[CustomEditor(typeof(PLHealthBar),true)]
	/// <summary>
	/// Custom editor for health bars (mostly a switch for prefab based / drawn bars
	/// </summary>
	public class HealthBarEditor : Editor 
	{
		public PLHealthBar HealthBarTarget 
		{ 
			get 
			{ 
				return (PLHealthBar)target;
			}
		} 

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			switch (HealthBarTarget.HealthBarType)
			{
				case PLHealthBar.HealthBarTypes.Prefab:
					Editor.DrawPropertiesExcluding(serializedObject, new string[] {"TargetProgressBar", "NestDrawnHealthBar", "Billboard", "FollowTargetMode", "Size","BackgroundPadding", "SortingLayerName", "InitialRotationAngles", "ForegroundColor", "DelayedColor", "BorderColor", "BackgroundColor", "Delay", "LerpFrontBar", "LerpFrontBarSpeed", "LerpDelayedBar", "LerpDelayedBarSpeed", "BumpScaleOnChange", "BumpDuration", "BumpAnimationCurve" });
					break;
				case PLHealthBar.HealthBarTypes.Drawn:
					Editor.DrawPropertiesExcluding(serializedObject, new string[] {"TargetProgressBar", "HealthBarPrefab" });
					break;
				case PLHealthBar.HealthBarTypes.Existing:
					Editor.DrawPropertiesExcluding(serializedObject, new string[] {"HealthBarPrefab", "NestDrawnHealthBar", "Billboard", "FollowTargetMode", "Size","BackgroundPadding", "SortingLayerName", "InitialRotationAngles", "ForegroundColor", "DelayedColor", "BorderColor", "BackgroundColor", "Delay", "LerpFrontBar", "LerpFrontBarSpeed", "LerpDelayedBar", "LerpDelayedBarSpeed", "BumpScaleOnChange", "BumpDuration", "BumpAnimationCurve" });
					break;
			}

			serializedObject.ApplyModifiedProperties();
		}

	}
}