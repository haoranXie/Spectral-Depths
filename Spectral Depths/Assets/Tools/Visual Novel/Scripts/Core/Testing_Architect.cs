using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TESTING
{
    public class Testing_Architect : MonoBehaviour
    {
        DialogueSystem ds;
        TextArchitect architect;
        
        string[] lines = new string[5]
        {
            "PEBIS KHAN! ",
            "I like apples. ",
            "I don't like apples. ",
            "I like pebbles. ",
            "We are lost in a maze of energy pipes bulging with vitality. It spans throughout the entire district and branches out like the veins of a living creature. The air is filled with the smell of the pure energy that courses through these pipelines, but despite how miserable the situation looked, we were all relieved and hopeful that maybe the days of the one-sided massacre were behind us. We approach the room where the remaining members have gathered. "
        };

        // Start is called before the first frame update
        void Start()
        {
            ds = DialogueSystem.instance;
            architect = new TextArchitect(ds.dialogueContainer.dialogueText);
            architect.buildMethod = TextArchitect.BuildMethod.typewriter;
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                if(architect.isBuilding)
                {
                    if (!architect.hurryUp)
                    {
                        architect.hurryUp = true;
                    }
                    else
                    {
                        architect.ForceComplete();
                    }
                }
                else
                {
                    architect.Build(lines[Random.Range(0, lines.Length)]);
                }
            }
            else if(Input.GetKeyDown(KeyCode.A))
            {
                architect.Append(lines[Random.Range(0, lines.Length)]);
            }
        }
    }
}