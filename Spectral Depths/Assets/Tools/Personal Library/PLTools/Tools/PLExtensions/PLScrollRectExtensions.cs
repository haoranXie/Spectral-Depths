using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// Scrollrect extensions
	/// </summary>
	public static class ScrollRectExtensions
	{
		/// <summary>
		/// Scrolls a scroll rect to the top
		/// </summary>
		/// <param name="scrollRect"></param>
		public static void PLScrollToTop(this ScrollRect scrollRect)
		{
			scrollRect.normalizedPosition = new Vector2(0, 1);
		}

		/// <summary>
		/// Scrolls a scroll rect to the bottom
		/// </summary>
		public static void PLScrollToBottom(this ScrollRect scrollRect)
		{
			scrollRect.normalizedPosition = new Vector2(0, 0);
		}
	}
}