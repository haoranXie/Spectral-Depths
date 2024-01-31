using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// An attribute to add to static methods to they can be called via the PLDebugMenu's command line
	/// </summary>
	[AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false)]
	public class PLDebugLogCommandAttribute : System.Attribute { }
}