using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A class used to keep track of tabs and their contents in a PLDebugMenu
	/// </summary>
	public class PLDebugMenuTabManager : MonoBehaviour
	{
		/// a list of all the tabs under that manager
		public List<PLDebugMenuTab> Tabs;
		/// a list of all the tabs contents under that manager
		public List<PLDebugMenuTabContents> TabsContents;

		/// <summary>
		/// Selects a tab, hides the others
		/// </summary>
		/// <param name="selected"></param>
		public virtual void Select(int selected)
		{
			foreach(PLDebugMenuTab tab in Tabs)
			{
				if (tab.Index != selected)
				{
					tab.Deselect();
				}
			}
			foreach(PLDebugMenuTabContents contents in TabsContents)
			{
				if (contents.Index == selected)
				{
					contents.gameObject.SetActive(true);
				}
				else
				{
					contents.gameObject.SetActive(false);
				}
			}
		}
	}
}