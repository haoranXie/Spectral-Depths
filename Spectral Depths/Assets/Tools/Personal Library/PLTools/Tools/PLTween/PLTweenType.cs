using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.Tools
{
	public enum PLTweenDefinitionTypes { PLTween, AnimationCurve }

	[Serializable]
	public class PLTweenType
	{
		public PLTweenDefinitionTypes PLTweenDefinitionType = PLTweenDefinitionTypes.PLTween;
		public PLTween.PLTweenCurve PLTweenCurve = PLTween.PLTweenCurve.EaseInCubic;
		public AnimationCurve Curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1f));
		public bool Initialized = false;

		public PLTweenType(PLTween.PLTweenCurve newCurve)
		{
			PLTweenCurve = newCurve;
			PLTweenDefinitionType = PLTweenDefinitionTypes.PLTween;
		}
		public PLTweenType(AnimationCurve newCurve)
		{
			Curve = newCurve;
			PLTweenDefinitionType = PLTweenDefinitionTypes.AnimationCurve;
		}

		public float Evaluate(float t)
		{
			return PLTween.Evaluate(t, this);
		}
	}
}