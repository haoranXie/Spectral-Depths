using System.Collections;
using System.Collections.Generic;
using Unity.Transforms;
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
        GameScene
    }

    private static Scene targetScene;

    private static void Load(Scene targetScene){
        Loader.targetScene=targetScene;
        SceneManager.LoadScene(Scene.LoadingScene.ToString());
        SceneManager.LoadScene(targetScene.ToString());
    }

    public static void LoaderCallback(){
        SceneManager.LoadScene(targetScene.ToString());
    }
}
