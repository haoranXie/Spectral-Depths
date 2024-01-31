using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A class used to bind a text item to a PLDebugMenu
	/// </summary>
	public class PLDebugMenuItemText : MonoBehaviour
	{
		[Header("Bindings")]
		/// a text comp used to display the text
		[TextArea]
		public Text ContentText;
	}
}