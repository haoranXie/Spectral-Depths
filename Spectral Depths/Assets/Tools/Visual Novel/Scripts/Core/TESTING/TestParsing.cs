using DIALOGUE;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class NewBehaviourScript : MonoBehaviour
    {
        //Drog text files in inspector to change what to parse
        [SerializeField] private TextAsset file;
        // Start is called before the first frame update
        void Start()
        {
            SendFileToParse();
        }

        void SendFileToParse()
        {
            List<string> lines = FileManager.ReadTextAsset("testFile"); //Reads the text in "testFile.txt"

            foreach(string line in lines)
            {
                if (line == string.Empty)
                    continue;

                DialogueLine dl = DialogueParser.Parse(line); //Parses the line(For now also displays it in console)
            }
        }
    }
}