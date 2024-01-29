using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.Feedbacks
{
	[Serializable]
	public class PLF_Button
	{
		public delegate void ButtonMethod();

		public string ButtonText;
		public ButtonMethod TargetMethod;

		public PLF_Button(string buttonText, ButtonMethod method)
		{
			ButtonText = buttonText;
			TargetMethod = method;
		}
	}
}