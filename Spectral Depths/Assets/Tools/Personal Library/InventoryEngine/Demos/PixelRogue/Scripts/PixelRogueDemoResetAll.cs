using System.Collections;
using System.Collections.Generic;
using SpectralDepths.Tools;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpectralDepths.InventoryEngine
{
	/// <summary>
	/// A very small class used to reset inventories and persistence data in the PixelRogue demos
	/// </summary>
	public class PixelRogueDemoResetAll : MonoBehaviour
	{
		const string _inventorySaveFolderName = "InventoryEngine"; 
		
		public virtual void ResetAll()
		{
			// we delete the save folder for inventories
			PLSaveLoadManager.DeleteSaveFolder (_inventorySaveFolderName);
			// we delete our persistence data
			PLPersistenceManager.Instance.ResetPersistence();
			// we reload the scene
			SceneManager.LoadScene("PixelRogueRoom1");
		}
	}	
}

