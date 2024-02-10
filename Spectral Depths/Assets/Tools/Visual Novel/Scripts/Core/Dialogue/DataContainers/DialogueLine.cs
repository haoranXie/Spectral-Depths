using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE
{
    //Storage container for dialogue information that has been parsed and ripped from a string
    public class DialogueLine
    {
        //Three parts of string
        public string speaker;
        public DLDialogueData dialogue;
        public string commands;

        public bool hasSpeaker => speaker != string.Empty; //Checks if have speaker
        public bool hasDialogue => dialogue.hasDialogue; //Checks if have dialogue
        public bool hasCommands => commands != string.Empty; //Checks if have commands

        public DialogueLine(string speaker, string dialogue, string commands)
        {
            this.speaker = speaker;
            this.dialogue = new DLDialogueData(dialogue);
            this.commands = commands;
        }
    }
}