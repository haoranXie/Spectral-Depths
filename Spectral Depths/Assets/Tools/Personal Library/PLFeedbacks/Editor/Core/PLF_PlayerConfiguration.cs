using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// An asset to store copy information, as well as global feedback settings.
	/// It requires that one (and only one) PLFeedbacksConfiguration asset be created and stored in a Resources folder.
	/// That's already done when installing PLFeedbacks.
	/// </summary>
	[CreateAssetMenu(menuName = "SpectralDepths/PLFeedbacks/Configuration", fileName = "PLFeedbacksConfiguration")]
	public class PLF_PlayerConfiguration : ScriptableObject
	{
		private static PLF_PlayerConfiguration _instance;
		private static bool _instantiated;
        
		/// <summary>
		/// Singleton pattern
		/// </summary>
		public static PLF_PlayerConfiguration Instance
		{
			get
			{
				if (_instantiated)
				{
					return _instance;
				}
                
				string assetName = typeof(PLF_PlayerConfiguration).Name;
                
				PLF_PlayerConfiguration loadedAsset = Resources.Load<PLF_PlayerConfiguration>("PLF_PlayerConfiguration");
				_instance = loadedAsset;    
				_instantiated = true;
                
				return _instance;
			}
		}
        
		[Header("Help settings")]
		/// if this is true, inspector tips will be shown for PLFeedbacks
		public bool ShowInspectorTips = true;
		/// if this is true, when exiting play mode when KeepPlaymodeChanges is active, it'll turn off automatically, otherwise it'll remain on
		public bool AutoDisableKeepPlaymodeChanges = true;
		/// if this is true, when exiting play mode when KeepPlaymodeChanges is active, it'll turn off automatically, otherwise it'll remain on
		public bool InspectorGroupsExpandedByDefault = true;


        
		private void OnDestroy(){ _instantiated = false; }
	}    
}