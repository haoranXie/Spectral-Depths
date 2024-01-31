using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace SpectralDepths.Feedbacks
{
	public class PLFInspectorGroupData
	{
		public bool GroupIsOpen;
		public PLFInspectorGroupAttribute GroupAttribute;
		public List<SerializedProperty> PropertiesList = new List<SerializedProperty>();
		public HashSet<string> GroupHashSet = new HashSet<string>();
		public Color GroupColor;

		public void ClearGroup()
		{
			GroupAttribute = null;
			GroupHashSet.Clear();
			PropertiesList.Clear();
		}
	}
    
	public class PLF_FeedbackInspector 
	{
		public bool DrawerInitialized;
		public List<SerializedProperty> PropertiesList = new List<SerializedProperty>();
		public Dictionary<string, PLFInspectorGroupData> GroupData = new Dictionary<string, PLFInspectorGroupData>();
        
		private string[] _mmHiddenPropertiesToHide;
		private bool _hasMMHiddenProperties = false;
		protected bool _shouldDrawBase = true;
		protected SerializedProperty _currentProperty;
		protected PLF_Feedback _feedback;
		protected bool _expandGroupInspectors;
		private const string _channelFieldName = "Channel";
		private const string _channelModeFieldName = "ChannelMode";
		private const string _channelDefinitionFieldName = "PLChannelDefinition";
		private const string _automatedTargetAcquisitionName = "AutomatedTargetAcquisition";
        
		public virtual void OnEnable()
		{
			DrawerInitialized = false;
			PropertiesList.Clear();
			GroupData.Clear();
            
			PLFHiddenPropertiesAttribute[] hiddenProperties = (PLFHiddenPropertiesAttribute[])_currentProperty.GetType().GetCustomAttributes(typeof(PLFHiddenPropertiesAttribute), false);
			if (hiddenProperties != null && hiddenProperties.Length > 0 && hiddenProperties[0].PropertiesNames != null)
			{
				_mmHiddenPropertiesToHide = hiddenProperties[0].PropertiesNames;
				_hasMMHiddenProperties = true;
			}
		}

		public virtual void OnDisable()
		{
			foreach (KeyValuePair<string, PLFInspectorGroupData> groupData in GroupData)
			{
				if (groupData.Value != null)
				{
					EditorPrefs.SetBool(string.Format($"{groupData.Value.GroupAttribute.GroupName}{groupData.Value.PropertiesList[0].name}{_feedback.UniqueID}"), groupData.Value.GroupIsOpen);
					groupData.Value.ClearGroup();    
				}
			}
		}
		
		protected Dictionary<string,PLFConditionAttribute> _conditionDictionary = new Dictionary<string,PLFConditionAttribute>();
		protected Dictionary<string,PLFEnumConditionAttribute> _enumConditionDictionary = new Dictionary<string,PLFEnumConditionAttribute>();
		protected PLFConditionAttribute _conditionAttributeStore;
		protected PLFEnumConditionAttribute _enumConditionAttributeStore;

		public virtual void Initialization(SerializedProperty currentProperty, PLF_Feedback feedback, bool expandGroupInspectors)
		{
			if (DrawerInitialized)
			{
				return;
			}
            
			_expandGroupInspectors = expandGroupInspectors;
			_currentProperty = currentProperty;
			_feedback = feedback;
			_conditionDictionary.Clear();
			_enumConditionDictionary.Clear();
			
			List<FieldInfo> fieldInfoList;
			PLFInspectorGroupAttribute previousGroupAttribute = default;
			int fieldInfoLength = PLF_FieldInfo.GetFieldInfo(feedback, out fieldInfoList);
            
			for (int i = 0; i < fieldInfoLength; i++)
			{
				SearchForConditions(fieldInfoList[i]);
				PLFInspectorGroupAttribute group = Attribute.GetCustomAttribute(fieldInfoList[i], typeof(PLFInspectorGroupAttribute)) as PLFInspectorGroupAttribute;

				PLFInspectorGroupData groupData;
				if (group == null)
				{
					if (previousGroupAttribute != null && previousGroupAttribute.GroupAllFieldsUntilNextGroupAttribute)
					{
						_shouldDrawBase = false;
						if (!GroupData.TryGetValue(previousGroupAttribute.GroupName, out groupData))
						{
							if (!ShouldSkipGroup(previousGroupAttribute.GroupName))
							{
								GroupData.Add(previousGroupAttribute.GroupName, new PLFInspectorGroupData
								{
									GroupAttribute = previousGroupAttribute,
									GroupHashSet = new HashSet<string> { fieldInfoList[i].Name },
									GroupColor = PLFeedbacksColors.GetColorAt(previousGroupAttribute.GroupColorIndex)
								});
							}
						}
						else
						{
							groupData.GroupColor = PLFeedbacksColors.GetColorAt(previousGroupAttribute.GroupColorIndex);
							groupData.GroupHashSet.Add(fieldInfoList[i].Name);
						}
					}

					continue;
				}
                
				previousGroupAttribute = group;

				if (!GroupData.TryGetValue(group.GroupName, out groupData))
				{
					bool fallbackOpenState = _expandGroupInspectors;
					if (group.ClosedByDefault) { fallbackOpenState = false; }
					bool groupIsOpen = EditorPrefs.GetBool(string.Format($"{group.GroupName}{fieldInfoList[i].Name}{feedback.UniqueID}"), fallbackOpenState);

					if (!ShouldSkipGroup(previousGroupAttribute.GroupName))
					{
						GroupData.Add(group.GroupName, new PLFInspectorGroupData
						{
							GroupAttribute = group,
							GroupColor = PLFeedbacksColors.GetColorAt(previousGroupAttribute.GroupColorIndex),
							GroupHashSet = new HashSet<string> { fieldInfoList[i].Name }, GroupIsOpen = groupIsOpen 
						});	
					}
				}
				else
				{
					groupData.GroupHashSet.Add(fieldInfoList[i].Name);
					groupData.GroupColor = PLFeedbacksColors.GetColorAt(previousGroupAttribute.GroupColorIndex);
				}
			}

			if (currentProperty.NextVisible(true))
			{
				do
				{
					FillPropertiesList(currentProperty);
				} while (currentProperty.NextVisible(false));
			}

			DrawerInitialized = true;
		}

		protected virtual bool ShouldSkipGroup(string groupName)
		{
			bool skip = false;
            
			if (groupName == PLF_Feedback._randomnessGroupName && !_feedback.HasRandomness)
			{
				skip = true;
			}

			if (groupName == PLF_Feedback._rangeGroupName && !_feedback.HasRange)
			{
				skip = true;
			}

			return skip;
		}

		protected virtual void SearchForConditions(FieldInfo fieldInfo)
		{
			_conditionAttributeStore = Attribute.GetCustomAttribute(fieldInfo, typeof(PLFConditionAttribute)) as PLFConditionAttribute;
			if (_conditionAttributeStore != null)
			{
				_conditionDictionary.Add(fieldInfo.Name, _conditionAttributeStore);
			}
			_enumConditionAttributeStore = Attribute.GetCustomAttribute(fieldInfo, typeof(PLFEnumConditionAttribute)) as PLFEnumConditionAttribute;
			if (_enumConditionAttributeStore != null)
			{
				_enumConditionDictionary.Add(fieldInfo.Name, _enumConditionAttributeStore);
			}
		}
        
		public void FillPropertiesList(SerializedProperty serializedProperty)
		{
			bool shouldClose = false;

			foreach (KeyValuePair<string, PLFInspectorGroupData> pair in GroupData)
			{
				if (pair.Value.GroupHashSet.Contains(serializedProperty.name))
				{
					SerializedProperty property = serializedProperty.Copy();
					shouldClose = true;
					pair.Value.PropertiesList.Add(property);
					break;
				}
			}

			if (!shouldClose)
			{
				SerializedProperty property = serializedProperty.Copy();
				PropertiesList.Add(property);
			}
		}

		public void DrawInspector(SerializedProperty currentProperty, PLF_Feedback feedback)
		{
			Initialization(currentProperty, feedback, _expandGroupInspectors);
			if (!DrawBase(currentProperty, feedback))
			{
				DrawContainer(feedback);
				DrawContents(feedback);    
			}
		}
        
		protected virtual bool DrawBase(SerializedProperty currentProperty, PLF_Feedback feedback)
		{
			if (_shouldDrawBase || !feedback.DrawGroupInspectors)
			{
				DrawNoGroupInspector(currentProperty, feedback);
				return true;
			}
			else
			{
				return false;
			}
		}

		protected virtual void DrawContainer(PLF_Feedback feedback)
		{
			if (PropertiesList.Count == 0)
			{
				return;
			}
            
			foreach (KeyValuePair<string, PLFInspectorGroupData> pair in GroupData)
			{
				DrawVerticalLayout(() => DrawGroup(pair.Value, feedback), PLF_FeedbackInspectorStyle.ContainerStyle);
				EditorGUI.indentLevel = 0;
			}
		}

		protected virtual void DrawContents(PLF_Feedback feedback)
		{
			if (PropertiesList.Count == 0)
			{
				return;
			}

			EditorGUILayout.Space();
			for (int i = 1; i < PropertiesList.Count; i++)
			{
				if (_hasMMHiddenProperties && (!_mmHiddenPropertiesToHide.Contains(PropertiesList[i].name)))
				{
					if (!DrawCustomInspectors(PropertiesList[i], feedback))
					{
						EditorGUILayout.PropertyField(PropertiesList[i], true);    
					}
				}
			}
		}

		protected Rect _leftBorderRect = new Rect();
		protected Rect _setupRect = new Rect();
		protected Rect _verticalGroup = new Rect();
		protected Rect _widthRect = new Rect();
		protected GUIContent _groupTitle = new GUIContent();
        
		protected virtual void DrawGroup(PLFInspectorGroupData groupData, PLF_Feedback feedback)
		{
			_verticalGroup = EditorGUILayout.BeginVertical();
            
			// we draw a colored line on the left
			_leftBorderRect.x = _verticalGroup.xMin + 5;
			_leftBorderRect.y = _verticalGroup.yMin - 0;
			_leftBorderRect.width = 3f;
			_leftBorderRect.height = _verticalGroup.height + 0;
			_leftBorderRect.xMin = 15f;
			_leftBorderRect.xMax = 18f;
			EditorGUI.DrawRect(_leftBorderRect, groupData.GroupColor);

			if (groupData.GroupAttribute.RequiresSetup && feedback.RequiresSetup)
			{
				// we draw a warning sign if needed
				_widthRect = EditorGUILayout.GetControlRect(false, 0);
				float setupRectWidth = 20f;
				_setupRect.x = _widthRect.xMax - setupRectWidth;
				_setupRect.y = _verticalGroup.yMin;
				_setupRect.width = setupRectWidth;
				_setupRect.height = 17f;
                
				EditorGUI.LabelField(_setupRect, PLF_PlayerStyling._setupRequiredIcon);
			}

			groupData.GroupIsOpen = EditorGUILayout.Foldout(groupData.GroupIsOpen, groupData.GroupAttribute.GroupName, true, PLF_FeedbackInspectorStyle.GroupStyle);

			if (groupData.GroupIsOpen)
			{
				EditorGUI.indentLevel = 0;

				for (int i = 0; i < groupData.PropertiesList.Count; i++)
				{
					DrawVerticalLayout(() => DrawChild(i, feedback), PLF_FeedbackInspectorStyle.BoxChildStyle);
				}
			}

			EditorGUILayout.EndVertical();

			void DrawChild(int i, PLF_Feedback feedbackDrawn)
			{
				if (i > groupData.PropertiesList.Count - 1)
				{
					return;
				}
	            
				if ((_hasMMHiddenProperties) && (_mmHiddenPropertiesToHide.Contains(groupData.PropertiesList[i].name)))
				{
					return;
				}

				if (!feedback.HasChannel 
				    && (groupData.PropertiesList[i].name == _channelFieldName
				        || groupData.PropertiesList[i].name == _channelModeFieldName
				        || groupData.PropertiesList[i].name == _channelDefinitionFieldName))
				{
					return;
				}

				_groupTitle.text = ObjectNames.NicifyVariableName(groupData.PropertiesList[i].name);
				_groupTitle.tooltip = groupData.PropertiesList[i].tooltip;
                
				if (!DrawCustomInspectors(groupData.PropertiesList[i], feedback))
				{
					bool shouldDraw = !((groupData.PropertiesList[i].name == _automatedTargetAcquisitionName) &&
					                    (!feedbackDrawn.HasAutomatedTargetAcquisition));
					if (shouldDraw)
					{
						EditorGUILayout.PropertyField(groupData.PropertiesList[i], _groupTitle, true);	
					}
				}
			}
		}

		public static void DrawVerticalLayout(Action action, GUIStyle style)
		{
			EditorGUILayout.BeginVertical(style);
			action();
			EditorGUILayout.EndVertical();
		}
        
		public void DrawNoGroupInspector(SerializedProperty currentProperty, PLF_Feedback feedback)
		{
			SerializedProperty endProp = currentProperty.GetEndProperty();

			while (currentProperty.NextVisible(true) && !EqualContents(endProp, currentProperty))
			{
				if (currentProperty.depth <= 2)
				{
					if (!DrawCustomInspectors(currentProperty, feedback))
					{
						EditorGUILayout.PropertyField(currentProperty, true);    
					}
				}
			}
		}
		
		protected GUIContent _tweenCurveGUIContent = new GUIContent("PL Tween Curve");
		protected GUIContent _animationCurveGUIContent = new GUIContent("Animation Curve");
		protected const string _customInspectorButtonPropertyName = "PLF_Button";
		protected const string _customTweenTypePropertyName = "PLTweenType";
		protected const string _findPropertyRelativeMMTweenDefinitionType = "PLTweenDefinitionType";
		protected const string _mmTweenCurvePropertyName = "PLTweenCurve";
		protected const string _curvePropertyName = "Curve";
		protected SerializedProperty _mmTweenTypeProperty;
		protected PLFConditionAttribute _conditionAttribute;
		protected PLFEnumConditionAttribute _enumConditionAttribute;

		private bool DrawCustomInspectors(SerializedProperty currentProperty, PLF_Feedback feedback)
		{
			if (feedback.HasCustomInspectors)
			{
				switch (currentProperty.type)
				{
					case _customInspectorButtonPropertyName:
						PLF_Button myButton = (PLF_Button)(currentProperty.PLFGetObjectValue());
						if (GUILayout.Button(myButton.ButtonText))
						{
							myButton.TargetMethod();
						}
						return true;
					case _customTweenTypePropertyName: 
						// if we're displaying a tween type, we need to handle conditions manually

						_animationCurveGUIContent.tooltip = currentProperty.tooltip;
						_tweenCurveGUIContent.tooltip = currentProperty.tooltip;
						
						_mmTweenTypeProperty = currentProperty.FindPropertyRelative(_findPropertyRelativeMMTweenDefinitionType);
						if (_conditionDictionary.TryGetValue(currentProperty.name, out _conditionAttribute))
						{
							string propertyPath = currentProperty.propertyPath;
							string conditionPath = propertyPath.Replace(currentProperty.name, _conditionAttribute.ConditionBoolean);
							SerializedProperty sourcePropertyValue = currentProperty.serializedObject.FindProperty(conditionPath);
							if (!_conditionAttribute.Negative && !sourcePropertyValue.boolValue) 
							{
								return true;
							}
							if (_conditionAttribute.Negative && sourcePropertyValue.boolValue)
							{
								return true;
							}
						}
						
						if (_enumConditionDictionary.TryGetValue(currentProperty.name, out _enumConditionAttribute))
						{
							string propertyPath = currentProperty.propertyPath;
							string conditionPath = propertyPath.Replace(currentProperty.name, _enumConditionAttribute.ConditionEnum);
							SerializedProperty sourcePropertyValue = currentProperty.serializedObject.FindProperty(conditionPath);

							if ((sourcePropertyValue != null) && (sourcePropertyValue.propertyType == SerializedPropertyType.Enum))
							{
								int currentEnum = sourcePropertyValue.enumValueIndex;
								if (!_enumConditionAttribute.ContainsBitFlag(currentEnum))
								{
									return true;
								}
							}
						}
						
						EditorGUILayout.PropertyField(_mmTweenTypeProperty, new GUIContent(currentProperty.displayName, currentProperty.tooltip));
						if (_mmTweenTypeProperty.enumValueIndex == 0)
						{
							EditorGUILayout.PropertyField(currentProperty.FindPropertyRelative(_mmTweenCurvePropertyName), _tweenCurveGUIContent);
						}
						if (_mmTweenTypeProperty.enumValueIndex == 1)
						{
							EditorGUILayout.PropertyField(currentProperty.FindPropertyRelative(_curvePropertyName), _animationCurveGUIContent);
						}
						return true;
				}
			}

			return false;
		}

		private bool EqualContents(SerializedProperty a, SerializedProperty b)
		{
			return SerializedProperty.EqualContents(a, b);
		}
	}
}