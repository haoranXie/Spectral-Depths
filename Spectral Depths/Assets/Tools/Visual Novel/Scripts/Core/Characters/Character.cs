using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    //The Base class from which all character types derive from
    public class Character
    {
        public string name = "";
        public RectTransform root = null;

        //When creating a new character
        public Character(string name)
        {
            this.name = name;

        }

        //Determining what kind of character
        public enum CharacterType
        {
            Text,
            Sprite,
            SpriteSheet,
        }
    }
}