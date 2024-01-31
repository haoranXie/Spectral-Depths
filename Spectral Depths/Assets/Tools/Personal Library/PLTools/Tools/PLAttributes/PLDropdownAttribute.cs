using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpectralDepths.Tools
{
	public class PLDropdownAttribute : PropertyAttribute
	{
		public readonly object[] DropdownValues;

		public PLDropdownAttribute(params object[] dropdownValues)
		{
			DropdownValues = dropdownValues;
		}
	}
}