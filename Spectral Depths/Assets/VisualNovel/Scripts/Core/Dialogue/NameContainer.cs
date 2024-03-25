using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

namespace DIALOGUE
{
    [System.Serializable]
    /// <summary>
    /// The box that holds the name text on screen. Part of the dialogue container.
    /// </summary>
    public class NameContainer
    {
        [SerializeField] private GameObject root;
        [SerializeField] private TextMeshProUGUI nameText;

        public void Show(string nameToShow = "")
        {
            root.SetActive(true);

            if (nameToShow != string.Empty)
                nameText.text = nameToShow;
        }

        public void Hide()
        {
            root.SetActive(false);
        }

        public void SetNameColor(Color color) => nameText.color = color;
        public void SetnameFont(TMP_FontAsset font) => nameText.font = font;
        public void SetNameFontSize(float size) => nameText.fontSize = size;
    }
}