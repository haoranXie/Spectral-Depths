using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Tools;
using TMPro;
using UnityEngine.UI;
using EmeraldAI;
namespace SpectralDepths.TopDown
{	
    public class StrainBar : MonoBehaviour,  PLEventListener<StrainEvent>
    {
		/// the prefab you want for your player
		[Header("UI Objects")]
		[PLInformation("All the references for a strain bar",PLInformationAttribute.InformationType.Info,false)]
		//Name of the squad
		[Tooltip("StrainProgressBar")]
        public PLProgressBar StrainProgressBar;
		[Tooltip("StrainNumber")]
        public TextMeshProUGUI StrainNumber;


        void Start()
        {
            StrainNumber.text = EnergyManager.Instance.Energy.ToString(); //+ "/" + EnergyManager.Instance.StartEnergy.ToString();
        }

        public virtual void OnMMEvent(StrainEvent engineEvent)
        {
            switch (engineEvent.EventType)
            {
                case StrainEventTypes.IncreaseStrain:
                    UpdateEnergyOnUI();
                    break;
                case StrainEventTypes.DecreaseStrain:
                    UpdateEnergyOnUI();
                    break;
                case StrainEventTypes.StrainChanged:
                    UpdateEnergyOnUI();
                    break;
            }
                    
        }
		void UpdateEnergyOnUI()
		{
            StrainNumber.text = EnergyManager.Instance.Energy.ToString(); //+ "/" + EnergyManager.Instance.StartEnergy.ToString();
			StrainProgressBar.UpdateBar(EnergyManager.Instance.Energy, 0, EnergyManager.Instance.StartEnergy);
		}
		protected virtual void OnEnable()
		{
			this.PLEventStartListening<StrainEvent> ();

		}

		/// <summary>
		/// OnDisable, we stop listening to events.
		/// </summary>
		protected virtual void OnDisable()
		{
			this.PLEventStopListening<StrainEvent> ();

        }
    }
}