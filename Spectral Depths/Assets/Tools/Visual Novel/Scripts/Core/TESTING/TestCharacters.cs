using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CHARACTERS;

namespace TESTING
{
    public class TestCharacters : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Character Alice = CharacterManager.instance.CreateCharacter("Alice");
            Character Michiru = CharacterManager.instance.CreateCharacter("Michiru");
            Character Michiru2 = CharacterManager.instance.CreateCharacter("Michiru");
            Character DNE = CharacterManager.instance.CreateCharacter("DNE");
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}