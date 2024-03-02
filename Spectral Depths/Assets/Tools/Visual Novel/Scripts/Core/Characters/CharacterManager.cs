using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    //The central hub for creating, retrieving, and managing characters in the scene
    public class CharacterManager : MonoBehaviour
    {
        public static CharacterManager instance { get; private set; }
        private Dictionary<string, Character> characters = new Dictionary<string, Character>();

        private CharacterConfigSO config => DialogueSystem.instance.config.characterConfigurationAsset;

        private void Awake()
        {
            instance = this;
        }

        //Creating a character
        public Character CreateCharacter(string characterName)
        {
            if (characters.ContainsKey(characterName.ToLower()))
            {
                Debug.LogWarning($"A Character called '{characterName}' already exists. Did not create the character.");
                return null;
            }

            CHARACTER_INFO info = GetCharacterInfo(characterName);

            Character character = CreateCharacterFromInfo(info);

            characters.Add(characterName.ToLower(), character);

            return character;
        }

        private CHARACTER_INFO GetCharacterInfo(string characterName)
        {
            CHARACTER_INFO result = new CHARACTER_INFO();

            result.name = characterName;

            result.config = config.GetConfig(characterName);

            return result;
        }

        private Character CreateCharacterFromInfo(CHARACTER_INFO info)
        {
            switch(info.config.characterType)
            {
                case Character.CharacterType.Text:
                    return new CharacterText(info.name);

                case Character.CharacterType.Sprite:
                case Character.CharacterType.SpriteSheet:
                    return new CharacterSprite(info.name);
            }
            CharacterConfigData config = info.config;
            return null;
        }

        private class CHARACTER_INFO
        {
            public string name = "";

            public CharacterConfigData config = null;
        }
    }
}