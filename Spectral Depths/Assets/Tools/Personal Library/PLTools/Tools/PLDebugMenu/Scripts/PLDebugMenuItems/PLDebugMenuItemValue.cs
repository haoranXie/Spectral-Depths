﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A class used to bind a value item to a PLDebugMenu
	/// </summary>
	public class PLDebugMenuItemValue : MonoBehaviour
	{
		[Header("Bindings")]
		/// the label to display next to the value
		public Text LabelText;
		/// the text comp to display the value with
		public Text ValueText;
		/// a radio receiver to update the value with
		public PLRadioReceiver RadioReceiver;
		/// the current level of this value item
		public float Level { get { return _level;  } set { _level = value;  ValueText.text = value.ToString("F2"); } }

		protected float _level;
	}
}