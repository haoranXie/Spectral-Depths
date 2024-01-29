using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using Object = UnityEngine.Object;


namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// A helper class to copy and paste feedback properties
	/// </summary>
	static class PLF_PlayerCopy
	{
		// Single Copy --------------------------------------------------------------------

		static public System.Type Type { get; private set; }
		static List<SerializedProperty> Properties = new List<SerializedProperty>();
        
		public static readonly List<PLF_Feedback> CopiedFeedbacks = new List<PLF_Feedback>();

		public static List<PLF_Player> ShouldKeepChanges = new List<PLF_Player>();

		static string[] IgnoreList = new string[]
		{
			"m_ObjectHideFlags",
			"m_CorrespondingSourceObject",
			"m_PrefabInstance",
			"m_PrefabAsset",
			"m_GameObject",
			"m_Enabled",
			"m_EditorHideFlags",
			"m_Script",
			"m_Name",
			"m_EditorClassIdentifier"
		};

		static public bool HasCopy()
		{
			return CopiedFeedbacks != null && CopiedFeedbacks.Count == 1;
		}

		static public bool HasMultipleCopies()
		{
			return CopiedFeedbacks != null && CopiedFeedbacks.Count > 1;
		}

		static public void Copy(PLF_Feedback feedback)
		{
			Type feedbackType = feedback.GetType();
			PLF_Feedback newFeedback = (PLF_Feedback)Activator.CreateInstance(feedbackType);
			EditorUtility.CopySerializedManagedFieldsOnly(feedback, newFeedback);
			CopiedFeedbacks.Clear();
			CopiedFeedbacks.Add(newFeedback);
		}
        
		static public void CopyAll(PLF_Player sourceFeedbacks)
		{
			CopiedFeedbacks.Clear();
			foreach (PLF_Feedback feedback in sourceFeedbacks.FeedbacksList)
			{
				Type feedbackType = feedback.GetType();
				PLF_Feedback newFeedback = (PLF_Feedback)Activator.CreateInstance(feedbackType);
				EditorUtility.CopySerializedManagedFieldsOnly(feedback, newFeedback);
				CopiedFeedbacks.Add(newFeedback);    
			}
		}

		// Multiple Copy ----------------------------------------------------------


		static public void PasteAll(PLF_PlayerEditor targetEditor)
		{
			foreach (PLF_Feedback feedback in PLF_PlayerCopy.CopiedFeedbacks)
			{
				targetEditor.TargetMmfPlayer.AddFeedback(feedback);
			}
			CopiedFeedbacks.Clear();
		}
	}
}