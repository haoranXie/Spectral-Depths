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
        public string dialogue;
        public string commands;

        public DialogueLine(string speaker, string dialogue, string commands)
        {
            this.speaker = speaker;
            this.dialogue = dialogue;
            this.commands = commands;
        }
    }
}