using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private LevelLoader levelLoader;


    private void Awake(){
        levelLoader.gameObject.SetActive(false);
        playButton.onClick.AddListener(()=>{
            levelLoader.gameObject.SetActive(true);
            levelLoader.Load(LevelLoader.Scene.SpireLobby);
        });
        quitButton.onClick.AddListener(()=>{
            Application.Quit();
        });

        Time.timeScale=1f; //Unpauses Game
    }

}
