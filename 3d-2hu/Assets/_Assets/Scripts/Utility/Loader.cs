using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Use to load between scenes
/// </summary>
public static class Loader
{

    public enum Scene{
        MainMenuScene,
        LoadingScene,
        SpireLobby,
        TestInterior
    }

    private static Scene targetScene;

    public static void Load(Scene targetScene){
        Loader.targetScene=targetScene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
        SceneManager.LoadScene(targetScene.ToString());
    }

    public static void LoaderCallback(){
        SceneManager.LoadScene(targetScene.ToString());
    }
}
