using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DIALOGUE
{
    //Hub for player input controls and logic
    public class PlayerInputManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //When space button pressed, call Prompt Advance(click detection is built into scene)
            if (Input.GetKeyDown(KeyCode.Space))
                PromptAdvance();
        }

        //Instantly completes the text by calling OnUserPrompt_Next method
        public void PromptAdvance()
        {
            DialogueSystem.instance.OnUserPrompt_Next();
        }
    }
}
