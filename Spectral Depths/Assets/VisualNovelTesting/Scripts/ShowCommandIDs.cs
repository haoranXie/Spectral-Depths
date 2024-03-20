using COMMANDS;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TESTING
{
    public class ShowCommandIDs : MonoBehaviour
    {
        public TextMeshProUGUI txt;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            txt.text = "<u>Commands:</u>\n";
            int i = 1;
            foreach (var cmd in CommandManager.instance.activeProcesses)
            {
                txt.text += $"{i}. [{cmd.ID}] '{cmd.command}({string.Join(',', cmd.args)})'\n";
                i++;
            }
        }
    }
}