﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using SpectralDepths.Tools;

namespace SpectralDepths.InventoryEngine
{	
	/// <summary>
	/// Demo class to go from one level to another
	/// </summary>
	public class ChangeLevel : MonoBehaviour 
	{
		/// The exact name of the scene to go to when entering the ChangeLevel zone
		[PLInformation("This demo component, when added to a BoxCollider2D, will change the scene to the one specified in the field below when the character enters the collider.", PLInformationAttribute.InformationType.Info,false)]
		[Tooltip("The exact name of the scene to go to when entering the ChangeLevel zone")]
		public string Destination;

		/// <summary>
		/// When a character enters the ChangeLevel zone, we trigger a general save and then load the destination scene
		/// </summary>
		/// <param name="collider">Collider.</param>
		public virtual void OnTriggerEnter2D (Collider2D collider) 
		{
			if ((Destination != null) && (collider.gameObject.GetComponent<InventoryDemoCharacter>() != null))
			{
				PLGameEvent.Trigger("Save");
				SceneManager.LoadScene(Destination);
			}
		}
	}
}