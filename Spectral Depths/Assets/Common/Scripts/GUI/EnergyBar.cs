using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Tools;
using TMPro;
using UnityEngine.UI;
using EmeraldAI;
namespace SpectralDepths.TopDown
{	
    public class EnergyBar : MonoBehaviour,  PLEventListener<EnergyEvent>
    {
		/// the prefab you want for your player
		[Header("UI Objects")]
		[PLInformation("All the references for a energy bar",PLInformationAttribute.InformationType.Info,false)]
		//Name of the squad
		[Tooltip("EnergyBar")]
        public PLProgressBar EnergyProgressBar;
		[Tooltip("EnergyText")]
        public TextMeshProUGUI EnergyNumber;


        void Start()
        {
            EnergyNumber.text = EnergyManager.Instance.Energy.ToString(); //+ "/" + EnergyManager.Instance.StartEnergy.ToString();
        }

        public virtual void OnMMEvent(EnergyEvent engineEvent)
        {
            switch (engineEvent.EventType)
            {
                case EnergyEventTypes.UseEnergy:
                    UpdateEnergyOnUI();
                    break;
                case EnergyEventTypes.RecoverEnergy:
                    UpdateEnergyOnUI();
                    break;
                case EnergyEventTypes.EnergyChanged:
                    UpdateEnergyOnUI();
                    break;
            }
                    
        }
		void UpdateEnergyOnUI()
		{
            EnergyNumber.text = EnergyManager.Instance.Energy.ToString(); //+ "/" + EnergyManager.Instance.StartEnergy.ToString();
			EnergyProgressBar.UpdateBar(EnergyManager.Instance.Energy, 0, EnergyManager.Instance.StartEnergy);
		}
		protected virtual void OnEnable()
		{
			this.PLEventStartListening<EnergyEvent> ();

		}

		/// <summary>
		/// OnDisable, we stop listening to events.
		/// </summary>
		protected virtual void OnDisable()
		{
			this.PLEventStopListening<EnergyEvent> ();

        }
    }
}