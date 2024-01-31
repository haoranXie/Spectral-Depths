using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// Vector3 Extensions
	/// </summary>
	public static class PLVector3Extensions
	{
		/// <summary>
		/// Sets the x value of a vector
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="newValue"></param>
		/// <returns></returns>
		public static Vector3 PLSetX(this Vector3 vector, float newValue)
		{
			vector.x = newValue;
			return vector;
		}

		/// <summary>
		/// Sets the y value of a vector
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="newValue"></param>
		/// <returns></returns>
		public static Vector3 PLSetY(this Vector3 vector, float newValue)
		{
			vector.y = newValue;
			return vector;
		}

		/// <summary>
		/// Sets the z value of a vector
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="newValue"></param>
		/// <returns></returns>
		public static Vector3 PLSetZ(this Vector3 vector, float newValue)
		{
			vector.z = newValue;
			return vector;
		}

		/// <summary>
		/// Inverts a vector
		/// </summary>
		/// <param name="newValue"></param>
		/// <returns></returns>
		public static Vector3 PLInvert(this Vector3 newValue)
		{
			return new Vector3
			(
				1.0f / newValue.x,
				1.0f / newValue.y,
				1.0f / newValue.z
			);
		}

		/// <summary>
		/// Projects a vector on another
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="projectedVector"></param>
		/// <returns></returns>
		public static Vector3 PLProject(this Vector3 vector, Vector3 projectedVector)
		{
			float _dot = Vector3.Dot(vector, projectedVector);
			return _dot * projectedVector;
		}

		/// <summary>
		/// Rejects a vector on another
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="rejectedVector"></param>
		/// <returns></returns>
		public static Vector3 PLReject(this Vector3 vector, Vector3 rejectedVector)
		{
			return vector - vector.PLProject(rejectedVector);
		}

		/// <summary>
		/// Rounds all components of a vector
		/// </summary>
		/// <param name="vector"></param>
		/// <returns></returns>
		public static Vector3 PLRound(this Vector3 vector)
		{
			vector.x = Mathf.Round(vector.x);
			vector.y = Mathf.Round(vector.y);
			vector.z = Mathf.Round(vector.z);
			return vector;
		}
	}
}