using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;
using DIALOGUE;
using TMPro;
using UnityEditor.Rendering;

namespace TESTING
{
    public class TestCharacters : MonoBehaviour
    {
        public TMP_FontAsset tempFont;

        private Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

        // Start is called before the first frame update
        void Start()
        {
            
            //Character Raelin = CharacterManager.instance.CreateCharacter("Raelin");
            //Character Stella2 = CharacterManager.instance.CreateCharacter("Stella");
            //Character Adam = CharacterManager.instance.CreateCharacter("Adam");
            StartCoroutine(Test());
        }

        IEnumerator Test()
        {

            Character Raelin = CharacterManager.instance.CreateCharacter("Raelin");

            yield return new WaitForSeconds(1);
            Raelin.Highlight();

            yield return new WaitForSeconds(1);

            Raelin.Say("Hi");

            yield return new WaitForSeconds(1);

            Raelin.UnHighlight();

            yield return new WaitForSeconds(1);

            Raelin.Highlight();

            Raelin.Flip();

            yield return new WaitForSeconds(1);
            Raelin.FaceRight();

            yield return new WaitForSeconds(1);
            Raelin.FaceLeft();
            yield return new WaitForSeconds(1);

            yield return null;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}