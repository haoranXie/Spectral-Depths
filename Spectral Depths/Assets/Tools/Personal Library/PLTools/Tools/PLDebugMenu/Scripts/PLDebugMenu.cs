using System.Collections;
using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif
using UnityEngine.UI;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A debug menu helper, meant to help create quick mobile friendly debug menus
	/// </summary>
	public class PLDebugMenu : MonoBehaviour
	{
		/// the possible directions for the menu to appear
		public enum ToggleDirections { TopToBottom, LeftToRight, RightToLeft, BottomToTop }

		[Header("Data")]
		/// the scriptable object containing the menu's data
		public PLDebugMenuData Data;

		[Header("Bindings")]
		/// the container of the whole menu
		public CanvasGroup MenuContainer;
		/// the scrolling contents
		public RectTransform Contents;        
		/// the menu's background image
		public Image MenuBackground;
		/// the icon used to close the menu
		public Image CloseIcon;
		/// the tab bar (where the tab buttons go)
		public RectTransform TabBar;
		/// the tab contents container (where the contents of the page will go)
		public RectTransform TabContainer;
		/// the tab manager
		public PLDebugMenuTabManager TabManager;
		/// the SpectralDepths logo
		public Image PLLogo;

		[Header("Events")] 
		/// an event to call when the menu opens
		public UnityEvent OnOpenEvent;
		/// an event to call when the menu closes
		public UnityEvent OnCloseEvent;

		[Header("Test")]
		/// whether or not this menu is active at this moment
		[PLReadOnly]
		public bool Active = false;
		/// a test button to toggle the menu
		[PLInspectorButton("ToggleMenu")]
		public bool ToggleButton;

		protected RectTransform _containerRect;
		protected Vector3 _initialContainerPosition;
		protected Vector3 _offPosition;
		protected Vector3 _newPosition;
		protected bool _toggling = false;

		/// <summary>
		/// On Start we init our menu
		/// </summary>
		protected virtual void Start()
		{
			Initialization();
		}

		/// <summary>
		/// Prepares transitions and grabs components
		/// </summary>
		protected virtual void Initialization()
		{

			if (Data != null)
			{
				FillMenu();
			}

			CloseIcon.color = Data.TextColor;
			_containerRect = MenuContainer.GetComponent<RectTransform>();
			_initialContainerPosition = _containerRect.localPosition;
			MenuBackground.color = Data.BackgroundColor;
			switch (Data.ToggleDirection)
			{
				case ToggleDirections.RightToLeft:
					_offPosition = _initialContainerPosition + Vector3.right * _containerRect.rect.width;
					break;
				case ToggleDirections.LeftToRight:
					_offPosition = _initialContainerPosition + Vector3.left * _containerRect.rect.width;
					break;
				case ToggleDirections.TopToBottom:
					_offPosition = _initialContainerPosition + Vector3.up * _containerRect.rect.height;
					break;
				case ToggleDirections.BottomToTop:
					_offPosition = _initialContainerPosition + Vector3.down * _containerRect.rect.height;
					break;
			}
            
			_containerRect.localPosition = _offPosition;

		}

		/// <summary>
		/// Fills the menu based on the data's contents
		/// </summary>
		public virtual void FillMenu(bool triggerEvents = false)
		{
			int tabCounter = 0;
			if (PLLogo != null)
			{
				PLLogo.color = Data.TextColor;
			}            

			foreach (Transform child in Contents.transform)
			{
				GameObject.Destroy(child.gameObject);
			}
            
			foreach (Transform child in TabBar.transform)
				Destroy(child.gameObject);

			TabManager.Tabs.Clear();
			TabManager.TabsContents.Clear();

			foreach(PLDebugMenuTabData tab in Data.Tabs)
			{
				if (!tab.Active)
				{
					continue;
				}

				// create tab in the menu
				PLDebugMenuTab tabBarTab = Instantiate(Data.TabPrefab);
				tabBarTab.SelectedBackgroundColor = Data.TextColor;
				tabBarTab.SelectedTextColor = Data.BackgroundColor;
				tabBarTab.DeselectedBackgroundColor = Data.BackgroundColor;
				tabBarTab.DeselectedTextColor = Data.TextColor;
				tabBarTab.TabText.text = tab.Name;
				tabBarTab.TabText.font = Data.RegularFont;
				tabBarTab.transform.SetParent(TabBar);
				tabBarTab.Index = tabCounter;
				tabBarTab.Manager = TabManager;
				TabManager.Tabs.Add(tabBarTab);
                
				// create tab contents
				PLDebugMenuTabContents contents = Instantiate(Data.TabContentsPrefab);
				contents.transform.SetParent(TabContainer);
				RectTransform rectTransform = contents.GetComponent<RectTransform>();
				rectTransform.PLSetLeft(0f);
				rectTransform.PLSetRight(0f);
				rectTransform.PLSetTop(0f);
				rectTransform.PLSetBottom(0f);
				contents.Index = tabCounter;
				FillTab(contents, tabCounter, triggerEvents);
				if (tabCounter == Data.InitialActiveTabIndex)
				{
					contents.gameObject.SetActive(true);
					tabBarTab.Select();
				}
				else
				{
					contents.gameObject.SetActive(false);
					tabBarTab.Deselect();
				}
				TabManager.TabsContents.Add(contents);

				tabCounter++;
			}

			// debug tab
			if (Data.DisplayDebugTab)
			{
				PLDebugMenuTab tabBarTab = Instantiate(Data.TabPrefab);
				tabBarTab.SelectedBackgroundColor = Data.TextColor;
				tabBarTab.SelectedTextColor = Data.BackgroundColor;
				tabBarTab.DeselectedBackgroundColor = Data.BackgroundColor;
				tabBarTab.DeselectedTextColor = Data.TextColor;
				tabBarTab.TabText.text = Data.DebugTabName;
				tabBarTab.TabText.font = Data.RegularFont;
				tabBarTab.transform.SetParent(TabBar);
				tabBarTab.Index = tabCounter;
				tabBarTab.Manager = TabManager;
				TabManager.Tabs.Add(tabBarTab);

				PLDebugMenuDebugTab debugTab = Instantiate(Data.DebugTabPrefab);
				debugTab.DebugText.color = Data.TextColor;
				debugTab.DebugText.font = Data.RegularFont;
				debugTab.transform.SetParent(TabContainer);

				debugTab.CommandPrompt.textComponent.font = Data.RegularFont;
				debugTab.CommandPrompt.textComponent.color = Data.TextColor;

				debugTab.CommandPromptCharacter.font = Data.RegularFont;
				debugTab.CommandPromptCharacter.color = Data.TextColor;

				PLDebugMenuTabContents debugTabContents = debugTab.GetComponent<PLDebugMenuTabContents>();
				debugTabContents.Index = tabCounter;
				TabManager.TabsContents.Add(debugTabContents);
				RectTransform rectTransform = debugTabContents.GetComponent<RectTransform>();
				rectTransform.PLSetLeft(0f);
				rectTransform.PLSetRight(0f);
				rectTransform.PLSetTop(0f);
				rectTransform.PLSetBottom(0f);
				if (tabCounter == Data.InitialActiveTabIndex)
				{
					debugTab.gameObject.SetActive(true);
					TabManager.Tabs[tabCounter].Select();
				}
				else
				{
					debugTab.gameObject.SetActive(false);
					TabManager.Tabs[tabCounter].Deselect();
				}
				tabCounter++;
			}

			// fill with spacers 
			int spacerCount = Data.MaxTabs - tabCounter;
			for (int i = 0; i < spacerCount; i++)
			{
				RectTransform spacer = Instantiate(Data.TabSpacerPrefab);
				spacer.transform.SetParent(TabBar);
			}

		}

		protected virtual void FillTab(PLDebugMenuTabContents tab, int index, bool triggerEvents = false)
		{
			Transform parent = tab.Parent;

			foreach (PLDebugMenuItem item in Data.Tabs[index].MenuItems)
			{
				if (!item.Active)
				{
					continue; 
				}

				switch (item.Type)
				{
					case PLDebugMenuItem.PLDebugMenuItemTypes.Button:
						PLDebugMenuItemButton button;
						button = (item.ButtonType == PLDebugMenuItem.PLDebugMenuItemButtonTypes.Border) ? Instantiate(Data.ButtonBorderPrefab) : Instantiate(Data.ButtonPrefab);
						button.name = "PLDebugMenuItemButton_" + item.Name;
						button.ButtonText.text = item.ButtonText;
						button.ButtonEventName = item.ButtonEventName;
						if (item.ButtonType == PLDebugMenuItem.PLDebugMenuItemButtonTypes.Border)
						{
							button.ButtonText.color = Data.AccentColor;
							button.ButtonBg.color = Data.TextColor;
						}
						else
						{
							button.ButtonText.color = Data.BackgroundColor;
							button.ButtonBg.color = Data.AccentColor;
						}
						button.ButtonText.font = Data.RegularFont;
						button.transform.SetParent(parent);
						break;

					case PLDebugMenuItem.PLDebugMenuItemTypes.Checkbox:
						PLDebugMenuItemCheckbox checkbox = Instantiate(Data.CheckboxPrefab);
						checkbox.name = "PLDebugMenuItemCheckbox_" + item.Name;
						checkbox.SwitchText.text = item.CheckboxText;
						if (item.CheckboxInitialState)
						{
							checkbox.Switch.SetTrue();
						}
						else
						{
							checkbox.Switch.SetFalse();
						}
						checkbox.CheckboxEventName = item.CheckboxEventName;
						checkbox.transform.SetParent(parent);
						checkbox.Switch.GetComponent<Image>().color = Data.AccentColor;
						checkbox.SwitchText.color = Data.TextColor;
						checkbox.SwitchText.font = Data.RegularFont;
						break;

					case PLDebugMenuItem.PLDebugMenuItemTypes.Slider:
						PLDebugMenuItemSlider slider = Instantiate(Data.SliderPrefab);
						slider.name = "PLDebugMenuItemSlider_" + item.Name;
						slider.Mode = item.SliderMode;
						slider.RemapZero = item.SliderRemapZero;
						slider.RemapOne = item.SliderRemapOne;
						slider.TargetSlider.value = PLMaths.Remap(item.SliderInitialValue, item.SliderRemapZero, item.SliderRemapOne, 0f, 1f);
						slider.transform.SetParent(parent);

						slider.SliderText.text = item.SliderText;
						slider.SliderText.color = Data.TextColor;
						slider.SliderText.font = Data.RegularFont;

						slider.SliderValueText.text = (item.SliderMode == PLDebugMenuItemSlider.Modes.Int) ? item.SliderInitialValue.ToString() : item.SliderInitialValue.ToString("F3");
						slider.SliderValueText.color = Data.AccentColor;
						slider.SliderValueText.font = Data.BoldFont;

						slider.SliderKnob.color = Data.AccentColor;
						slider.SliderLine.color = Data.TextColor;

						slider.SliderEventName = item.SliderEventName;
						break;

					case PLDebugMenuItem.PLDebugMenuItemTypes.Spacer:
						GameObject spacerPrefab = (item.SpacerType == PLDebugMenuItem.PLDebugMenuItemSpacerTypes.Small) ? Data.SpacerSmallPrefab : Data.SpacerBigPrefab;
						GameObject spacer = Instantiate(spacerPrefab);
						spacer.name = "PLDebugMenuItemSpacer_" + item.Name;
						spacer.transform.SetParent(parent);
						break;

					case PLDebugMenuItem.PLDebugMenuItemTypes.Title:
						PLDebugMenuItemTitle title = Instantiate(Data.TitlePrefab);
						title.name = "PLDebugMenuItemSlider_" + item.Name;
						title.TitleText.text = item.TitleText;
						title.TitleText.color = Data.TextColor;
						title.TitleText.font = Data.BoldFont;
						title.TitleLine.color = Data.AccentColor;
						title.transform.SetParent(parent);
						break;

					case PLDebugMenuItem.PLDebugMenuItemTypes.Choices:
						PLDebugMenuItemChoices choicesPrefab;
						if (item.ChoicesType == PLDebugMenuItem.PLDebugMenuItemChoicesTypes.TwoChoices)
						{
							choicesPrefab = Data.TwoChoicesPrefab;
						}
						else
						{
							choicesPrefab = Data.ThreeChoicesPrefab;
						}

						PLDebugMenuItemChoices choices = Instantiate(choicesPrefab);
						choices.name = "PLDebugMenuItemChoices_" + item.Name;

						choices.Choices[0].ButtonText.text = item.ChoiceOneText;
						choices.Choices[1].ButtonText.text = item.ChoiceTwoText;

						choices.Choices[0].ButtonEventName = item.ChoiceOneEventName;
						choices.Choices[1].ButtonEventName = item.ChoiceTwoEventName;

						if (item.ChoicesType == PLDebugMenuItem.PLDebugMenuItemChoicesTypes.ThreeChoices)
						{
							choices.Choices[2].ButtonEventName = item.ChoiceThreeEventName;
							choices.Choices[2].ButtonText.text = item.ChoiceThreeText;
						}

						choices.OffColor = Data.BackgroundColor;
						choices.OnColor = Data.TextColor;
						choices.AccentColor = Data.AccentColor;

						foreach (PLDebugMenuChoiceEntry entry in choices.Choices)
						{
							if (entry != null)
							{
								entry.ButtonText.font = Data.RegularFont;
							}
						}

						choices.Select(item.SelectedChoice);
						if (triggerEvents)
							choices.TriggerButtonEvent(item.SelectedChoice);
                        
						choices.transform.SetParent(parent);
						break;

					case PLDebugMenuItem.PLDebugMenuItemTypes.Value:
						PLDebugMenuItemValue value = Instantiate(Data.ValuePrefab);
						value.name = "PLDebugMenuItemValue_" + item.Name;
						value.LabelText.text = item.ValueLabel;
						value.LabelText.color = Data.TextColor;
						value.LabelText.font = Data.RegularFont;
						value.ValueText.text = item.ValueInitialValue;
						value.ValueText.color = Data.AccentColor;
						value.ValueText.font = Data.BoldFont;
						value.RadioReceiver.Channel = item.ValueMMRadioReceiverChannel;
						value.transform.SetParent(parent);
						break;

					case PLDebugMenuItem.PLDebugMenuItemTypes.Text:

						PLDebugMenuItemText textPrefab;
						switch (item.TextType)
						{
							case PLDebugMenuItem.PLDebugMenuItemTextTypes.Tiny:
								textPrefab = Data.TextTinyPrefab;
								break;
							case PLDebugMenuItem.PLDebugMenuItemTextTypes.Small:
								textPrefab = Data.TextSmallPrefab;
								break;
							case PLDebugMenuItem.PLDebugMenuItemTextTypes.Long:
								textPrefab = Data.TextLongPrefab;
								break;
							default:
								textPrefab = Data.TextTinyPrefab;
								break;
						}
						PLDebugMenuItemText text = Instantiate(textPrefab);
						text.name = "PLDebugMenuItemText_" + item.Name;
						text.ContentText.text = item.TextContents;
						text.ContentText.color = Data.TextColor;
						text.ContentText.font = Data.RegularFont;
						text.transform.SetParent(parent);
						break;
				}
			}

			// we always add a spacer at the end because scrollviews are terrible
			GameObject finalSpacer = Instantiate(Data.SpacerBigPrefab);
			finalSpacer.name = "PLDebugMenuItemSpacer_FinalSpacer";
			finalSpacer.transform.SetParent(parent);
		}

		/// <summary>
		/// Makes the menu appear
		/// </summary>
		public virtual void OpenMenu()
		{
			OnOpenEvent?.Invoke();
			StartCoroutine(ToggleCo(false));
		}

		/// <summary>
		/// Makes the menu disappear
		/// </summary>
		public virtual void CloseMenu()
		{
			StartCoroutine(ToggleCo(true));
		}

		/// <summary>
		/// Closes or opens the menu depending on its current state
		/// </summary>
		public virtual void ToggleMenu()
		{
			StartCoroutine(ToggleCo(Active));
		}

		/// <summary>
		/// A coroutine used to toggle the menu
		/// </summary>
		/// <param name="active"></param>
		/// <returns></returns>
		protected virtual IEnumerator ToggleCo(bool active)
		{
			if (_toggling)
			{
				yield break;
			}
			if (!active)
			{
				OnOpenEvent?.Invoke();
				_containerRect.gameObject.SetActive(true);
			}
			_toggling = true;
			Active = active;
			_newPosition = active ? _offPosition : _initialContainerPosition;
			PLTween.MoveRectTransform(this, _containerRect, _containerRect.localPosition, _newPosition, null, 0f, Data.ToggleDuration, Data.ToggleCurve, ignoreTimescale:true);
			yield return PLCoroutine.WaitForUnscaled(Data.ToggleDuration);
			if (active)
			{
				OnCloseEvent?.Invoke();
				_containerRect.gameObject.SetActive(false);
			}
			Active = !active;
			_toggling = false;
		}

		/// <summary>
		/// On update we handle our input
		/// </summary>
		protected virtual void Update()
		{
			HandleInput();
		}

		/// <summary>
		/// Looks for shortcut input
		/// </summary>
		protected virtual void HandleInput()
		{
			bool input = false;
			
			#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
				input = Keyboard.current[Data.ToggleKey].wasPressedThisFrame;
			#else
			input = Input.GetKeyDown(Data.ToggleShortcut);
			#endif
	        
			if (input)
			{
				ToggleMenu();
			}
		}

		/// <summary>
		/// Routes console logs to the PLDebugConsole 
		/// </summary>
		/// <param name="logString"></param>
		/// <param name="stackTrace"></param>
		/// <param name="type"></param>
		protected virtual void CaptureConsoleLog(string logString, string stackTrace, LogType type)
		{
			PLDebug.LogDebugToConsole(logString + " (" + type + ")", "#00FFFF", 3, false);
		}
     
		/// <summary>
		/// On Enable, we start listening for log messages
		/// </summary>   
		protected virtual void OnEnable()
		{
			Application.logMessageReceived += CaptureConsoleLog;
		}
     
		/// <summary>
		/// On Disable, we stop listening for log messages
		/// </summary>
		protected virtual void OnDisable()
		{
			Application.logMessageReceived -= CaptureConsoleLog;
		}
	}

}