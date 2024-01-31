using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpectralDepths.PLInterface
{
	[RequireComponent(typeof(Image))]
	public class PLScrollviewButton : Button
	{
		public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
		{

		}

		public override void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
		{
			if (this.interactable)
			{
				base.OnPointerExit(eventData);
				if (!eventData.dragging)
				{
					base.OnPointerClick(eventData);
				}
			}
		}

		public override void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
		{

		}
	}
}