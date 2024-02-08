using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpectralDepths.Tools
{
	[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
	public class PLHiddenPropertiesAttribute : Attribute
	{
		public string[] PropertiesNames;

		public PLHiddenPropertiesAttribute(params string[] propertiesNames)
		{
			PropertiesNames = propertiesNames;
		}
	}
}