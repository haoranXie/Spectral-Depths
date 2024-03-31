using System.Collections;
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EmeraldAI.Utility
{
    [CustomEditor(typeof(EmeraldWeaponCollision))]
    [CanEditMultipleObjects]
    public class EmeraldWeaponCollisionEditor : Editor
    {
        GUIStyle FoldoutStyle;
        Texture WeaponCollisionEditorIcon;
        SerializedProperty CollisionBoxColor, HideSettingsFoldout, WeaponCollisionFoldout, WeaponName;

        void OnEnable()
        {
            if (WeaponCollisionEditorIcon == null) WeaponCollisionEditorIcon = Resources.Load("Editor Icons/EmeraldWeaponCollision") as Texture;
            EmeraldWeaponCollision self = (EmeraldWeaponCollision)target;
            self.WeaponCollider = self.GetComponent<BoxCollider>();
            CollisionBoxColor = serializedObject.FindProperty("CollisionBoxColor");
            WeaponName = serializedObject.FindProperty("WeaponName");
            HideSettingsFoldout = serializedObject.FindProperty("HideSettingsFoldout");
            WeaponCollisionFoldout = serializedObject.FindProperty("WeaponCollisionFoldout");
        }

        public override void OnInspectorGUI()
        {
            FoldoutStyle = CustomEditorProperties.UpdateEditorStyles();
            serializedObject.Update();

            CustomEditorProperties.BeginScriptHeaderNew("Weapon Collision", WeaponCollisionEditorIcon, new GUIContent(), HideSettingsFoldout);

            if (!HideSettingsFoldout.boolValue)
            {
                EditorGUILayout.Space();
                WeaponCollisionSettings();
                EditorGUILayout.Space();
            }

            CustomEditorProperties.EndScriptHeader();

            
            serializedObject.ApplyModifiedProperties();
        }

        void WeaponCollisionSettings ()
        {          
            WeaponCollisionFoldout.boolValue = EditorGUILayout.Foldout(WeaponCollisionFoldout.boolValue, "Weapon Collision Settings", true, FoldoutStyle);

            if (WeaponCollisionFoldout.boolValue)
            {
                CustomEditorProperties.BeginFoldoutWindowBox();
                CustomEditorProperties.TextTitleWithDescription("Weapon Collision Settings", "Adjust the size of the Box Collider (using the Box Collider component) to position the collider to an AI's weapon. The Weapon Collision is intended for melee related attacks. In order for " +
                    "the Weapon Collision component to work, it needs to be enabled through an Animation Event.", true);

                EditorGUILayout.PropertyField(CollisionBoxColor, new GUIContent("Collision Box Color"));
                CustomEditorProperties.CustomHelpLabelField("Controls the color of the Collision Box.", true);

                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(WeaponName, new GUIContent("WeaponName"));
                CustomEditorProperties.CustomHelpLabelField("Weapons are turned or turned off depending on their weapon name", true);

                EditorGUILayout.Space();
                CustomEditorProperties.EndFoldoutWindowBox();
            }
        }
    }
}
#endif