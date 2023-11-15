using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private const string START = "Start";

    [SerializeField] private Animator transition;
    [SerializeField] private float transitionTime = 1f;

    public enum Scene{
        MainMenuScene,
        LoadingScene,
        SpireLobby,
        TestInterior
    }
    public void Load(Scene targetScene){
        StartCoroutine(LoadLevel(targetScene));
    }
    IEnumerator LoadLevel(Scene scene){
        //Play Animation
        transition.SetTrigger(START);
        yield return new WaitForSeconds(transitionTime);
       SceneManager.LoadScene(scene.ToString());
    }
}
