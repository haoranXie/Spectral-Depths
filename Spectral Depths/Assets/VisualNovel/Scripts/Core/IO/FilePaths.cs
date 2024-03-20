using UnityEngine;

public class FilePaths
{
    private const string HOME_DIRECTORY_SYMBOL = "~/";

    public static readonly string root = $"{Application.dataPath}/gameData/";

    //Resources Paths
    public static readonly string resources_graphics = "Graphics/";
    public static readonly string resources_backgroundImages = $"{resources_graphics}BG Images/";
    public static readonly string resources_backgroundVideos = $"{resources_graphics}BG Videos/";
    public static readonly string resources_blendTextures = $"{resources_graphics}Transition Effects/";

    public static readonly string resources_audio = "Audio/";
    public static readonly string resources_sfx = $"{resources_audio}SFX/";
    public static readonly string resources_voices = $"{resources_audio}Voices/";
    public static readonly string resources_music = $"{resources_audio}Music/";
    public static readonly string resources_ambience = $"{resources_audio}Ambience/";


    /// <summary>
    /// Returns the path to the resource using the default path or the root of the resources folder if the resource name begins with the Home Directory Symbol
    /// </summary>
    /// <param name="defaultPath"></param>
    /// <param name="resourceName"></param>
    /// <returns></returns>
    public static string GetPathToResource(string defaultPath, string resourceName)
    {
        if (resourceName.StartsWith(HOME_DIRECTORY_SYMBOL))
            return resourceName.Substring(HOME_DIRECTORY_SYMBOL.Length);

        return defaultPath + resourceName;
    }
}
