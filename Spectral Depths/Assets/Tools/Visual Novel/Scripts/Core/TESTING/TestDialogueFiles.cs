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

        //Deubg split speaker lines
        /**
        for (int i = 0; i < lines.Count; i++)
        {
            string line = lines[i];

            if (string.IsNullOrWhiteSpace(line))
                continue;

            DialogueLine dl = DialogueParser.Parse(line);

            Debug.Log($"{dl.speaker.name} as [{(dl.speaker.castName != string.Empty ? dl.speaker.castName : dl.speaker.name)}]at {dl.speaker.castPosition}");

            List<(int l, string ex)> expr = dl.speaker.CastExpressions;
            for (int c = 0; c < expr.Count; c++)
            {
                Debug.Log($"[Layer[{expr[c].l}] = '{expr[c].ex}']");
            }
        }
        **/

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

        //Debug split command lines
        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            DialogueLine dl = DialogueParser.Parse(line);

            for(int i = 0; i < dl.commandData.commands.Count; i++)
            {
                DLCommandData.Command command = dl.commandData.commands[i];
                Debug.Log($"Command [{i}] '{command.name}' has arguments [{string.Join(", ", command.arguments)}]");
            }
        }

        //DialogueSystem.instance.Say(lines); //Inputs lines into Dialogue System to put into method "Say"
    }
}
