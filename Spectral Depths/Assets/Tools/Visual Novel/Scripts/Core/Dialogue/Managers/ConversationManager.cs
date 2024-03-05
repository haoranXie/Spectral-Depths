using COMMANDS;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Hosting;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace DIALOGUE
{
    //Handles all logic to run dialogue on screen, one line at a time
    public class ConversationManager
    {
        private DialogueSystem dialogueSystem => DialogueSystem.instance;
        private Coroutine process = null;

        public bool isRunning => process != null; //helps check if it's already running so only one conversation runs at a time

        private TextArchitect architect = null;
        private bool userPrompt = false;

        public ConversationManager(TextArchitect architect)
        {
            this.architect = architect;
            dialogueSystem.onUserPrompt_Next += OnUserPrompt_Next; //When onUserPrompt_Next event runs, it will trigger the method OnUserPrompt_Next;
        }

        //Switches userPrompt to true so that in Line_RunDialogue it will cause the text to complete
        private void OnUserPrompt_Next()
        {
            userPrompt = true;
        }
        public Coroutine StartConversation(List<string> conversation)
        {
            StopConversation(); //Stops conversation if there is already one going on

            process = dialogueSystem.StartCoroutine(RunningConversation(conversation));

            return process;
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

                //THIS MAKES COMMANDS AUTOMATICALLY RUN
                if (line.hasDialogue)
                    //Wait for user input
                    yield return WaitForUserInput(); //THIS MAKES COMMANDS AUTOMATICALLY RUN
            }
        }

        IEnumerator Line_RunDialogue(DialogueLine line)
        {
            //Show or hide the speaker name if there is one present
            if (line.hasSpeaker)
                dialogueSystem.ShowSpeakerName(line.speakerData.displayname);
            else
                dialogueSystem.HideSpeakerName();//REMOVE ELSE IF YOU DONT WANT TO TYPE SPEAKER NAME EACH TIME FOR SAME CHARACTER IN TEXTFILE

            //Build dialogue
            yield return BuildLineSegments(line.dialogueData);

            //Wait for user input
            //yield return WaitForUserInput(); THIS MAKES COMMANDS RUN AFTER ANOTHER CLICK
        }

        IEnumerator Line_RunCommands(DialogueLine line)
        {
            List<DLCommandData.Command> commands = line.commandData.commands;

            foreach(DLCommandData.Command command in commands)
            {
                if (command.waitForCompletion)
                    yield return CommandManager.instance.Execute(command.name, command.arguments);
                else
                    CommandManager.instance.Execute(command.name, command.arguments);
            }

            yield return null;
        }

        IEnumerator BuildLineSegments(DLDialogueData line)
        {
            for(int i = 0; i < line.segments.Count; i++)
            {
                DLDialogueData.DIALOGUE_SEGMENT segment = line.segments[i];

                yield return WaitForDialogueSegmentSignalToBeTriggered(segment);

                yield return BuildDialogue(segment.dialogue, segment.appendText);
            }
        }

        IEnumerator WaitForDialogueSegmentSignalToBeTriggered(DLDialogueData.DIALOGUE_SEGMENT segment)
        {
            switch (segment.startSignal)
            {
                case DLDialogueData.DIALOGUE_SEGMENT.StartSignal.C:
                case DLDialogueData.DIALOGUE_SEGMENT.StartSignal.A:
                    yield return WaitForUserInput();
                    break;
                case DLDialogueData.DIALOGUE_SEGMENT.StartSignal.WC:
                case DLDialogueData.DIALOGUE_SEGMENT.StartSignal.WA:
                    yield return new WaitForSeconds(segment.signalDelay);
                    break;
                default:
                    break;
            }
        }

        //Dialogue change on user click
        IEnumerator BuildDialogue(string dialogue, bool append = false)
        {
            //Build the dialogue
            if (!append)
                architect.Build(dialogue);
            else
                architect.Append(dialogue);

            //Wait for the dialogue to complete
            while (architect.isBuilding)
            {
                if (userPrompt)
                {
                    architect.ForceComplete();

                    userPrompt = false;
                }

                yield return null;
            }
        }

        IEnumerator WaitForUserInput()
        {
            while (!userPrompt)
                yield return null;

            userPrompt = false;
        }
    }
}