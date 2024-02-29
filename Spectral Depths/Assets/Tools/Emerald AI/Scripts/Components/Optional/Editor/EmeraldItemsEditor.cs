﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace EmeraldAI.Utility
{
    //TODO: Any component that relies on another component, such as this and EmeraldCombat, it can be a RequiredComponent here.
    [CustomEditor(typeof(EmeraldItems))]
    [CanEditMultipleObjects]
    public class EmeraldItemsEditor : Editor
    {
        GUIStyle FoldoutStyle;
        EmeraldSystem EmeraldComp;
        EmeraldAnimation EmeraldAnimation;
        EmeraldCombat EmeraldCombat;
        Texture ItemsEditorIcon;

        #region SerializedProperties
        //Objects
        SerializedProperty HeldMeleeWeaponObjectProp, HolsteredMeleeWeaponObjectProp, HeldRangedWeaponObjectProp, HolsteredRangedWeaponObjectProp;

        //Bools
        SerializedProperty HideSettingsFoldout, WeaponsFoldout, ItemsFoldout;

        //Enum
        SerializedProperty UseDroppableWeaponProp;

        ReorderableList ItemList, Type1EquippableWeaponsList;
        #endregion

        void OnEnable()
        {
            EmeraldItems self = (EmeraldItems)target;
            EmeraldComp = self.GetComponent<EmeraldSystem>();
            EmeraldAnimation = self.GetComponent<EmeraldAnimation>();
            EmeraldCombat = self.GetComponent<EmeraldCombat>();
            if (ItemsEditorIcon == null) ItemsEditorIcon = Resources.Load("Editor Icons/EmeraldItems") as Texture;

            InitializeProperties();
            InitializeLists ();
        }

        void InitializeProperties()
        {
            //Objects
            HeldMeleeWeaponObjectProp = serializedObject.FindProperty("HeldMeleeWeaponObject");
            HolsteredMeleeWeaponObjectProp = serializedObject.FindProperty("HolsteredMeleeWeaponObject");
            HeldRangedWeaponObjectProp = serializedObject.FindProperty("HeldRangedWeaponObject");
            HolsteredRangedWeaponObjectProp = serializedObject.FindProperty("HolsteredRangedWeaponObject");

            //Bools
            HideSettingsFoldout = serializedObject.FindProperty("HideSettingsFoldout");
            WeaponsFoldout = serializedObject.FindProperty("WeaponsFoldout");
            ItemsFoldout = serializedObject.FindProperty("ItemsFoldout");

            //Enum
            UseDroppableWeaponProp = serializedObject.FindProperty("UseDroppableWeapon");
        }

        void InitializeLists ()
        {
            //Type 1 Equippables
            Type1EquippableWeaponsList = new ReorderableList(serializedObject, serializedObject.FindProperty("Type1EquippableWeapons"), true, true, true, true);
            Type1EquippableWeaponsList.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    EditorGUI.BeginChangeCheck();
                    var element = Type1EquippableWeaponsList.serializedProperty.GetArrayElementAtIndex(index);

                    EditorGUI.LabelField(new Rect(rect.x + 10, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Item " + (index + 1).ToString());

                    float Height = (EditorGUIUtility.singleLineHeight * 1.25f);

                    //EditorGUI.PropertyField(new Rect(rect.x - 12.5f, rect.y + Height, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("HeldToggle"), GUIContent.none); //Toggle
                    EditorGUI.LabelField(new Rect(rect.x + 10, rect.y + Height, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Held", "The Held object is used to reference the position for spawning droppable weapons and for equipping items.")); //Title
                    //EditorGUI.BeginDisabledGroup(!element.FindPropertyRelative("HeldToggle").boolValue);
                    EditorGUI.PropertyField(new Rect(rect.x + 100, rect.y + Height, rect.width - 100, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("HeldObject"), GUIContent.none); //Object
                    //EditorGUI.EndDisabledGroup();

                    EditorGUI.PropertyField(new Rect(rect.x - 12.5f, rect.y + Height * 2, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("HolsteredToggle"), GUIContent.none); //Toggle
                    EditorGUI.LabelField(new Rect(rect.x + 10, rect.y + Height * 2, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Holstered", "The weapon object that's holstered on an AI. This is used when using Equipping Animations." +
                        "\n\nNote: If an Animation Profile does not have any Equipping Animations, this setting will be ignored.")); //Title
                    EditorGUI.BeginDisabledGroup(!element.FindPropertyRelative("HolsteredToggle").boolValue);
                    EditorGUI.PropertyField(new Rect(rect.x + 100, rect.y + Height * 2, rect.width - 100, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("HolsteredObject"), GUIContent.none); //Object
                    EditorGUI.EndDisabledGroup();

                    EditorGUI.PropertyField(new Rect(rect.x - 12.5f, rect.y + Height * 3, rect.width, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("DroppableToggle"), GUIContent.none); //Toggle
                    EditorGUI.LabelField(new Rect(rect.x + 10, rect.y + Height * 3, rect.width, EditorGUIUtility.singleLineHeight), new GUIContent("Droppable", "If enabled, this will spawn a copy of the attached object to the exact position and rotation of this item's Held object.")); //Title
                    EditorGUI.BeginDisabledGroup(!element.FindPropertyRelative("DroppableToggle").boolValue);
                    EditorGUI.PropertyField(new Rect(rect.x + 100, rect.y + Height * 3, rect.width - 100, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("DroppableObject"), GUIContent.none); //Object
                    EditorGUI.EndDisabledGroup();
                };

            Type1EquippableWeaponsList.elementHeightCallback = (int index) =>
            {
                SerializedProperty element = Type1EquippableWeaponsList.serializedProperty.GetArrayElementAtIndex(index);
                return EditorGUIUtility.singleLineHeight * 5.0f;
            };

            Type1EquippableWeaponsList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Type 1 Equippable Weapons", EditorStyles.boldLabel);
            };

            //Item Objects
            ItemList = new ReorderableList(serializedObject, serializedObject.FindProperty("ItemList"), true, true, true, true);
            ItemList.drawElementCallback =
                (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    EditorGUI.BeginChangeCheck();
                    var element = ItemList.serializedProperty.GetArrayElementAtIndex(index);
                    EditorGUI.PropertyField(new Rect(rect.x + 60, rect.y, rect.width - 120, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("ItemObject"), GUIContent.none);
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, 50, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("ItemID"), GUIContent.none);
                };

            ItemList.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "   ID  " + "         Item Object", EditorStyles.boldLabel);
            };
        }

        public override void OnInspectorGUI()
        {
            FoldoutStyle = CustomEditorProperties.UpdateEditorStyles();
            EmeraldItems self = (EmeraldItems)target;
            serializedObject.Update();

            CustomEditorProperties.BeginScriptHeaderNew("Items", ItemsEditorIcon, new GUIContent(), HideSettingsFoldout);

            if (!HideSettingsFoldout.boolValue)
            {
                EditorGUILayout.Space();
                WeaponSettings(self);
                EditorGUILayout.Space();
                ItemSettings(self);
                EditorGUILayout.Space();
            }

            serializedObject.ApplyModifiedProperties();
            CustomEditorProperties.EndScriptHeader();
        }

        void WeaponSettings(EmeraldItems self)
        {
            WeaponsFoldout.boolValue = EditorGUILayout.Foldout(WeaponsFoldout.boolValue, "Weapon Settings", true, FoldoutStyle);

            if (WeaponsFoldout.boolValue)
            {
                CustomEditorProperties.BeginFoldoutWindowBox();

                CustomEditorProperties.TextTitleWithDescription("Weapon Settings", "Allows AI to enable and disable weapon objects using Animation Events (Requires AI to have Equip and Unequip Animations).", true);

                Type1EquippableWeaponsList.DoLayoutList();

                /*
                EditorGUILayout.PropertyField(UseDroppableWeaponProp, new GUIContent("Use Droppable Weapon"));
                CustomEditorProperties.CustomHelpLabelField("Controls whether or not this AI will drop its active weapon object when it dies.", true);

                if (self.UseDroppableWeapon == YesOrNo.Yes && EmeraldAnimation.UseEquipAnimations == YesOrNo.No)
                {
                    CustomEditorProperties.BeginIndent();

                    if (EmeraldCombat.WeaponTypeAmount == EmeraldCombat.WeaponTypeAmounts.One || EmeraldCombat.WeaponTypeAmount == EmeraldCombat.WeaponTypeAmounts.Two)
                    {
                        CustomEditorProperties.CustomObjectField(new Rect(), new GUIContent(), HeldMeleeWeaponObjectProp, "Held Type 1 Weapon Object", typeof(GameObject), false);
                        CustomEditorProperties.CustomHelpLabelField("The melee weapon model that the AI uses that is present in their hand. When the AI dies, this model will be copied, unparented, and dropped.", false);
                    }

                    if (EmeraldCombat.WeaponTypeAmount == EmeraldCombat.WeaponTypeAmounts.Two)
                    {
                        CustomEditorProperties.CustomObjectField(new Rect(), new GUIContent(), HeldRangedWeaponObjectProp, "Held Type 2 Weapon Object", typeof(GameObject), false);
                        CustomEditorProperties.CustomHelpLabelField("The ranged weapon model that the AI uses that is present in their hand. When the AI dies, this model will be copied, unparented, and dropped.", true);
                    }

                    CustomEditorProperties.EndIndent();
                }
                else if (EmeraldAnimation.UseEquipAnimations == YesOrNo.Yes)
                {
                    EditorGUILayout.Space();

                    CustomEditorProperties.CustomObjectField(new Rect(), new GUIContent(), HeldMeleeWeaponObjectProp, "Held Type 1 Weapon Object", typeof(GameObject), false);
                    CustomEditorProperties.CustomHelpLabelField("The melee weapon model that the AI uses that is present in their hand. If Use Droppable Weapon is enabled, this model will be copied, unparented, " +
                        "and dropped when the AI dies, given that the AI is using its melee weapon.", false);

                    CustomEditorProperties.CustomObjectField(new Rect(), new GUIContent(), HolsteredMeleeWeaponObjectProp, "Sheathed Type 1 Weapon Object", typeof(GameObject), false);
                    CustomEditorProperties.CustomHelpLabelField("The melee weapon model that the AI has sheathed. This object acts as the AI's melee weapon that's 'put away'.", true);

                    if (EmeraldCombat.WeaponTypeAmount == EmeraldCombat.WeaponTypeAmounts.Two)
                    {
                        CustomEditorProperties.CustomObjectField(new Rect(), new GUIContent(), HeldRangedWeaponObjectProp, "Held Type 2 Weapon Object", typeof(GameObject), false);
                        CustomEditorProperties.CustomHelpLabelField("The ranged weapon model that the AI uses that is present in their hand. If Use Droppable Weapon is enabled, this model will be copied, unparented, " +
                            "and dropped when the AI dies, given that the AI is using its ranged weapon.", false);

                        CustomEditorProperties.CustomObjectField(new Rect(), new GUIContent(), HolsteredRangedWeaponObjectProp, "Holstered Type 2 Weapon Object", typeof(GameObject), false);
                        CustomEditorProperties.CustomHelpLabelField("The ranged weapon model that the AI has hosltered. This object acts as the AI's ranged weapon that's 'put away'.", true);
                    }
                }
                */
                EditorGUILayout.Space();
                CustomEditorProperties.EndFoldoutWindowBox();
            }
        }

        void ItemSettings(EmeraldItems self)
        {
            ItemsFoldout.boolValue = EditorGUILayout.Foldout(ItemsFoldout.boolValue, "Item Settings", true, FoldoutStyle);

            if (ItemsFoldout.boolValue)
            {
                CustomEditorProperties.BeginFoldoutWindowBox();

                CustomEditorProperties.TextTitleWithDescription("Item Settings", "Objects that are attached to your AI that can be enabled or disable through Animation Events or programmatically " +
                "using the item ID. This can be useful for quests items, animation effects, animation specific items, etc. For more information regarding this, please see the Documentation.", true);

                CustomEditorProperties.CustomHelpLabelField("Each Item below has an ID number. This ID is used to find that particular item and to either enable or disable it using Emerald AI's API.", true);
                ItemList.DoLayoutList();

                CustomEditorProperties.EndFoldoutWindowBox();
            }
        }
    }
}