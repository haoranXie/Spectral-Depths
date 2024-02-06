using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DIALOGUE
{
    //System to control the dialogue on the screen
    public class DialogueSystem : MonoBehaviour
    {
        //Declares method to call other scripts
        public DialogueContainer dialogueContainer = new DialogueContainer();
        private ConversationManager conversationManager;
        private TextArchitect architect;

        public static DialogueSystem instance;

        public delegate void DialogueSystemEvent();
        public event DialogueSystemEvent onUserPrompt_Next; //For any script that is subscribed to DialogueSystemEvent, it will prompt this

        public bool isRunningConversation => conversationManager.isRunning;  //Gets data from script, conversationManager on if it's running or not

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                Initialize();
            }
            else
                DestroyImmediate(gameObject);
        }

        bool _initialized = false;
        private void Initialize()
        {
            if (_initialized)
                return;

            architect = new TextArchitect(dialogueContainer.dialogueText);
            conversationManager = new ConversationManager(architect);
        }

        public void OnUserPrompt_Next()
        {
            onUserPrompt_Next?.Invoke(); //Checks if onUserPrompt_Next is null so it only runs if it isn't null
        }

        //Method that shows speaker name
        public void ShowSpeakerName(string speakerName = "")
        {
            if (speakerName.ToLower() != "narrator") //CAN ADD LATER "THOUGHTS" SO IT ALSO DOESN'T SHOW WHEN ITS A THOUGHT
                dialogueContainer.nameContainer.Show(speakerName);
            else
                HideSpeakerName(); //THIS HIDES SPEAKER NAME IF SAME SPEAKER COMES BACK
        }

        //Method that hides speaker name
        public void HideSpeakerName() => dialogueContainer.nameContainer.Hide();

        //Takes in speaker and dialogue string and compacts it into a list of strings(conversation)
        public void Say(string speaker, string dialogue)
        {
            List<string> conversation = new List<string>() { $"{speaker} \"{dialogue}\"" };
            Say(conversation);
        }

        public void Say(List<string> conversation)
        {
            conversationManager.StartConversation(conversation);
        }
    }
}