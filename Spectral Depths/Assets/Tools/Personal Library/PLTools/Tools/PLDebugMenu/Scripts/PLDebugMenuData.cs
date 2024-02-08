using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A class used to store and display a reorderable list of menu items
	/// </summary>
	[Serializable]
	public class PLDebugMenuItemList : PLReorderableArray<PLDebugMenuItem>
	{

	}

	[Serializable]
	public class PLDebugMenuTabData
	{
		public string Name = "TabName";
		public bool Active = true;
		[PLReorderableAttribute]
		public PLDebugMenuItemList MenuItems;
	}

	/// <summary>
	/// A class used to store a menu item
	/// </summary>
	[Serializable]
	public class PLDebugMenuItem
	{
		// EDITOR NAME
		public string Name;
		public bool Active = true;
		public enum PLDebugMenuItemTypes { Title, Spacer, Button, Checkbox, Slider, Text, Value, Choices }

		public PLDebugMenuItemTypes Type = PLDebugMenuItemTypes.Title;

		// TITLE
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Title)]
		public string TitleText = "Title text";

		// TEXT
		public enum PLDebugMenuItemTextTypes { Tiny, Small, Long }
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Text)]
		public PLDebugMenuItemTextTypes TextType = PLDebugMenuItemTextTypes.Tiny;
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Text)]
		public string TextContents = "Lorem ipsum dolor sit amet";

		// CHOICES 
		public enum PLDebugMenuItemChoicesTypes { TwoChoices, ThreeChoices }
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Choices)]
		public PLDebugMenuItemChoicesTypes ChoicesType = PLDebugMenuItemChoicesTypes.TwoChoices;
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Choices)]
		public string ChoiceOneText;
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Choices)]
		public string ChoiceOneEventName = "ChoiceOneEvent";
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Choices)]
		public string ChoiceTwoText;
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Choices)]
		public string ChoiceTwoEventName = "ChoiceTwoEvent";
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Choices)]
		public string ChoiceThreeText;
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Choices)]
		public string ChoiceThreeEventName = "ChoiceThreeEvent";
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Choices)]
		public int SelectedChoice = 0;

		// VALUE
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Value)]
		public string ValueLabel = "Value Label";
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Value)]
		public string ValueInitialValue = "255";
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Value)]
		public int ValueMMRadioReceiverChannel = 0;

		// BUTTON
		public enum PLDebugMenuItemButtonTypes { Border, Full }
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Button)]
		public string ButtonText = "Button text";
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Button)]
		public PLDebugMenuItemButtonTypes ButtonType = PLDebugMenuItemButtonTypes.Border;
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Button)]
		public string ButtonEventName = "Button";

		// SPACER
		public enum PLDebugMenuItemSpacerTypes { Small, Big }
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Spacer)]
		public PLDebugMenuItemSpacerTypes SpacerType = PLDebugMenuItemSpacerTypes.Small;

		// CHECKBOX
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Checkbox)]
		public string CheckboxText;
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Checkbox)]
		public bool CheckboxInitialState = false;
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Checkbox)]
		public string CheckboxEventName = "CheckboxEventName";

		// SLIDER
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Slider)]
		public PLDebugMenuItemSlider.Modes SliderMode = PLDebugMenuItemSlider.Modes.Float;
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Slider)]
		public string SliderText;
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Slider)]
		public float SliderRemapZero = 0f;
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Slider)]
		public float SliderRemapOne = 1f;
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Slider)]
		public float SliderInitialValue = 0f;
		[PLEnumCondition("Type", (int)PLDebugMenuItemTypes.Slider)]
		public string SliderEventName = "Slider";

		[PLHidden]
		public PLDebugMenuItemSlider TargetSlider;
		[PLHidden]
		public PLDebugMenuItemButton TargetButton;
		[PLHidden]
		public PLDebugMenuItemCheckbox TargetCheckbox;
	}

	/// <summary>
	/// A data class used to store the contents of a debug menu
	/// </summary>
	[CreateAssetMenu(fileName = "PLDebugMenuData", menuName = "SpectralDepths/PLDebugMenu/PLDebugMenuData")]
	public class PLDebugMenuData : ScriptableObject
	{
		[Header("Prefabs")]
		public PLDebugMenuItemTitle TitlePrefab;
		public PLDebugMenuItemButton ButtonPrefab;
		public PLDebugMenuItemButton ButtonBorderPrefab;
		public PLDebugMenuItemCheckbox CheckboxPrefab;
		public PLDebugMenuItemSlider SliderPrefab;
		public GameObject SpacerSmallPrefab;
		public GameObject SpacerBigPrefab;
		public PLDebugMenuItemText TextTinyPrefab;
		public PLDebugMenuItemText TextSmallPrefab;
		public PLDebugMenuItemText TextLongPrefab;
		public PLDebugMenuItemValue ValuePrefab;
		public PLDebugMenuItemChoices TwoChoicesPrefab;
		public PLDebugMenuItemChoices ThreeChoicesPrefab;
		public PLDebugMenuTab TabPrefab;
		public PLDebugMenuTabContents TabContentsPrefab;
		public RectTransform TabSpacerPrefab;
		public PLDebugMenuDebugTab DebugTabPrefab;
		public string DebugTabName = "Logs";

		[Header("Tabs")]
		public List<PLDebugMenuTabData> Tabs;
		public bool DisplayDebugTab = true;
		public int MaxTabs = 5;
		public int InitialActiveTabIndex = 0;
        
		[Header("Toggle")]
		public PLDebugMenu.ToggleDirections ToggleDirection = PLDebugMenu.ToggleDirections.RightToLeft;
		public float ToggleDuration = 0.2f;
		public PLTween.PLTweenCurve ToggleCurve = PLTween.PLTweenCurve.EaseInCubic;
        
		#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
			public Key ToggleKey = Key.Backquote;
		#else
		public KeyCode ToggleShortcut = KeyCode.Quote;
		#endif

		[Header("Style")]
		public Font RegularFont;
		public Font BoldFont;
		public Color BackgroundColor = Color.black;
		public Color AccentColor = PLColors.ReunoYellow;
		public Color TextColor = Color.white;
	}
}