using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A class used to store choices contents
	/// </summary>
	[System.Serializable]
	public class PLDebugMenuChoiceEntry
	{
		/// the button associated to this choice
		public Button TargetButton;
		/// the text comp used to display the button's text
		public Text ButtonText;
		/// the button's background image comp
		public Image ButtonBg;
		/// the name of the event bound to this button
		public string ButtonEventName = "ButtonEvent";
	}

	/// <summary>
	/// A class used to bind a Choice menu item to a PLDebugMenu
	/// </summary>
	public class PLDebugMenuItemChoices : MonoBehaviour
	{
		[Header("Bindings")]
		/// the sprite to use when the button is active
		public Sprite SelectedSprite;
		/// the sprite to use as bg when the button is inactive
		public Sprite OffSprite;
		/// the color to use when the button is active
		public Color OnColor = Color.white;
		/// the color to use when the button is inactive
		public Color OffColor = Color.black;
		/// the color to use when the button is accented
		public Color AccentColor = PLColors.ReunoYellow;
		/// a list of choices
		public List<PLDebugMenuChoiceEntry> Choices;

		/// <summary>
		/// Triggers a button event of the selected index
		/// </summary>
		/// <param name="index"></param>
		public virtual void TriggerButtonEvent(int index)
		{
			PLDebugMenuButtonEvent.Trigger(Choices[index].ButtonEventName);
		}

		/// <summary>
		/// Selects one of the buttons
		/// </summary>
		/// <param name="index"></param>
		public virtual void Select(int index)
		{
			Deselect();
			Choices[index].ButtonBg.sprite = SelectedSprite;
			Choices[index].ButtonBg.color = AccentColor;
			Choices[index].ButtonText.color = OffColor;
		}

		/// <summary>
		/// Deselects all buttons
		/// </summary>
		public virtual void Deselect()
		{
			foreach(PLDebugMenuChoiceEntry entry in Choices)
			{
				entry.ButtonBg.sprite = OffSprite;
				entry.ButtonBg.color = OnColor;
				entry.ButtonText.color = OnColor;
			}
		}
	}
}