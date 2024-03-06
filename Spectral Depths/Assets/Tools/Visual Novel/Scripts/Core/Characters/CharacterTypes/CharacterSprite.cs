using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    //A character that uses sprites or sprite sheets to render its display
    public class CharacterSprite : Character
    {
        private CanvasGroup rootCG => root.GetComponent<CanvasGroup>();

        public CharacterSprite(string name, CharacterConfigData config, GameObject prefab) : base(name, config, prefab)
        {
            rootCG.alpha = 0; //STARTS OFF INVISIBLE
            Debug.Log($"Created Sprite Character: '{name}'");
        }

        public override IEnumerator ShowingOrHiding(bool show)
        {
            float targetAlpha = show ? 1f : 0;
            CanvasGroup self = rootCG;

            while (self.alpha != targetAlpha)
            {
                self.alpha = Mathf.MoveTowards(self.alpha, targetAlpha, 3f * Time.deltaTime); //CHANGES SPEED AT WHICH CHARACTER FADES IN
                yield return null;
            }

            co_revealing = null;
            co_hiding = null;
        }
    }
}