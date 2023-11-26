using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;

namespace SpectralDepths.TopDown
{
	public struct SelectionEvent
	{
        public Dictionary<int, GameObject> SelectedTable;

        public SelectionEvent(Dictionary<int, GameObject> selectedTable)
		{
            SelectedTable = selectedTable;

		}

		static SelectionEvent e;
        public static void Trigger(Dictionary<int, GameObject> selectedTable)
		{
            e.SelectedTable = selectedTable;
            MMEventManager.TriggerEvent(e);
		}
	}
}