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
	public class PLFeedbacksConfiguration : ScriptableObject
	{
		private static PLFeedbacksConfiguration _instance;
		private static bool _instantiated;
        
		/// <summary>
		/// Singleton pattern
		/// </summary>
		public static PLFeedbacksConfiguration Instance
		{
			get
			{
				if (_instantiated)
				{
					return _instance;
				}
                
				string assetName = typeof(PLFeedbacksConfiguration).Name;
                
				PLFeedbacksConfiguration loadedAsset = Resources.Load<PLFeedbacksConfiguration>("PLFeedbacksConfiguration");
				_instantiated = true;
				_instance = loadedAsset;
                
				return _instance;
			}
		}

		[Header("Debug")]
		/// storage for copy/paste
		public PLFeedbacks _mmFeedbacks;
        
		[Header("Help settings")]
		/// if this is true, inspector tips will be shown for PLFeedbacks
		public bool ShowInspectorTips = true;
        
		private void OnDestroy(){ _instantiated = false; }
	}    
}