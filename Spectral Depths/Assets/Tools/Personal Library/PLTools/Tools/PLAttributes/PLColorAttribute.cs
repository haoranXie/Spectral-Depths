using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace SpectralDepths.Tools
{
	public class PLColorAttribute : PropertyAttribute
	{
		public Color color;

		public PLColorAttribute(float red = 1, float green = 0, float blue = 0)
		{
			this.color = new Color(red, green, blue, 1);
		}
	}
}