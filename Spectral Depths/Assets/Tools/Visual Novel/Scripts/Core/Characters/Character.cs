using DIALOGUE;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    //The Base class from which all character types derive from
    public class Character
    {
        public string name = "";
        public string displayName = "";
        public RectTransform root = null;
        public CharacterConfigData config;

        public DialogueSystem dialogueSystem => DialogueSystem.instance;

        //When creating a new character
        public Character(string name, CharacterConfigData config)
        {
            this.name = name;
            displayName = name;
            this.config = config;
        }

        public Coroutine Say(string dialogue) => Say(new List<string> { dialogue });

        public Coroutine Say(List<string> dialogue)
        {
            dialogueSystem.ShowSpeakerName(displayName);
            dialogueSystem.ApplySpeakerDataToDialogueContainer(config);

            //NOT OFFICIAL
            dialogue = FormatDialogue(dialogue);
            //NOT OFFICIAL

            return dialogueSystem.Say(dialogue);
        }

        //Determining what kind of character
        public enum CharacterType
        {
            Text,
            Sprite,
            SpriteSheet,
        }

        //NOT OFFICIAL STUFF
        private List<string> FormatDialogue(List<string> dialogue)
        {
            List<string> formattedDialogue = new List<string>();

            foreach (string line in dialogue)
            {
                formattedDialogue.Add($"{displayName} \"{line}\"");
            }

            return formattedDialogue;
        }
        //NOT OFFICIAL STUFF
    }
}