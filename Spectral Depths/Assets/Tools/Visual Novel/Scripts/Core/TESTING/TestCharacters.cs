using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;
using DIALOGUE;
using TMPro;
using System.Security.AccessControl;
using UnityEngine.ProBuilder;

namespace TESTING
{
    public class TestCharacters : MonoBehaviour
    {
        public TMP_FontAsset tempFont;

        private Character CreateCharacter(string name) => CharacterManager.instance.CreateCharacter(name);

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Test());
        }

        IEnumerator Test()
        {
            //Test moving, transitioning, setting, everything
            CharacterSprite Guard = CreateCharacter("Guard as Generic") as CharacterSprite;
            CharacterSprite Raelin = CreateCharacter("Raelin") as CharacterSprite;
            Guard.isVisible = false;

            yield return new WaitForSeconds(1);

            Sprite body = Raelin.GetSprite("Raelin_3");
            Sprite face = Raelin.GetSprite("Raelin_7");
            Raelin.TransitionSprite(body);
            yield return Raelin.TransitionSprite(face, 1, 0.3f);

            Raelin.MoveToPosition(Vector2.zero);
            Guard.Show();
            yield return Guard.MoveToPosition(new Vector2(1, 0));

            Raelin.TransitionSprite(Raelin.GetSprite("Raelin_11"), layer:1);

            body = Guard.GetSprite("Man");
            face = Guard.GetSprite("Girl");

            Guard.TransitionSprite(body);
            yield return new WaitForSeconds(1);
            Guard.TransitionSprite(face);

            /**
            CharacterSprite Raelin = CreateCharacter("Raelin") as CharacterSprite;

            yield return new WaitForSeconds(1);

            yield return Raelin.TransitionSprite(Raelin.GetSprite("Raelin_7"), 1);
            Raelin.TransitionSprite(Raelin.GetSprite("Raelin_3"));
            **/

            yield return null;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}