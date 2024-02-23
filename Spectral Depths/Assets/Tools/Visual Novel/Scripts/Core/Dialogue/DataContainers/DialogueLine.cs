using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE
{
    //Storage container for dialogue information that has been parsed and ripped from a string
    public class DialogueLine
    {
        //Three parts of string
        public DLSpeakerData speakerData;
        public DLDialogueData dialogueData;
        public DLCommandData commandData;

        public bool hasSpeaker => speakerData != null; // speaker != string.Empty; //Checks if have speaker
        public bool hasDialogue => dialogueData != null; //Checks if have dialogue
        public bool hasCommands => commandData != null; //Checks if have commands

        public DialogueLine(string speaker, string dialogue, string commands)
        {
            this.speakerData = (string.IsNullOrWhiteSpace(speaker) ? null : new DLSpeakerData(speaker));
            this.dialogueData = (string.IsNullOrWhiteSpace(dialogue) ? null : new DLDialogueData(dialogue));
            this.commandData = (string.IsNullOrWhiteSpace(commands) ? null : new DLCommandData(commands));
        }
    }
}