using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CHARACTERS
{
    //A character with no grahical art. Text operations only
    public class CharacterText : Character
    {
        public CharacterText(string name, CharacterConfigData config) : base(name, config, prefab: null)
        {
            Debug.Log($"Created Text Character: '{name}'");
        }
    }
}