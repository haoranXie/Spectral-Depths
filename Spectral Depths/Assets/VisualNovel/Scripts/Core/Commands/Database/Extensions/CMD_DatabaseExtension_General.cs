using DIALOGUE;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using TMPro;
using UnityEngine;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_General : CMD_DatabaseExtension
    {
        private static readonly string[] PARAM_SPEED = new string[] { "-s", "-spd" };
        private static readonly string[] PARAM_IMMEDIATE = new string[] { "-i", "-immediate" };

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("wait", new Func<string, IEnumerator>(Wait));

            //Dialogue System Controls
            database.AddCommand("showui", new Func<string[], IEnumerator>(ShowDialogueSystem));
            database.AddCommand("hideui", new Func<string[], IEnumerator>(HideDialogueSystem));

            //Dialogue Box Controls
            database.AddCommand("showdb", new Func<string[], IEnumerator>(ShowDialogueBox));
            database.AddCommand("hidedb", new Func<string[], IEnumerator>(HideDialogueBox));

            //MY OWN Switch Dialogue Bar
            database.AddCommand("usebigdb", new Func<string[], IEnumerator>(UseBigDialogueBox));
            database.AddCommand("usesmalldb", new Func<string[], IEnumerator>(UseSmallDialogueBox));
        }

        private static IEnumerator Wait(string data)
        {
            if (float.TryParse(data, out float time))
            {
                yield return new WaitForSeconds(time);
            }
        }

        private static IEnumerator ShowDialogueBox(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            yield return DialogueSystem.instance.dialogueContainer.Show(speed, immediate);
        }

        private static IEnumerator HideDialogueBox(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            yield return DialogueSystem.instance.dialogueContainer.Hide(speed, immediate);
        }

        private static IEnumerator ShowDialogueSystem(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            yield return DialogueSystem.instance.Show(speed, immediate);
        }

        private static IEnumerator HideDialogueSystem(string[] data)
        {
            float speed;
            bool immediate;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_SPEED, out speed, defaultValue: 1f);
            parameters.TryGetValue(PARAM_IMMEDIATE, out immediate, defaultValue: false);

            yield return DialogueSystem.instance.Hide(speed, immediate);
        }

        //MY OWN
        public DialogueContainer dialogueContainer;
        private static IEnumerator UseBigDialogueBox(string[] data)
        {
            DialogueSystem.instance.dialogueContainer.root = GameObject.Find("Big Root Container");
            DialogueSystem.instance.dialogueContainer.dialogueText = GameObject.Find("Big Root Container").GetComponentInChildren<TextMeshProUGUI>();
            yield return null;
        }

        private static IEnumerator UseSmallDialogueBox(string[] data)
        {
            DialogueSystem.instance.dialogueContainer.root = GameObject.Find("Root Container");
            DialogueSystem.instance.dialogueContainer.dialogueText = GameObject.Find("Root Container").GetComponentInChildren<TextMeshProUGUI>();
            yield return null;
        }
    }
}