using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// RectTransform extensions
	/// </summary>
	public static class PLRectTransformExtensions
	{
		/// <summary>
		/// Sets the left offset of a rect transform to the specified value
		/// </summary>
		/// <param name="rt"></param>
		/// <param name="left"></param>
		public static void PLSetLeft(this RectTransform rt, float left)
		{
			rt.offsetMin = new Vector2(left, rt.offsetMin.y);
		}

		/// <summary>
		/// Sets the right offset of a rect transform to the specified value
		/// </summary>
		/// <param name="rt"></param>
		/// <param name="right"></param>
		public static void PLSetRight(this RectTransform rt, float right)
		{
			rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
		}

		/// <summary>
		/// Sets the top offset of a rect transform to the specified value
		/// </summary>
		/// <param name="rt"></param>
		/// <param name="top"></param>
		public static void PLSetTop(this RectTransform rt, float top)
		{
			rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
		}

		/// <summary>
		/// Sets the bottom offset of a rect transform to the specified value
		/// </summary>
		/// <param name="rt"></param>
		/// <param name="bottom"></param>
		public static void PLSetBottom(this RectTransform rt, float bottom)
		{
			rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
		}
	}
}