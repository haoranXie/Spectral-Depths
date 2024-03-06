using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;
using DIALOGUE;
using TMPro;

namespace TESTING
{
    public class TestCharacters : MonoBehaviour
    {
        public TMP_FontAsset tempFont;

        // Start is called before the first frame update
        void Start()
        {
            //Character Generic = CharacterManager.instance.CreateCharacter("Generic");
            //Character Michiru = CharacterManager.instance.CreateCharacter("Michiru");
            //Character Michiru2 = CharacterManager.instance.CreateCharacter("Michiru");
            //Character DNE = CharacterManager.instance.CreateCharacter("DNE");
            StartCoroutine(Test());
        }

        IEnumerator Test()
        {
            Character Raelin = CharacterManager.instance.CreateCharacter("Raelin");
            yield return new WaitForSeconds(1f);
            yield return Raelin.Hide();
            yield return new WaitForSeconds(0.5f);
            yield return Raelin.Show();
            yield return Raelin.Say("Hello");

            //Test Character Speaking
            /**
            Character Elen = CharacterManager.instance.CreateCharacter("Elen");
            Character Adam = CharacterManager.instance.CreateCharacter("Adam");
            Character Ben = CharacterManager.instance.CreateCharacter("Benjamin");

            List<string> lines = new List<string>()
            {
                "Hi, there!",
                "My name is Elen.",
                "What's your name?",
                "Oh,{wa 1} that's very nice."
            };
            yield return Elen.Say(lines);

            Elen.SetNameColor(Color.red);
            Elen.SetDialogueColor(Color.green);
            Elen.SetNameFont(tempFont);
            Elen.SetDialogueFont(tempFont);

            yield return Elen.Say(lines);

            Elen.ResetConfigurationData();

            yield return Elen.Say(lines);

            lines = new List<string>()
            {
                "I am Adam.",
                "More lines{c}Here."
            };
            yield return Adam.Say(lines);

            yield return Ben.Say("This is a line that I want to say.{a} It is a simple line.");

            Debug.Log("Finished");
            **/
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}