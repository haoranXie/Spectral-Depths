using System;
using System.Collections;
using System.Collections.Generic;
using SpectralDepths.TopDown;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using SpectralDepths.Tools;

namespace COMMANDS
{
    public class CMD_DatabaseExtension_Scene : CMD_DatabaseExtension
    {
        private static string[] PARAM_TARGETSCENE = new string[] { "-n", "-name" };
        private static string[] PARAM_LOADINGSCENE = new string[] { "-l", "-load", "-loading" };

        new public static void Extend(CommandDatabase database)
        {
            database.AddCommand("loadscene", new Action<string[]>(LoadNewScene));
            database.AddCommand("switchtogamemode", new Action<string[]>(SwitchToGameMode));
        }

        private static void LoadNewScene(string[] data)
        {
            string targetSceneName;
            string loadingSceneName;

            var parameters = ConvertDataToParameters(data);

            parameters.TryGetValue(PARAM_TARGETSCENE, out targetSceneName);
            parameters.TryGetValue(PARAM_LOADINGSCENE, out loadingSceneName);

            //If we do not have a custom loading scene we use the default
            if(loadingSceneName==null)
            {
                PLAdditiveSceneLoadingManager.LoadScene(targetSceneName);  
            }
            //Otherwise we use the custom  loading scene
            else
            {
                PLAdditiveSceneLoadingManager.LoadScene(targetSceneName,loadingSceneName);  
            }
        }

        private static void SwitchToGameMode(string[] data)
        {
            TopDownEngineEvent.Trigger(TopDownEngineEventTypes.SwitchToGameMode, null);
        }

    }
}
