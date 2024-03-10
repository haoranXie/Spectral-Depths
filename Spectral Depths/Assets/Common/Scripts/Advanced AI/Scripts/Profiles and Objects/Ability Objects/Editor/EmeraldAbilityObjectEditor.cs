using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace EmeraldAI.Utility
{
    [CustomEditor(typeof(EmeraldAbilityObject), true)]
    [CanEditMultipleObjects]
    public class EmeraldAbilityObjectEditor : Editor
    {
        GUIStyle FoldoutStyle;
        FieldInfo[] CustomFields;
        SerializedProperty AbilityName, AbilityIcon, DerivedSettingsFoldout, InfoSettingsFoldout, HideSettingsFoldout, ModularSettingsFoldout, CooldownSettings;

        SerializedProperty MeleeSettings, ProjectileSettings, ArrowProjectileSettings, GeneralProjectileSettings, BulletProjectileSettings, AerialProjectileSettings, GroundProjectileSettings, BarrageProjectileSettings, TeleportSettings, HomingSettings, TargetTypeSettings, 
            SpreadSettings, ColliderSettings, CreateSettings, ChargeSettings, AreaOfEffectSettings, DamageSettings, StunnedSettings;
        string ChargeSettingsTooltip = "Allows the ability to play an effect and/or sound when it is charging. The location of this effect is determined by the Attack Transform " +
            "name passed through the ChargeEffect animation event. In order to use this setting, it must be set to true.\n\nNote: An ChargeEffect animation event is required for this to trigger. This should be done through the AI's attack animations before the EmeraldAttack animation event is called.";
        string CreateSettingsTooltip = "Allows the ability to play an effect and/or sound when it is created. The location of this effect is determined by the Attack Transform " +
            "name passed through the CreateAbility. In order to use this setting, it must be set to true.";
        string MeleeSettingsTooltip = "Allows an ability to deal damage within the specified angle and distance.\n\nNote: " +
            "If your melee attack animation is using Weapon Collision Events, the angle and distance settings will be ignored and this ability will rely on a successful collision from the weapon instead.";
        string ProjectileSettingsTooltip = "Controls the main effects used for this ability.\n\nNote: Settings can be shared between other Projectile Effect Modules by right clicking and copying this module and pasting it elsewhere. " +
            "The Projectile Effect slot is required, any other settings that are left empty will be ignored when the ability is used.";
        string GeneralProjectileSettingsTooltip = "Allows projectiles to move towards the specified target. Can be used for various spells or even rockets.";
        string BulletProjectileSettingsTooltip = "Allows projectiles, like bullets, to move very quickly towards the specified target.";
        string GroundProjectileSettingsTooltip = "Allows projectiles to align themselves with and travel along the ground.\n\nNote: This setting relies on this ability's Projectile Settings.";
        string TeleportSettingsTooltip = "Allows the ability to teleport the owner within the radius of the specified target.";
        string HomingSettingsTooltip = "Allows projectiles to home towards their Target Source.";
        string AerialProjectileSettingsTooltip = "Allows projectiles to spawn from above the creator or Target Source within a customizable radius.\n\nNote: This setting relies on this ability's Projectile Settings.";
        string TargetTypeSettingsTooltip = "Controls the source of this ability's intended target.";
        //string BranchSettingsTooltip = "Allows projectiles the chance to branch to other nearby targets after they have collided with the ability's Target Source. The effect for this is based on this ability's Projectile Module.";
        string SpreadSettingsTooltip = "Allows projectiles to spread in the X and Y directions from the their spawn source.";
        string ColliderSettingsTooltip = "Allows projectiles to collide with objects and targets by automatically adding a Sphere Collider to the Projectile Effect." +
            "\n\nNote: If a collider exists on Projectile Effect, the Sphere Collider will not be generated and the existing collider will be used instead.";
        string AreaOfEffectSettingsTooltip = "Allows an ability to affect the specified area with";
        string StunSettingsTooltip = "Allows abilities the chance to stun a successfully hit target.";
        string DamageSettingsTooltip = "Allows abilities to deal damage to a successfully hit target.";


        void OnEnable()
        {
            EmeraldAbilityObject self = (EmeraldAbilityObject)target;
            if (self.AbilityIcon == null) self.AbilityIcon = Resources.Load("Editor Icons/EmeraldAbility") as Texture2D; //Load the default icon if the AbilityIcon is null
            DerivedSettingsFoldout = serializedObject.FindProperty("DerivedSettingsFoldout");
            HideSettingsFoldout = serializedObject.FindProperty("HideSettingsFoldout");
            InfoSettingsFoldout = serializedObject.FindProperty("InfoSettingsFoldout");
            ModularSettingsFoldout = serializedObject.FindProperty("ModularSettingsFoldout");
            AbilityName = serializedObject.FindProperty("AbilityName");
            AbilityIcon = serializedObject.FindProperty("AbilityIcon");
            CooldownSettings = serializedObject.FindProperty("CooldownSettings");
            MeleeSettings = serializedObject.FindProperty("MeleeSettings");
            ProjectileSettings = serializedObject.FindProperty("ProjectileSettings");
            BulletProjectileSettings = serializedObject.FindProperty("BulletProjectileSettings");
            GeneralProjectileSettings = serializedObject.FindProperty("GeneralProjectileSettings");
            ArrowProjectileSettings = serializedObject.FindProperty("ArrowProjectileSettings");
            AerialProjectileSettings = serializedObject.FindProperty("AerialProjectileSettings");
            GroundProjectileSettings = serializedObject.FindProperty("GroundProjectileSettings");
            BarrageProjectileSettings = serializedObject.FindProperty("BarrageProjectileSettings");
            TeleportSettings = serializedObject.FindProperty("TeleportSettings");
            HomingSettings = serializedObject.FindProperty("HomingSettings");
            TargetTypeSettings = serializedObject.FindProperty("TargetTypeSettings");
            SpreadSettings = serializedObject.FindProperty("SpreadSettings");
            ColliderSettings = serializedObject.FindProperty("ColliderSettings");
            CreateSettings = serializedObject.FindProperty("CreateSettings");
            ChargeSettings = serializedObject.FindProperty("ChargeSettings");
            AreaOfEffectSettings = serializedObject.FindProperty("AreaOfEffectSettings");
            DamageSettings = serializedObject.FindProperty("DamageSettings");
            StunnedSettings = serializedObject.FindProperty("StunnedSettings");

            //Get all variables that are not part of the parent class.
            CustomFields = target.GetType().GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
        }

        public override void OnInspectorGUI()
        {
            EmeraldAbilityObject self = (EmeraldAbilityObject)target;
            FoldoutStyle = CustomEditorProperties.UpdateEditorStyles();
            serializedObject.Update();
            CustomEditorProperties.BeginScriptHeaderNew(self.AbilityName, self.AbilityIcon, new GUIContent(), HideSettingsFoldout, false);

            EditorGUILayout.Space();
            InfoSettings(self);
            EditorGUILayout.Space();
            DerivedSettings(self);
            EditorGUILayout.Space();

            CustomEditorProperties.EndScriptHeader();

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                Undo.RecordObject(self, "Undo");

                if (GUI.changed)
                {
                    EditorUtility.SetDirty(target);
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
                }
            }
#endif

            serializedObject.ApplyModifiedProperties();
        }

        void InfoSettings(EmeraldAbilityObject self)
        {
            InfoSettingsFoldout.boolValue = EditorGUILayout.Foldout(InfoSettingsFoldout.boolValue, "Info Settings", true, FoldoutStyle);

            if (InfoSettingsFoldout.boolValue)
            {
                CustomEditorProperties.BeginFoldoutWindowBox();
                CustomEditorProperties.TextTitleWithDescription("Info Settings", "Customize the name, description, and icon of an ability.", true);

                EditorGUILayout.PropertyField(AbilityName);
                CustomEditorProperties.CustomHelpLabelField("The name of this Ability.", true);

                self.AbilityDescription = CustomEditorProperties.CustomDescriptionField(self, "Ability Description", self.AbilityDescription);
                CustomEditorProperties.CustomHelpLabelField("The description of this Ability.", true);

                EditorGUILayout.PropertyField(AbilityIcon);
                CustomEditorProperties.CustomHelpLabelField("The icon for this Ability. If this field is empty, the default icon will be used.", true);

                EditorGUILayout.Space();
                CustomEditorProperties.EndFoldoutWindowBox();
            }
        }

        /// <summary>
        /// Displays all custom variables in a separate part of the editor.
        /// </summary>
        void DerivedSettings(EmeraldAbilityObject self)
        {
            DerivedSettingsFoldout.boolValue = EditorGUILayout.Foldout(DerivedSettingsFoldout.boolValue, self.AbilityName + " Settings", true, FoldoutStyle);

            if (DerivedSettingsFoldout.boolValue)
            {
                CustomEditorProperties.BeginFoldoutWindowBox();
                CustomEditorProperties.TextTitleWithDescription(self.AbilityName + " Settings", self.AbilityDescription, true);

                if (ChargeSettings != null) DrawModule(ChargeSettings, "Charge Module", ChargeSettingsTooltip);
                if (CreateSettings != null) DrawModule(CreateSettings, "Create Module", CreateSettingsTooltip);

                if (CooldownSettings != null) DrawModule(CooldownSettings, "Cooldown Module", "", false);

                if (MeleeSettings != null) DrawModule(MeleeSettings, "Melee Module", MeleeSettingsTooltip, true);

                if (ColliderSettings != null) DrawModule(ColliderSettings, "Collider Module", ColliderSettingsTooltip, true);
                if (TargetTypeSettings != null) DrawModule(TargetTypeSettings, "Target Type Module", TargetTypeSettingsTooltip, true);
                if (ProjectileSettings != null) DrawModule(ProjectileSettings, "Projectile Effects Module", ProjectileSettingsTooltip, true);
                if (GeneralProjectileSettings != null) DrawModule(GeneralProjectileSettings, "General Projectile Module", GeneralProjectileSettingsTooltip, true);
                if (BulletProjectileSettings != null) DrawModule(BulletProjectileSettings, "Bullet Projectile Module", BulletProjectileSettingsTooltip, true);
                if (ArrowProjectileSettings != null) DrawModule(ArrowProjectileSettings, "Arrow Projectile Module", "", true);
                if (AerialProjectileSettings != null) DrawModule(AerialProjectileSettings, "Aerial Projectile Module", AerialProjectileSettingsTooltip, true);
                if (GroundProjectileSettings != null) DrawModule(GroundProjectileSettings, "Ground Projectile Module", GroundProjectileSettingsTooltip, true);
                if (BarrageProjectileSettings != null) DrawModule(BarrageProjectileSettings, "Barrage Projectile Module", "", true);
                if (TeleportSettings != null) DrawModule(TeleportSettings, "Teleport Module", TeleportSettingsTooltip, true);
                if (AreaOfEffectSettings != null) DrawModule(AreaOfEffectSettings, "Area of Effect Module", AreaOfEffectSettingsTooltip, true);

                if (HomingSettings != null) DrawModule(HomingSettings, "Homing Module", HomingSettingsTooltip);
                //if (BranchSettings != null) DrawModule(BranchSettings, "Branch Module", BranchSettingsTooltip);
                if (SpreadSettings != null) DrawModule(SpreadSettings, "Spread Module", SpreadSettingsTooltip);
                if (DamageSettings != null) DrawDamageModule(DamageSettings, "Damage Module", DamageSettingsTooltip);
                if (StunnedSettings != null) DrawModule(StunnedSettings, "Stunned Module", StunSettingsTooltip);
                

                foreach (FieldInfo field in CustomFields)
                {
                    //Get all fields so they can be drawn in the Ability Object Editor's style,
                    //but exclude any classes derived from EmeraldAI.AbilityData as they are handled elsewhere.
                    var TypeInfo = field.FieldType.GetTypeInfo();
                    string Namespace = TypeInfo.Namespace;
                    var DeclaringType = TypeInfo.DeclaringType;
                    string ClassInfo = "";

                    if (DeclaringType != null)
                    {
                        ClassInfo = DeclaringType.ToString();
                    }

                    //Offset Arrays with extra space
                    if (field.FieldType.GetElementType() != null)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(15);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty(field.Name));
                        GUILayout.Space(1);
                        EditorGUILayout.EndHorizontal();
                    }
                    //Offset Lists with extra space
                    else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(15);
                        EditorGUILayout.PropertyField(serializedObject.FindProperty(field.Name));
                        GUILayout.Space(1);
                        EditorGUILayout.EndHorizontal();
                    }
                    else if (field.FieldType.IsClass && Namespace != "UnityEngine" && Namespace != "System" && ClassInfo != "EmeraldAI.AbilityData")
                    {
                        CustomEditorProperties.BeginFoldoutWindowBox();

                        if (serializedObject.FindProperty(field.Name).FindPropertyRelative("Enabled") == null)
                        {
                            GUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUI.BeginDisabledGroup(true);
                                EditorGUILayout.Toggle(true, GUILayout.Width(28));
                                EditorGUI.EndDisabledGroup();
                                EditorGUILayout.PropertyField(serializedObject.FindProperty(field.Name));
                                EditorGUILayout.EndHorizontal();
                            }
                            GUILayout.Space(5);
                            GUILayout.EndHorizontal();
                        }
                        else
                        {
                            GUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.BeginHorizontal();
                                var Style = new GUIStyle(EditorStyles.radioButton);
                                serializedObject.FindProperty(field.Name).FindPropertyRelative("Enabled").boolValue = EditorGUILayout.Toggle(serializedObject.FindProperty(field.Name).FindPropertyRelative("Enabled").boolValue, GUILayout.Width(28));
                                EditorGUILayout.PropertyField(serializedObject.FindProperty(field.Name));
                                EditorGUILayout.EndHorizontal();

                            }
                            GUILayout.Space(5);
                            GUILayout.EndHorizontal();
                        }

                        GUILayout.Space(2.5f);
                        CustomEditorProperties.EndFoldoutWindowBox();
                    }
                    //Don't apply an offset to single variables
                    else
                    {
                        if (ClassInfo != "EmeraldAI.AbilityData")
                            EditorGUILayout.PropertyField(serializedObject.FindProperty(field.Name));
                    }
                    GUILayout.Space(2.5f);
                }
                EditorGUILayout.Space();
                CustomEditorProperties.EndFoldoutWindowBox();
            }
        }

        /// <summary>
        /// Draws the passed property (dervied from the AnilityData class) as a module foldout.
        /// </summary>
        void DrawModule (SerializedProperty property, string Name, string Tooltip, bool Required = false)
        {
            CustomEditorProperties.BeginFoldoutWindowBox();
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginHorizontal();
                if (!Required)
                {
                    property.FindPropertyRelative("Enabled").boolValue = EditorGUILayout.Toggle(property.FindPropertyRelative("Enabled").boolValue, GUILayout.Width(28));
                    EditorGUILayout.PropertyField(property, new GUIContent(Name, "(Optional) "+ Tooltip));
                }
                else if (Required)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.Toggle(true, GUILayout.Width(28));
                    property.FindPropertyRelative("Enabled").boolValue = true;
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.PropertyField(property, new GUIContent(Name, "(Required) " + Tooltip));
                }
                
                EditorGUILayout.EndHorizontal();

            }
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            GUILayout.Space(2.5f);
            CustomEditorProperties.EndFoldoutWindowBox();
            GUILayout.Space(2.5f);
        }

        /// <summary>
        /// Draws the passed property (dervied from the AnilityData class) as a module foldout.
        /// </summary>
        void DrawDamageModule(SerializedProperty property, string Name, string Tooltip, bool Required = false)
        {
            CustomEditorProperties.BeginFoldoutWindowBox();
            GUILayout.BeginHorizontal();
            {
                EditorGUILayout.BeginHorizontal();
                if (!Required)
                {
                    property.FindPropertyRelative("Enabled").boolValue = EditorGUILayout.Toggle(property.FindPropertyRelative("Enabled").boolValue, GUILayout.Width(28));
                    property.FindPropertyRelative("Foldout").boolValue = EditorGUILayout.Foldout(property.FindPropertyRelative("Foldout").boolValue, new GUIContent(Name, "(Optional) " + Tooltip), true);
                }
                else if (Required)
                {
                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.Toggle(true, GUILayout.Width(28));
                    property.FindPropertyRelative("Enabled").boolValue = true;
                    EditorGUI.EndDisabledGroup();
                    EditorGUILayout.PropertyField(property, new GUIContent(Name, "(Required) " + Tooltip));
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.Space(5);
            GUILayout.EndHorizontal();

            if (property.FindPropertyRelative("Foldout").boolValue)
            {
                //Base Damage
                GUILayout.Space(5);
                CustomEditorProperties.BeginIndent(45);
                EditorGUILayout.PropertyField(property.FindPropertyRelative("BaseDamageSettings"));
                CustomEditorProperties.EndIndent();
                //Base Damage

                //Critical Hits
                CustomEditorProperties.BeginIndent(45);
                EditorGUILayout.PropertyField(property.FindPropertyRelative("UseCriticalHits"));
                EditorGUI.BeginDisabledGroup(!property.FindPropertyRelative("UseCriticalHits").boolValue);
                CustomEditorProperties.BeginFoldoutWindowBox();
                CustomEditorProperties.BeginIndent(15);
                EditorGUILayout.PropertyField(property.FindPropertyRelative("CriticalHitSettings"));
                CustomEditorProperties.EndIndent();
                CustomEditorProperties.EndFoldoutWindowBox();
                EditorGUI.EndDisabledGroup();             
                CustomEditorProperties.EndIndent();
                //Critical Hits

                //Damage Over Time
                CustomEditorProperties.BeginIndent(45);
                EditorGUILayout.PropertyField(property.FindPropertyRelative("UseDamageOverTime"));
                EditorGUI.BeginDisabledGroup(!property.FindPropertyRelative("UseDamageOverTime").boolValue);
                CustomEditorProperties.BeginFoldoutWindowBox();
                CustomEditorProperties.BeginIndent(15);
                EditorGUILayout.PropertyField(property.FindPropertyRelative("DamageOverTimeSettings"));
                CustomEditorProperties.EndIndent();
                CustomEditorProperties.EndFoldoutWindowBox();
                EditorGUI.EndDisabledGroup();
                CustomEditorProperties.EndIndent();
                //Damage Over Time
            }

            GUILayout.Space(2.5f);
            CustomEditorProperties.EndFoldoutWindowBox();
            GUILayout.Space(2.5f);
        }

        void SetModule (SerializedProperty property, bool State)
        {
            property.FindPropertyRelative("Enabled").boolValue = State;
        }
    }
}