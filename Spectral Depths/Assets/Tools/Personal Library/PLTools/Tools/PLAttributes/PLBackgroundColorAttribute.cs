using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace SpectralDepths.Tools
{
	public enum PLBackgroundAttributeColor
	{
		Red,
		Pink,
		Orange,
		Yellow,
		Green,
		Blue,
		Violet,
		White
	}

	public class PLBackgroundColorAttribute : PropertyAttribute
	{
		public PLBackgroundAttributeColor Color;

		public PLBackgroundColorAttribute(PLBackgroundAttributeColor color = PLBackgroundAttributeColor.Yellow)
		{
			this.Color = color;
		}
	}
}