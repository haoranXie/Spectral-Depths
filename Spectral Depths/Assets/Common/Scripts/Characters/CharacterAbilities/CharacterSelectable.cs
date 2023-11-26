using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

namespace SpectralDepths.TopDown{
    public class CharacterSelectable : CharacterAbility, MMEventListener<SelectionEvent>
    {
        /// <summary>
        /// Add this ability to a Character to have it be selectable by player 
        /// </summary>
        [AddComponentMenu("Spectral Depths/Character/Abilities/Character Selectable")] 

		[Header("Selected Visual")]

		/// visual shown when character is selected
		[Tooltip("visual to indicate when character is selected")]
		public Transform SelectedVisual;

        
        public bool selected;
        protected override void Initialization()
        {
            base.Initialization();
            SelectedVisual.gameObject.SetActive(false);
        }

        public virtual void OnMMEvent(SelectionEvent selectionEvent){
            if(selectionEvent.SelectedTable.ContainsKey(_character.gameObject.GetInstanceID())){
                Selected();
            } else{
                DeSelected();
            }
        }

        private void Selected(){
            selected=true;
            SelectedVisual.gameObject.SetActive(true);
        }

        private void DeSelected(){
            selected=false;
            SelectedVisual.gameObject.SetActive(false);
        }

        protected override void OnEnable(){
            base.OnEnable();
            this.MMEventStartListening<SelectionEvent>();
        }

        protected override void OnDisable(){
            base.OnDisable();
            this.MMEventStopListening<SelectionEvent>();
        }

    }
}