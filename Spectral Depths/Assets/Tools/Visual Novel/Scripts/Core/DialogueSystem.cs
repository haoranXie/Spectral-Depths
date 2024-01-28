using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DIALOGUE
{
    //System to control the dialogue on the screen
    public class DialogueSystem : MonoBehaviour
    {
        //Allows us to access it from inspector despite it being private
        public DialogueContainer dialogueContainer = new DialogueContainer();

        public static DialogueSystem instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                DestroyImmediate(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}