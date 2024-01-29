using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using SpectralDepths.Tools;
using UnityEngine.SceneManagement;

namespace SpectralDepths.Tools
{	
	/// <summary>
	/// Add this component on an object, specify a scene name in its inspector, and call LoadScene() to load the desired scene.
	/// </summary>
	public class PLLoadScene : MonoBehaviour 
	{
		/// the possible modes to load scenes. Either Unity's native API, or SpectralDepths' LoadingSceneManager
		public enum LoadingSceneModes { UnityNative, PLSceneLoadingManager, PLAdditiveSceneLoadingManager }

		/// the name of the scene that needs to be loaded when LoadScene gets called
		[Tooltip("the name of the scene that needs to be loaded when LoadScene gets called")]
		public string SceneName;
		/// defines whether the scene will be loaded using Unity's native API or SpectralDepths' way
		[Tooltip("defines whether the scene will be loaded using Unity's native API or SpectralDepths' way")]
		public LoadingSceneModes LoadingSceneMode = LoadingSceneModes.UnityNative;

		/// <summary>
		/// Loads the scene specified in the inspector
		/// </summary>
		public virtual void LoadScene()
		{
			switch (LoadingSceneMode)
			{
				case LoadingSceneModes.UnityNative:
					SceneManager.LoadScene (SceneName);
					break;
				case LoadingSceneModes.PLSceneLoadingManager:
					PLSceneLoadingManager.LoadScene (SceneName);
					break;
				case LoadingSceneModes.PLAdditiveSceneLoadingManager:
					PLAdditiveSceneLoadingManager.LoadScene(SceneName);
					break;
			}
		}
	}
}