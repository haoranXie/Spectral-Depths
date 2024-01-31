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
        private ConversationManager conversationManager = new ConversationManager();

        public static DialogueSystem instance;

        public bool isRunningConversation => conversationManager.isRunning;  //Gets data from script, conversationManager on if it's running or not

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                DestroyImmediate(gameObject);
        }

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