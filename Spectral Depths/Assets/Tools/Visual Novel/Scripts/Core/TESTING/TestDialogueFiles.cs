using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDialogueFiles : MonoBehaviour
{
    [SerializeField] private TextAsset fileToRead = null;
    // Start is called before the first frame update
    void Start()
    {
        StartConversation();
    }

    void StartConversation()
    {
        List<string> lines = FileManager.ReadTextAsset(fileToRead); //Reads the text in "testFile.txt"

        //Debug split dialogue lines
        /**
        foreach(string line in lines)
        {
            if (string.IsNullOrEmpty(line))
                continue;

            Debug.Log($"Segmenting line '{ line}'");
            DialogueLine dlLine = DialogueParser.Parse(line);

            int i = 0;
            foreach(DLDialogueData.DIALOGUE_SEGMENT segment in dlLine.dialogue.segments)
            {
                Debug.Log($"Segment [{i++}] = '{segment.dialogue}' [signal={segment.startSignal.ToString()}{(segment.signalDelay > 0 ? $" {segment.signalDelay}" : $"")}]");
            }
        }
        **/

        DialogueSystem.instance.Say(lines); //Inputs lines into Dialogue System to put into method "Say"
    }
}
