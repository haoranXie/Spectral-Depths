using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    //A character that uses sprites or sprite sheets to render its display
    public class CharacterSprite : Character
    {
        public CharacterSprite(string name, CharacterConfigData config) : base(name, config)
        {
            Debug.Log($"Created Sprite Character: '{name}'");
        }
    }
}