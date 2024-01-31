using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

//Logic controller for a DialogueContainer's speaker name field. Control visibility and other logic independently
public class NameContainer : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI nameText;

    public void show(string nameToShow = "")
    {
        root.SetActive(true);

        if (nameToShow != string.Empty)
            nameText.text = nameToShow;
    }

    public void hide()
    {
        root.SetActive(false);
    }
}
