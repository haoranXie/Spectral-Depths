using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace DIALOGUE
{
    [System.Serializable]

    //Manages all dialogue box elements to change how they look
    public class DialogueContainer
    {
        //Dialogue and Name text connected to root so disabling this will hide dialogue
        public GameObject root;

        //Displays what character is talking
        public TextMeshProUGUI nameText;

        //Displays the correct dialogue
        public TextMeshProUGUI dialogueText;
    }
}