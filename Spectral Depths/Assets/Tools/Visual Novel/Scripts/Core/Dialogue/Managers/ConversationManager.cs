using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE
{
    //Handles all logic to run dialogue on screen, one line at a time
    public class ConversationManager
    {
        private DialogueSystem dialogueSystem => DialogueSystem.instance;
        private Coroutine process = null;

        public bool isRunning => process != null; //helps check if it's already running so only one conversation runs at a time

        public void StartConversation(List<string> conversation)
        {
            StopConversation(); //Stops conversation if there is already one going on

            process = dialogueSystem.StartCoroutine(RunningConversation(conversation));
        }

        //Stops conversation if there is already one going on
        public void StopConversation()
        {
            if (!isRunning)
                return;

            dialogueSystem.StopCoroutine(process);
            process = null;
        }

        IEnumerator RunningConversation(List<string> conversation)
        {
            //Goes through every dialogue line in list of strings
            for (int i = 0; i < conversation.Count; i++)
            {
                //Skips line if there is nothing
                if (string.IsNullOrWhiteSpace(conversation[i]))
                    continue;

                DialogueLine line = DialogueParser.Parse(conversation[i]); //DialogueLine is (Name, Dialogue, Command) and dialogue parser splits the lines into that format

                //Show dialogue
                if (line.hasDialogue)
                    yield return Line_RunDialogue(line);

                //Run any commands
                if (line.hasCommands)
                    yield return Line_RunCommands(line);
            }
        }

        IEnumerator Line_RunDialogue(DialogueLine line)
        {
            yield return null;
        }

        IEnumerator Line_RunCommands(DialogueLine line)
        {
            yield return null;
        }
    }
}