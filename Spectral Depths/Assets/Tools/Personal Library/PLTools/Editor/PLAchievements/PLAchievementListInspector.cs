using UnityEngine;
using System.Collections;
using SpectralDepths.Tools;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UI;

namespace SpectralDepths.Tools
{
	[CustomEditor(typeof(PLAchievementList),true)]
	/// <summary>
	/// Custom inspector for the PLAchievementList scriptable object. 
	/// </summary>
	public class PLAchievementListInspector : Editor 
	{
		/// <summary>
		/// When drawing the GUI, adds a "Reset Achievements" button, that does exactly what you think it does.
		/// </summary>
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector ();
			PLAchievementList achievementList = (PLAchievementList)target;
			if(GUILayout.Button("Reset Achievements"))
			{
				achievementList.ResetAchievements();
			}	
			EditorUtility.SetDirty (achievementList);
		}
	}
}