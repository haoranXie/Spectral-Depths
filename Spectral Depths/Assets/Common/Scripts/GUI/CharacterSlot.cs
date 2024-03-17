using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpectralDepths.Tools;
using TMPro;
using UnityEngine.UI;
using EmeraldAI;
namespace SpectralDepths.TopDown
{	
    public class CharacterSlot : MonoBehaviour
    {
		/// the prefab you want for your player
		[Header("UI Objects")]
		[PLInformation("All the references for a character slot",PLInformationAttribute.InformationType.Info,false)]
		//Name of the squad
		[Tooltip("Healthbar")]
        public GameObject HealthBar;
		[Tooltip("HealthText")]
        public TextMeshProUGUI HealthNumber;
		[Tooltip("Strainbar")]
        public GameObject StrainBar;
		[Tooltip("Name of character")]
        public TextMeshProUGUI CharacterName;
		[Tooltip("Name of character's model")]
        public TextMeshProUGUI ModelName;
		[Tooltip("Name of character's class")]
        public TextMeshProUGUI Class;
		[Tooltip("Image for character icon")]
        public Image Icon;

		public EmeraldSystem EmeraldComponent;
		public Character CharacterComponent;
		public PLProgressBar HealthProgressBar;
		private CanvasGroup CG;
		private Coroutine C;

		public void InitializeSlot()
		{
			if(CharacterComponent.UseEmeraldAI){EmeraldComponent = CharacterComponent.EmeraldComponent;}
			HealthProgressBar = HealthBar.GetComponent<PLProgressBar>();
			CG = GetComponent<CanvasGroup>();
            EmeraldComponent.HealthComponent.OnDeath += OnDeathUI; //Subscribe FadeOutUI to the OnDeath delegate.
            EmeraldComponent.HealthComponent.OnTakeDamage += UpdateHealthOnUi; //Subscribe TransitionDamage to the OnTakeDamage delegate.
            EmeraldComponent.HealthComponent.OnTakeCritDamage += UpdateHealthOnUi; //Subscribe TransitionDamage to the OnTakeCritDamage delegate.
            EmeraldComponent.HealthComponent.OnHealRateTick += UpdateHealthOnUi; //Subscribe TransitionHealing to the OnHealRateTick delegate.
            EmeraldComponent.HealthComponent.OnHealthChange += UpdateHealthOnUi; //Subscribe UpdateHealthUI to the OnHealthChange delegate.
		}

		void OnDeathUI()
		{
            if (gameObject.activeSelf)
            {
                if (C != null) { StopCoroutine(C); }
                C = StartCoroutine(FadeOutUIInternal(0.0f, 1.5f));
            }
		}
        IEnumerator FadeOutUIInternal(float DesiredValue, float TransitionTime)
        {
			HealthProgressBar.UpdateBar(EmeraldComponent.HealthComponent.CurrentHealth, 0, EmeraldComponent.HealthComponent.StartHealth);
            float alpha = CG.alpha;
            float t = 0;

            while ((t / TransitionTime) < 1)
            {
                t += Time.deltaTime;
                Color newColor1 = new Color(1, 1, 1, Mathf.Lerp(alpha, DesiredValue, t));
                CG.alpha = newColor1.a;
                yield return null;
            }

            gameObject.SetActive(false);
			Destroy(this);
        }

		void UpdateHealthOnUi()
		{
            HealthNumber.text = EmeraldComponent.HealthComponent.CurrentHealth.ToString() + "/" + EmeraldComponent.HealthComponent.StartingHealth.ToString();
			HealthProgressBar.UpdateBar(EmeraldComponent.HealthComponent.CurrentHealth, 0, EmeraldComponent.HealthComponent.StartHealth);
		}
    }
}