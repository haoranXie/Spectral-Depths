using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SpectralDepths.Tools;
using UnityEngine.EventSystems;

namespace SpectralDepths.TopDown
{
    public class CityMapUI : PLSingleton<CityMapUI> 
    {
            /// the main canvas
        [Tooltip("the main canvas")]
        public Canvas MainCanvas;

        [Tooltip("Spire Menu")]
        public Transform SpireMenu;

        public void ShowSpireCanvas()
        {
            SpireMenu.GetComponent<Animator>().SetTrigger("SlideIn");
        }

        public void HideSpireCanvas()
        {
            SpireMenu.GetComponent<Animator>().SetTrigger("SlideOut");
        }
    }
}
