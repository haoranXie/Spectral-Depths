using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SpectralDepths.TopDown
{
    [CreateAssetMenu(fileName = "Character Data", menuName = "SpectralDepths/Character Data")]
    public class CharacterData : ScriptableObject
    {
        public string CharacterName;
        public string Class;
        public int MaxHealth;
        public Sprite Icon; 

    }
}