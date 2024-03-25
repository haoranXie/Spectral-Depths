using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Tools;
using TMPro;
using EmeraldAI;
namespace SpectralDepths.TopDown
{	
    public class SquadDisplay : MonoBehaviour
    {
		/// the prefab you want for your player
		[Header("UI Objects")]
		[PLInformation("All the objects that are being used to create the the Squad Display",PLInformationAttribute.InformationType.Info,false)]
		//Name of the squad
		[Tooltip("Name of the squad")]
		public GameObject SquadName;
		//Grid Layout of the Squad Display where all the slots are inserted
		[Tooltip("Grid Layout of the Squad Display where all the slots are inserted")]
		public GameObject SquadLayout;
        //All the slots containing squad info
		[Tooltip("Character Slot Pool")]
		public PLSimpleObjectPooler CharacterSlotPool;
        //All the slots containing squad info
        public List<CharacterSlot> CharacterSlots;
        //The gameobject that each slot is based off of
        private GameObject _characterSlotPrefab;
        void Start()
        {           
            _characterSlotPrefab = Resources.Load<GameObject>("UI/Character Slot"); 
            SetAllCharacterSlots(LevelManager.Instance.Players);
        }

        protected void SetAllCharacterSlots(List<Character> characters)
        {
            for(int i = 0 ; i<characters.Count; i++)
            {
                //GameObject newCharacterSlot = Instantiate(_characterSlotPrefab);
                GameObject newCharacterSlot = CharacterSlotPool.GetPooledGameObject();
                CharacterSlot characterSlot = newCharacterSlot.GetComponent<CharacterSlot>();
                newCharacterSlot.transform.SetParent(SquadLayout.transform);

                //Set the key used to instantly take over a character
                CharacterAbilityOverdrive characterAbilityOverdrive = characters[i].GetComponent<CharacterAbilityOverdrive>();
                if(characterAbilityOverdrive != null)
                {
                    characterAbilityOverdrive.CharacterKey = i+1;
                }                

                //Setup text info
                if(characters[i].CharacterComponentData!=null)
                {
                    characterSlot.CharacterName.text = characters[i].CharacterName;
                    characterSlot.ModelName.text = characters[i].CharacterComponentData.Class;
                    characterSlot.Icon.sprite = characters[i].CharacterIcon;
                    EmeraldHealth health = characters[i].EmeraldComponent.GetComponent<EmeraldHealth>();
                    if(health!=null){characterSlot.HealthNumber.text = health.CurrentHealth.ToString() + "/" + health.StartingHealth.ToString();}
                }

                //Sets the popsition up correctly
                Vector3 newPosition = newCharacterSlot.GetComponent<RectTransform>().localPosition;      
                newPosition.z = 0;
                newCharacterSlot.GetComponent<RectTransform>().localPosition = newPosition;
                newCharacterSlot.transform.localScale = new Vector3(1f,1f,1f);
                newCharacterSlot.transform.SetAsLastSibling();
                
                //Initializes the CharacterSlot component
                characterSlot.CharacterComponent = characters[i];
                characterSlot.InitializeSlot();
                

                CharacterSlots.Add(characterSlot);
            }
        }
    }
}