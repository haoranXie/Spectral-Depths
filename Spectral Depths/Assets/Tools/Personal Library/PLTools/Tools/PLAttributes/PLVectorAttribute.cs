using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpectralDepths.Tools
{
	public class PLVectorAttribute : PropertyAttribute
	{
		public readonly string[] Labels;

		public PLVectorAttribute(params string[] labels)
		{
			Labels = labels;
		}
	}
}