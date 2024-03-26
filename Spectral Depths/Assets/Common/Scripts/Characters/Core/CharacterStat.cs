using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Tools;
using EmeraldAI;

namespace SpectralDepths.TopDown
{
    public class CharacterStat : MonoBehaviour
    {
		[PLInformation("Component that distributes parameters like stats to other components.",SpectralDepths.Tools.PLInformationAttribute.InformationType.Info,false)]
		[Tooltip("Base Scriptable Object that contains stats and identification for a character")]
        public CharacterData CharacterComponentData;
        [Header("Custom Identity")]
		public bool UseCustomIdentity = false;
		[Tooltip("Name assigned to character inside the UI. NOT used for identification")]
		[PLCondition("UseCustomIdentity", true)] 
        public string CharacterName;
		[Tooltip("Class used in the UI")]
		[PLCondition("UseCustomIdentity", true)] 
        public string CharacterClass;
		[Tooltip("Image for character on the UI")]
		[PLCondition("UseCustomIdentity", true)] 
        public Sprite CharacterIcon;
		[Header("Custom Stats")]
        public bool UseCustomStat = false;
		[Tooltip("Name assigned to character inside the UI. NOT used for identification")]
		[PLCondition("UseCustomStat", true)] 
        public int StartHealth = -1;
        private Character _character;
        private EmeraldSystem _emeraldComponent;
        private EmeraldHealth _emeraldHealth;
        private EmeraldUI _emeraldUI;
        void Awake()
        {
            _character = GetComponentInParent<Character>();
            _emeraldComponent = GetComponent<EmeraldSystem>();
            _emeraldHealth = GetComponent<EmeraldHealth>();
            _emeraldUI = GetComponent<EmeraldUI>();
            SetupCharacter();
        }
		protected void SetupCharacter()
		{
			UpdateIdentity();
			UpdateStats();
		}
		/// <summary>
		/// Re-updates all the character's identifiers 
		/// </summary>
		public void UpdateIdentity()
		{
            //We first try to set our identity based off of the CharacterComponentData
            if(CharacterComponentData)
            {
                if(_character!=null)
                {
                    _character.CharacterName = CharacterComponentData.name;
                    _character.CharacterIcon = CharacterComponentData.Icon;
                    _character.CharacterClass = CharacterComponentData.Class;
                    if(_emeraldUI!=null)
                    {
                        _emeraldUI.AIName = CharacterComponentData.name;
                    }
                }
            }
            //We then try to setup based off Custom Identity
            if(UseCustomIdentity)
            {
                if(_character!=null)
                {
                    if(!string.IsNullOrEmpty(CharacterName)) _character.CharacterName = CharacterName;
                    if(!string.IsNullOrEmpty(CharacterClass)) _character.CharacterClass = CharacterClass;
                    if(CharacterIcon!=null) _character.CharacterIcon = CharacterIcon;
                    if(!string.IsNullOrEmpty(CharacterName) && _emeraldUI!=null) _emeraldUI.AIName = CharacterName;
                }
            }
		}	

		/// <summary>
		/// Re-updates all the character's stats by redistributing values through the CharacterData
		/// </summary>
		public void UpdateStats()
		{
            //Sets up stats based off of character component data
            if(CharacterComponentData)
            {
                if(_emeraldHealth!=null)
                {
                    _emeraldHealth.StartHealth = CharacterComponentData.MaxHealth;
                    _emeraldHealth.StartPoise = CharacterComponentData.MaxPoise;
                    _emeraldHealth.PoiseResistance = CharacterComponentData.PoiseResistance;
                    _emeraldHealth.PoiseResetTime = CharacterComponentData.PoiseResetTime;
                }         
            }

            if(UseCustomStat)
            {
                if(_emeraldHealth!=null)
                {
                    if(StartHealth!=-1) _emeraldHealth.StartHealth = StartHealth;
                }
            }


		}
    }
}