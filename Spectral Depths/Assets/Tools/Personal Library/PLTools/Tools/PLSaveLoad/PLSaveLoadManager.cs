using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SpectralDepths.Tools
{
	/// <summary>
	/// Allows the save and load of objects in a specific folder and file.
	/// 
	/// How to use (at a minimum) :
	/// 
	/// Save : PLSaveLoadManager.Save(TestObject, FileName+SaveFileExtension, FolderName);
	/// 
	/// Load : TestObject = (YourObjectClass)PLSaveLoadManager.Load(typeof(YourObjectClass), FileName + SaveFileExtension, FolderName);
	/// 
	/// Delete save : PLSaveLoadManager.DeleteSave(FileName+SaveFileExtension, FolderName);
	/// 
	/// Delete save folder : PLSaveLoadManager.DeleteSaveFolder(FolderName);
	/// 
	/// You can also specify what IMMSaveLoadManagerMethod the system should use. By default it's binary but you can also pick binary encrypted, json, or json encrypted
	/// You'll find examples of how to set each of these in the PLSaveLoadTester class
	/// 
	/// </summary>
	public static class PLSaveLoadManager
	{
		/// the method to use when saving and loading files (has to be the same at both times of course)
		public static IMMSaveLoadManagerMethod SaveLoadMethod = new PLSaveLoadManagerMethodBinary();
		/// the default top level folder the system will use to save the file
		private const string _baseFolderName = "/PLData/";
		/// the name of the save folder if none is provided
		private const string _defaultFolderName = "PLSaveLoadManager";

		/// <summary>
		/// Determines the save path to use when loading and saving a file based on a folder name.
		/// </summary>
		/// <returns>The save path.</returns>
		/// <param name="folderName">Folder name.</param>
		static string DetermineSavePath(string folderName = _defaultFolderName)
		{
			string savePath;
			// depending on the device we're on, we assemble the path
			if (Application.platform == RuntimePlatform.IPhonePlayer) 
			{
				savePath = Application.persistentDataPath + _baseFolderName;
			} 
			else 
			{
				savePath = Application.persistentDataPath + _baseFolderName;
			}
			#if UNITY_EDITOR
			savePath = Application.dataPath + _baseFolderName;
			#endif

			savePath = savePath + folderName + "/";
			return savePath;
		}

		/// <summary>
		/// Determines the name of the file to save
		/// </summary>
		/// <returns>The save file name.</returns>
		/// <param name="fileName">File name.</param>
		static string DetermineSaveFileName(string fileName)
		{
			return fileName;
		}

		/// <summary>
		/// Save the specified saveObject, fileName and foldername into a file on disk.
		/// </summary>
		/// <param name="saveObject">Save object.</param>
		/// <param name="fileName">File name.</param>
		/// <param name="foldername">Foldername.</param>
		public static void Save(object saveObject, string fileName, string foldername = _defaultFolderName)
		{
			string savePath = DetermineSavePath(foldername);
			string saveFileName = DetermineSaveFileName(fileName);
			// if the directory doesn't already exist, we create it
			if (!Directory.Exists(savePath))
			{
				Directory.CreateDirectory(savePath);
			}
			// we serialize and write our object into a file on disk

			FileStream saveFile = File.Create(savePath + saveFileName);

			SaveLoadMethod.Save(saveObject, saveFile);
			saveFile.Close();
		}

		/// <summary>
		/// Load the specified file based on a file name into a specified folder
		/// </summary>
		/// <param name="fileName">File name.</param>
		/// <param name="foldername">Foldername.</param>
		public static object Load(System.Type objectType, string fileName, string foldername = _defaultFolderName)
		{
			string savePath = DetermineSavePath(foldername);
			string saveFileName = savePath + DetermineSaveFileName(fileName);

			object returnObject;

			// if the PLSaves directory or the save file doesn't exist, there's nothing to load, we do nothing and exit
			if (!Directory.Exists(savePath) || !File.Exists(saveFileName))
			{
				return null;
			}

			FileStream saveFile = File.Open(saveFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			returnObject = SaveLoadMethod.Load(objectType, saveFile);
			saveFile.Close();

			return returnObject;
		}

		/// <summary>
		/// Removes a save from disk
		/// </summary>
		/// <param name="fileName">File name.</param>
		/// <param name="folderName">Folder name.</param>
		public static void DeleteSave(string fileName, string folderName = _defaultFolderName)
		{
			string savePath = DetermineSavePath(folderName);
			string saveFileName = DetermineSaveFileName(fileName);
			if (File.Exists(savePath + saveFileName))
			{
				File.Delete(savePath + saveFileName);
			}	
			if (File.Exists(savePath + saveFileName + ".meta"))
			{
				File.Delete(savePath + saveFileName + ".meta");
			}			
		}

		/// <summary>
		/// Deletes the whole save folder
		/// </summary>
		/// <param name="folderName"></param>
		public static void DeleteSaveFolder(string folderName = _defaultFolderName)
		{
			string savePath = DetermineSavePath(folderName);
			if (Directory.Exists(savePath))
			{
				DeleteDirectory(savePath);
			}
		}
		
		/// <summary>
		/// Deletes all save files saved by this PLSaveLoadManager
		/// </summary>
		public static void DeleteAllSaveFiles()
		{
			string savePath = DetermineSavePath("");

			savePath = savePath.Substring(0, savePath.Length - 1);
			if (savePath.EndsWith("/"))
			{
				savePath = savePath.Substring(0, savePath.Length - 1);
			}

			if (Directory.Exists(savePath))
			{
				DeleteDirectory(savePath);
			}
		}

		/// <summary>
		/// Deletes the specified directory
		/// </summary>
		/// <param name="target_dir"></param>
		public static void DeleteDirectory(string target_dir)
		{
			string[] files = Directory.GetFiles(target_dir);
			string[] dirs = Directory.GetDirectories(target_dir);

			foreach (string file in files)
			{
				File.SetAttributes(file, FileAttributes.Normal);
				File.Delete(file);
			}

			foreach (string dir in dirs)
			{
				DeleteDirectory(dir);
			}

			Directory.Delete(target_dir, false);

			if (File.Exists(target_dir + ".meta"))
			{
				File.Delete(target_dir + ".meta");
			}
		}
	}
}