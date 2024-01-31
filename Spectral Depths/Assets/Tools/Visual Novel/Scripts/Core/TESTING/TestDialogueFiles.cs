using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDialogueFiles : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartConversation();
    }

    void StartConversation()
    {
        List<string> lines = FileManager.ReadTextAsset("testFile"); //Reads the text in "testFile.txt"

        DialogueSystem.instance.Say(lines); //Inputs lines into Dialogue System to put into method "Say"
    }
}
