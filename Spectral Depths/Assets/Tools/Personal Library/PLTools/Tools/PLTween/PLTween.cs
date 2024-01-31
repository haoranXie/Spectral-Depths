using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// The formulas described here are (loosely) based on Robert Penner's easing equations http://robertpenner.com/easing/
	/// </summary>

	public class PLTween : MonoBehaviour
	{
		/// <summary>
		/// A list of all the possible curves you can tween a value along
		/// </summary>
		public enum PLTweenCurve
		{
			LinearTween,        
			EaseInQuadratic,    EaseOutQuadratic,   EaseInOutQuadratic,
			EaseInCubic,        EaseOutCubic,       EaseInOutCubic,
			EaseInQuartic,      EaseOutQuartic,     EaseInOutQuartic,
			EaseInQuintic,      EaseOutQuintic,     EaseInOutQuintic,
			EaseInSinusoidal,   EaseOutSinusoidal,  EaseInOutSinusoidal,
			EaseInBounce,       EaseOutBounce,      EaseInOutBounce,
			EaseInOverhead,     EaseOutOverhead,    EaseInOutOverhead,
			EaseInExponential,  EaseOutExponential, EaseInOutExponential,
			EaseInElastic,      EaseOutElastic,     EaseInOutElastic,
			EaseInCircular,     EaseOutCircular,    EaseInOutCircular,
			AntiLinearTween,    AlmostIdentity
		}
		
		public static TweenDelegate[] TweenDelegateArray = new TweenDelegate[]
		{
			LinearTween,        
			EaseInQuadratic,    EaseOutQuadratic,   EaseInOutQuadratic,
			EaseInCubic,        EaseOutCubic,       EaseInOutCubic,
			EaseInQuartic,      EaseOutQuartic,     EaseInOutQuartic,
			EaseInQuintic,      EaseOutQuintic,     EaseInOutQuintic,
			EaseInSinusoidal,   EaseOutSinusoidal,  EaseInOutSinusoidal,
			EaseInBounce,       EaseOutBounce,      EaseInOutBounce,
			EaseInOverhead,     EaseOutOverhead,    EaseInOutOverhead,
			EaseInExponential,  EaseOutExponential, EaseInOutExponential,
			EaseInElastic,      EaseOutElastic,     EaseInOutElastic,
			EaseInCircular,     EaseOutCircular,    EaseInOutCircular,
			AntiLinearTween,    AlmostIdentity
		};

		// Core methods ---------------------------------------------------------------------------------------------------------------

		/// <summary>
		/// Moves a value between a startValue and an endValue based on a currentTime, along the specified tween curve
		/// </summary>
		/// <param name="currentTime"></param>
		/// <param name="initialTime"></param>
		/// <param name="endTime"></param>
		/// <param name="startValue"></param>
		/// <param name="endValue"></param>
		/// <param name="curve"></param>
		/// <returns></returns>
		public static float Tween(float currentTime, float initialTime, float endTime, float startValue, float endValue, PLTweenCurve curve)
		{
			currentTime = PLMaths.Remap(currentTime, initialTime, endTime, 0f, 1f);
			currentTime = TweenDelegateArray[(int)curve](currentTime);
			return startValue + currentTime * (endValue - startValue);
		}

		public static float Evaluate(float t, PLTweenCurve curve)
		{
			return TweenDelegateArray[(int)curve](t);
		}

		public static float Evaluate(float t, PLTweenType tweenType)
		{
			if (tweenType.PLTweenDefinitionType == PLTweenDefinitionTypes.PLTween)
			{
				return Evaluate(t, tweenType.PLTweenCurve);
			}
			if (tweenType.PLTweenDefinitionType == PLTweenDefinitionTypes.AnimationCurve)
			{
				return tweenType.Curve.Evaluate(t);
			}
			return 0f;
		}

		public delegate float TweenDelegate(float currentTime);
		
		public static float LinearTween(float currentTime) { return PLTweenDefinitions.Linear_Tween(currentTime); }
		public static float AntiLinearTween(float currentTime) { return PLTweenDefinitions.LinearAnti_Tween(currentTime); }
		public static float EaseInQuadratic(float currentTime) { return PLTweenDefinitions.EaseIn_Quadratic(currentTime); }
		public static float EaseOutQuadratic(float currentTime) { return PLTweenDefinitions.EaseOut_Quadratic(currentTime); }
		public static float EaseInOutQuadratic(float currentTime) { return PLTweenDefinitions.EaseInOut_Quadratic(currentTime); }
		public static float EaseInCubic(float currentTime) { return PLTweenDefinitions.EaseIn_Cubic(currentTime); }
		public static float EaseOutCubic(float currentTime) { return PLTweenDefinitions.EaseOut_Cubic(currentTime); }
		public static float EaseInOutCubic(float currentTime) { return PLTweenDefinitions.EaseInOut_Cubic(currentTime); }
		public static float EaseInQuartic(float currentTime) { return PLTweenDefinitions.EaseIn_Quartic(currentTime); }
		public static float EaseOutQuartic(float currentTime) { return PLTweenDefinitions.EaseOut_Quartic(currentTime); }
		public static float EaseInOutQuartic(float currentTime) { return PLTweenDefinitions.EaseInOut_Quartic(currentTime); }
		public static float EaseInQuintic(float currentTime) { return PLTweenDefinitions.EaseIn_Quintic(currentTime); }
		public static float EaseOutQuintic(float currentTime) { return PLTweenDefinitions.EaseOut_Quintic(currentTime); }
		public static float EaseInOutQuintic(float currentTime) { return PLTweenDefinitions.EaseInOut_Quintic(currentTime); }
		public static float EaseInSinusoidal(float currentTime) { return PLTweenDefinitions.EaseIn_Sinusoidal(currentTime); }
		public static float EaseOutSinusoidal(float currentTime) { return PLTweenDefinitions.EaseOut_Sinusoidal(currentTime); }
		public static float EaseInOutSinusoidal(float currentTime) { return PLTweenDefinitions.EaseInOut_Sinusoidal(currentTime); }
		public static float EaseInBounce(float currentTime) { return PLTweenDefinitions.EaseIn_Bounce(currentTime); }
		public static float EaseOutBounce(float currentTime) { return PLTweenDefinitions.EaseOut_Bounce(currentTime); }
		public static float EaseInOutBounce(float currentTime) { return PLTweenDefinitions.EaseInOut_Bounce(currentTime); }
		public static float EaseInOverhead(float currentTime) { return PLTweenDefinitions.EaseIn_Overhead(currentTime); }
		public static float EaseOutOverhead(float currentTime) { return PLTweenDefinitions.EaseOut_Overhead(currentTime); }
		public static float EaseInOutOverhead(float currentTime) { return PLTweenDefinitions.EaseInOut_Overhead(currentTime); }
		public static float EaseInExponential(float currentTime) { return PLTweenDefinitions.EaseIn_Exponential(currentTime); }
		public static float EaseOutExponential(float currentTime) { return PLTweenDefinitions.EaseOut_Exponential(currentTime); }
		public static float EaseInOutExponential(float currentTime) { return PLTweenDefinitions.EaseInOut_Exponential(currentTime); }
		public static float EaseInElastic(float currentTime) { return PLTweenDefinitions.EaseIn_Elastic(currentTime); }
		public static float EaseOutElastic(float currentTime) { return PLTweenDefinitions.EaseOut_Elastic(currentTime); }
		public static float EaseInOutElastic(float currentTime) { return PLTweenDefinitions.EaseInOut_Elastic(currentTime); }
		public static float EaseInCircular(float currentTime) { return PLTweenDefinitions.EaseIn_Circular(currentTime); }
		public static float EaseOutCircular(float currentTime) { return PLTweenDefinitions.EaseOut_Circular(currentTime); }
		public static float EaseInOutCircular(float currentTime) { return PLTweenDefinitions.EaseInOut_Circular(currentTime); }
		public static float AlmostIdentity(float currentTime) { return PLTweenDefinitions.AlmostIdentity(currentTime); }

		/// <summary>
		/// To use :
		/// public PLTween.PLTweenCurve Tween = PLTween.PLTweenCurve.EaseInOutCubic;
		/// private PLTween.TweenDelegate _tween;
		///
		/// _tween = PLTween.GetTweenMethod(Tween);
		/// float t = _tween(someFloat);
		/// </summary>
		/// <param name="tween"></param>
		/// <returns></returns>
		public static TweenDelegate GetTweenMethod(PLTweenCurve tween)
		{
			switch (tween)
			{
				case PLTweenCurve.LinearTween: return LinearTween;
				case PLTweenCurve.AntiLinearTween: return AntiLinearTween;
				case PLTweenCurve.EaseInQuadratic: return EaseInQuadratic;
				case PLTweenCurve.EaseOutQuadratic: return EaseOutQuadratic;
				case PLTweenCurve.EaseInOutQuadratic: return EaseInOutQuadratic;
				case PLTweenCurve.EaseInCubic: return EaseInCubic;
				case PLTweenCurve.EaseOutCubic: return EaseOutCubic;
				case PLTweenCurve.EaseInOutCubic: return EaseInOutCubic;
				case PLTweenCurve.EaseInQuartic: return EaseInQuartic;
				case PLTweenCurve.EaseOutQuartic: return EaseOutQuartic;
				case PLTweenCurve.EaseInOutQuartic: return EaseInOutQuartic;
				case PLTweenCurve.EaseInQuintic: return EaseInQuintic;
				case PLTweenCurve.EaseOutQuintic: return EaseOutQuintic;
				case PLTweenCurve.EaseInOutQuintic: return EaseInOutQuintic;
				case PLTweenCurve.EaseInSinusoidal: return EaseInSinusoidal;
				case PLTweenCurve.EaseOutSinusoidal: return EaseOutSinusoidal;
				case PLTweenCurve.EaseInOutSinusoidal: return EaseInOutSinusoidal;
				case PLTweenCurve.EaseInBounce: return EaseInBounce;
				case PLTweenCurve.EaseOutBounce: return EaseOutBounce;
				case PLTweenCurve.EaseInOutBounce: return EaseInOutBounce;
				case PLTweenCurve.EaseInOverhead: return EaseInOverhead;
				case PLTweenCurve.EaseOutOverhead: return EaseOutOverhead;
				case PLTweenCurve.EaseInOutOverhead: return EaseInOutOverhead;
				case PLTweenCurve.EaseInExponential: return EaseInExponential;
				case PLTweenCurve.EaseOutExponential: return EaseOutExponential;
				case PLTweenCurve.EaseInOutExponential: return EaseInOutExponential;
				case PLTweenCurve.EaseInElastic: return EaseInElastic;
				case PLTweenCurve.EaseOutElastic: return EaseOutElastic;
				case PLTweenCurve.EaseInOutElastic: return EaseInOutElastic;
				case PLTweenCurve.EaseInCircular: return EaseInCircular;
				case PLTweenCurve.EaseOutCircular: return EaseOutCircular;
				case PLTweenCurve.EaseInOutCircular: return EaseInOutCircular;
				case PLTweenCurve.AlmostIdentity: return AlmostIdentity;
			}
			return LinearTween;
		}

		public static Vector2 Tween(float currentTime, float initialTime, float endTime, Vector2 startValue, Vector2 endValue, PLTweenCurve curve)
		{
			startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
			startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
			return startValue;
		}

		public static Vector3 Tween(float currentTime, float initialTime, float endTime, Vector3 startValue, Vector3 endValue, PLTweenCurve curve)
		{
			startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
			startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
			startValue.z = Tween(currentTime, initialTime, endTime, startValue.z, endValue.z, curve);
			return startValue;
		}

		public static Quaternion Tween(float currentTime, float initialTime, float endTime, Quaternion startValue, Quaternion endValue, PLTweenCurve curve)
		{
			float turningRate = Tween(currentTime, initialTime, endTime, 0f, 1f, curve);
			startValue = Quaternion.Slerp(startValue, endValue, turningRate);
			return startValue;
		}

		// Animation curve methods --------------------------------------------------------------------------------------------------------------

		public static float Tween(float currentTime, float initialTime, float endTime, float startValue, float endValue, AnimationCurve curve)
		{
			currentTime = PLMaths.Remap(currentTime, initialTime, endTime, 0f, 1f);
			currentTime = curve.Evaluate(currentTime);
			return startValue + currentTime * (endValue - startValue);
		}

		public static Vector2 Tween(float currentTime, float initialTime, float endTime, Vector2 startValue, Vector2 endValue, AnimationCurve curve)
		{
			startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
			startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
			return startValue;
		}

		public static Vector3 Tween(float currentTime, float initialTime, float endTime, Vector3 startValue, Vector3 endValue, AnimationCurve curve)
		{
			startValue.x = Tween(currentTime, initialTime, endTime, startValue.x, endValue.x, curve);
			startValue.y = Tween(currentTime, initialTime, endTime, startValue.y, endValue.y, curve);
			startValue.z = Tween(currentTime, initialTime, endTime, startValue.z, endValue.z, curve);
			return startValue;
		}

		public static Quaternion Tween(float currentTime, float initialTime, float endTime, Quaternion startValue, Quaternion endValue, AnimationCurve curve)
		{
			float turningRate = Tween(currentTime, initialTime, endTime, 0f, 1f, curve);
			startValue = Quaternion.Slerp(startValue, endValue, turningRate);
			return startValue;
		}

		// Tween type methods ------------------------------------------------------------------------------------------------------------------------

		public static float Tween(float currentTime, float initialTime, float endTime, float startValue, float endValue, PLTweenType tweenType)
		{
			if (tweenType.PLTweenDefinitionType == PLTweenDefinitionTypes.PLTween)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.PLTweenCurve);
			}
			if (tweenType.PLTweenDefinitionType == PLTweenDefinitionTypes.AnimationCurve)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
			}
			return 0f;
		}
		public static Vector2 Tween(float currentTime, float initialTime, float endTime, Vector2 startValue, Vector2 endValue, PLTweenType tweenType)
		{
			if (tweenType.PLTweenDefinitionType == PLTweenDefinitionTypes.PLTween)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.PLTweenCurve);
			}
			if (tweenType.PLTweenDefinitionType == PLTweenDefinitionTypes.AnimationCurve)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
			}
			return Vector2.zero;
		}
		public static Vector3 Tween(float currentTime, float initialTime, float endTime, Vector3 startValue, Vector3 endValue, PLTweenType tweenType)
		{
			if (tweenType.PLTweenDefinitionType == PLTweenDefinitionTypes.PLTween)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.PLTweenCurve);
			}
			if (tweenType.PLTweenDefinitionType == PLTweenDefinitionTypes.AnimationCurve)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
			}
			return Vector3.zero;
		}
		public static Quaternion Tween(float currentTime, float initialTime, float endTime, Quaternion startValue, Quaternion endValue, PLTweenType tweenType)
		{
			if (tweenType.PLTweenDefinitionType == PLTweenDefinitionTypes.PLTween)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.PLTweenCurve);
			}
			if (tweenType.PLTweenDefinitionType == PLTweenDefinitionTypes.AnimationCurve)
			{
				return Tween(currentTime, initialTime, endTime, startValue, endValue, tweenType.Curve);
			}
			return Quaternion.identity;
		}

		// MOVE METHODS ---------------------------------------------------------------------------------------------------------
		public static Coroutine MoveTransform(MonoBehaviour mono, Transform targetTransform, Vector3 origin, Vector3 destination, 
			WaitForSeconds delay, float delayDuration, float duration, PLTween.PLTweenCurve curve, bool ignoreTimescale = false)
		{
			return mono.StartCoroutine(MoveTransformCo(targetTransform, origin, destination, delay, delayDuration, duration, curve, ignoreTimescale));
		}

		public static Coroutine MoveRectTransform(MonoBehaviour mono, RectTransform targetTransform, Vector3 origin, Vector3 destination,
			WaitForSeconds delay, float delayDuration, float duration, PLTween.PLTweenCurve curve, bool ignoreTimescale = false)
		{
			return mono.StartCoroutine(MoveRectTransformCo(targetTransform, origin, destination, delay, delayDuration, duration, curve, ignoreTimescale));
		}

		public static Coroutine MoveTransform(MonoBehaviour mono, Transform targetTransform, Transform origin, Transform destination, WaitForSeconds delay, float delayDuration, float duration, 
			PLTween.PLTweenCurve curve, bool updatePosition = true, bool updateRotation = true, bool ignoreTimescale = false)
		{
			return mono.StartCoroutine(MoveTransformCo(targetTransform, origin, destination, delay, delayDuration, duration, curve, updatePosition, updateRotation, ignoreTimescale));
		}

		public static Coroutine RotateTransformAround(MonoBehaviour mono, Transform targetTransform, Transform center, Transform destination, float angle, WaitForSeconds delay, float delayDuration, 
			float duration, PLTween.PLTweenCurve curve, bool ignoreTimescale = false)
		{
			return mono.StartCoroutine(RotateTransformAroundCo(targetTransform, center, destination, angle, delay, delayDuration, duration, curve, ignoreTimescale));
		}

		protected static IEnumerator MoveRectTransformCo(RectTransform targetTransform, Vector3 origin, Vector3 destination, WaitForSeconds delay,
			float delayDuration, float duration, PLTween.PLTweenCurve curve, bool ignoreTimescale = false)
		{
			if (delayDuration > 0f)
			{
				yield return delay;
			}
			float timeLeft = duration;
			while (timeLeft > 0f)
			{
				targetTransform.localPosition = PLTween.Tween(duration - timeLeft, 0f, duration, origin, destination, curve);
				timeLeft -= ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
				yield return null;
			}
			targetTransform.localPosition = destination;
		}

		protected static IEnumerator MoveTransformCo(Transform targetTransform, Vector3 origin, Vector3 destination, WaitForSeconds delay, 
			float delayDuration, float duration, PLTween.PLTweenCurve curve, bool ignoreTimescale = false)
		{
			if (delayDuration > 0f)
			{
				yield return delay;
			}
			float timeLeft = duration;
			while (timeLeft > 0f)
			{
				targetTransform.transform.position = PLTween.Tween(duration - timeLeft, 0f, duration, origin, destination, curve);
				timeLeft -= ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
				yield return null;
			}
			targetTransform.transform.position = destination;
		}

		protected static IEnumerator MoveTransformCo(Transform targetTransform, Transform origin, Transform destination, WaitForSeconds delay, float delayDuration, float duration, 
			PLTween.PLTweenCurve curve, bool updatePosition = true, bool updateRotation = true, bool ignoreTimescale = false)
		{
			if (delayDuration > 0f)
			{
				yield return delay;
			}
			float timeLeft = duration;
			while (timeLeft > 0f)
			{
				if (updatePosition)
				{
					targetTransform.transform.position = PLTween.Tween(duration - timeLeft, 0f, duration, origin.position, destination.position, curve);
				}
				if (updateRotation)
				{
					targetTransform.transform.rotation = PLTween.Tween(duration - timeLeft, 0f, duration, origin.rotation, destination.rotation, curve);
				}
				timeLeft -= ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
				yield return null;
			}
			if (updatePosition) { targetTransform.transform.position = destination.position; }
			if (updateRotation) { targetTransform.transform.localEulerAngles = destination.localEulerAngles; }
		}

		protected static IEnumerator RotateTransformAroundCo(Transform targetTransform, Transform center, Transform destination, float angle, WaitForSeconds delay, float delayDuration, float duration, 
			PLTween.PLTweenCurve curve, bool ignoreTimescale = false)
		{
			if (delayDuration > 0f)
			{
				yield return delay;
			}

			Vector3 initialRotationPosition = targetTransform.transform.position;
			Quaternion initialRotationRotation = targetTransform.transform.rotation;

			float rate = 1f / duration;

			float timeSpent = 0f;
			while (timeSpent < duration)
			{

				float newAngle = PLTween.Tween(timeSpent, 0f, duration, 0f, angle, curve);

				targetTransform.transform.position = initialRotationPosition;
				initialRotationRotation = targetTransform.transform.rotation;
				targetTransform.RotateAround(center.transform.position, center.transform.up, newAngle);
				targetTransform.transform.rotation = initialRotationRotation;

				timeSpent += ignoreTimescale ? Time.unscaledDeltaTime : Time.deltaTime;
				yield return null;
			}
			targetTransform.transform.position = destination.position;
		}
	}
}