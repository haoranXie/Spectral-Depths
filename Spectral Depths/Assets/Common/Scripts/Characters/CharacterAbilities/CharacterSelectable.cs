using System.Collections;
using System.Collections.Generic;
using SpectralDepths.Tools;
using UnityEngine;

namespace SpectralDepths.TopDown{
    public class CharacterSelectable : CharacterAbility, PLEventListener<RTSEvent>
    {
        /// <summary>
        /// Add this ability to a Character to have it be selectable by player 
        /// </summary>
        [AddComponentMenu("Spectral Depths/Character/Abilities/Character Selectable")] 

		[Header("Selected Visual")]

		/// visual shown when character is selected
		[Tooltip("visual to indicate when character is selected")]
		public Transform SelectedVisual;

		/// collider to select the character
		[Tooltip("collider to select the character")]
		public Collider SelectedCollider;

        
        public bool selected;
        public bool OnlySelected; //True if character is only one selected 
        protected override void Initialization()
        {
            base.Initialization();
            SelectedVisual.gameObject.SetActive(false);
        }

        public virtual void OnMMEvent(RTSEvent rtsEvent){
            switch(rtsEvent.EventType)
            {
                case RTSEventTypes.PlayerSelected:
                    if(rtsEvent.SelectedTable.ContainsKey(_character.GetInstanceID())){
                        Selected();
                        if(rtsEvent.SelectedTable.Count==1)
                        {
                            OnlySelected=true;
                        }
                        else
                        {
                            OnlySelected=false;
                        }
                    } else{
                        DeSelected();
                    }
                    break;
            }
        }

        private void Selected(){
            selected=true;
            SelectedVisual.gameObject.SetActive(true);
        }

        private void DeSelected(){
            selected=false;
            OnlySelected=false;
            SelectedVisual.gameObject.SetActive(false);
        }

        protected override void OnEnable(){
            base.OnEnable();
            this.PLEventStartListening<RTSEvent>();
        }

        protected override void OnDisable(){
            base.OnDisable();
            DeSelected();
            this.PLEventStopListening<RTSEvent>();
            RTSEvent.Trigger(RTSEventTypes.SelectionDisabled,_character,null);
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            DeSelected();
            SelectedCollider.gameObject.SetActive(false);
        }

    }
}