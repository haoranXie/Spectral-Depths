using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

namespace SpectralDepths.Feedbacks
{
	/// <summary>
	/// This class is used to automatically install optional dependencies used in PLFeedbacks
	/// </summary>
	public static class FeedbackListOutputer 
	{
		/// <summary>
		/// Outputs a list of all PLFeedbacks to the console (there's only one target user for this and it's me hello!)
		/// </summary>
		[MenuItem("Tools/Spectral Depths/PLFeedbacks/Output PLFeedbacks list", false, 704)]
		public static void OutputFeedbacksList()
		{
			// Retrieve available feedbacks
			List<System.Type> types = (from domainAssembly in System.AppDomain.CurrentDomain.GetAssemblies()
				from assemblyType in domainAssembly.GetTypes()
				where assemblyType.IsSubclassOf(typeof(PLFeedback))
				select assemblyType).ToList();
            
			List<string> typeNames = new List<string>();


			string previousType = "";
			for (int i = 0; i < types.Count; i++)
			{
				PLFeedbacksEditor.FeedbackTypePair newType = new PLFeedbacksEditor.FeedbackTypePair();
				newType.FeedbackType = types[i];
				newType.FeedbackName = FeedbackPathAttribute.GetFeedbackDefaultPath(types[i]);
				if (newType.FeedbackName == "PLFeedbackBase")
				{
					continue;
				}

				string newEntry = FeedbackPathAttribute.GetFeedbackDefaultPath(newType.FeedbackType);
				typeNames.Add(newEntry);
			}
            
			typeNames.Sort();
			StringBuilder builder = new StringBuilder();
			int counter = 1;
			foreach (string typeName in typeNames)
			{
				if (typeName == null)
				{
					continue;
				}
				
				string[] splitArray =  typeName.Split(char.Parse("/"));
                
				if ((previousType != splitArray[0]) && (counter > 1))
				{
					builder.Append("\n");
				}

				builder.Append("- [ ] ");
				builder.Append(counter.ToString("000"));
				builder.Append(" - ");
				builder.Append(typeName);
				builder.Append("\n");

				previousType = splitArray[0];
				counter++;
			}
			Debug.Log(builder.ToString());
		}
        
		/// <summary>
		/// Outputs a list of all PLFeedbacks to the console (there's only one target user for this and it's me hello!)
		/// </summary>
		[MenuItem("Tools/Spectral Depths/PLFeedbacks/Output PLF_Feedbacks list", false, 705)]
		public static void OutputIFeedbacksList()
		{
			// Retrieve available feedbacks
			List<System.Type> types = (from domainAssembly in System.AppDomain.CurrentDomain.GetAssemblies()
				from assemblyType in domainAssembly.GetTypes()
				where assemblyType.IsSubclassOf(typeof(PLF_Feedback))
				select assemblyType).ToList();
            
			List<string> typeNames = new List<string>();


			string previousType = "";
			for (int i = 0; i < types.Count; i++)
			{
				PLFeedbacksEditor.FeedbackTypePair newType = new PLFeedbacksEditor.FeedbackTypePair();
				newType.FeedbackType = types[i];
				newType.FeedbackName = FeedbackPathAttribute.GetFeedbackDefaultPath(types[i]);
				if (newType.FeedbackName == "PLF_FeedbackBase")
				{
					continue;
				}

				string newEntry = FeedbackPathAttribute.GetFeedbackDefaultPath(newType.FeedbackType);
				typeNames.Add(newEntry);
			}
            
			typeNames.Sort();
			StringBuilder builder = new StringBuilder();
			int counter = 1;
			foreach (string typeName in typeNames)
			{
				if (typeName == null)
				{
					continue;
				}
				string[] splitArray =  typeName.Split(char.Parse("/"));
                
				if ((previousType != splitArray[0]) && (counter > 1))
				{
					builder.Append("\n");
				}
                
				builder.Append("- [ ] ");
				builder.Append(counter.ToString("000"));
				builder.Append(" - ");
				builder.Append(typeName);
				builder.Append("\n");

				previousType = splitArray[0];
				counter++;
			}
			Debug.Log(builder.ToString());
		}
	}    
}