using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A test object to store data to test the PLSaveLoadManager class
	/// </summary>
	[System.Serializable]
	public class PLSaveLoadTestObject
	{
		public string SavedText;
	}

	/// <summary>
	/// A simple class used in the PLSaveLoadTestScene to test the PLSaveLoadManager class
	/// </summary>
	public class PLSaveLoadTester : MonoBehaviour
	{
		[Header("Bindings")]
		/// the text to save
		[Tooltip("the text to save")]
		public InputField TargetInputField;

		[Header("Save settings")]
		/// the chosen save method (json, encrypted json, binary, encrypted binary)
		[Tooltip("the chosen save method (json, encrypted json, binary, encrypted binary)")]
		public PLSaveLoadManagerMethods SaveLoadMethod = PLSaveLoadManagerMethods.Binary;
		/// the name of the file to save
		[Tooltip("the name of the file to save")]
		public string FileName = "TestObject";
		/// the name of the destination folder
		[Tooltip("the name of the destination folder")]
		public string FolderName = "PLTest/";
		/// the extension to use
		[Tooltip("the extension to use")]
		public string SaveFileExtension = ".testObject";
		/// the key to use to encrypt the file (if needed)
		[Tooltip("the key to use to encrypt the file (if needed)")]
		public string EncryptionKey = "ThisIsTheKey";

		/// Test button
		[PLInspectorButton("Save")]
		public bool TestSaveButton;
		/// Test button
		[PLInspectorButton("Load")]
		public bool TestLoadButton;
		/// Test button
		[PLInspectorButton("Reset")]
		public bool TestResetButton;

		protected IMMSaveLoadManagerMethod _saveLoadManagerMethod;

		/// <summary>
		/// Saves the contents of the TestObject into a file
		/// </summary>
		public virtual void Save()
		{
			InitializeSaveLoadMethod();
			PLSaveLoadTestObject testObject = new PLSaveLoadTestObject();
			testObject.SavedText = TargetInputField.text;
			PLSaveLoadManager.Save(testObject, FileName+SaveFileExtension, FolderName);
		}

		/// <summary>
		/// Loads the saved data
		/// </summary>
		public virtual void Load()
		{
			InitializeSaveLoadMethod();
			PLSaveLoadTestObject testObject = (PLSaveLoadTestObject)PLSaveLoadManager.Load(typeof(PLSaveLoadTestObject), FileName + SaveFileExtension, FolderName);
			TargetInputField.text = testObject.SavedText;
		}

		/// <summary>
		/// Resets all saves by deleting the whole folder
		/// </summary>
		protected virtual void Reset()
		{
			PLSaveLoadManager.DeleteSaveFolder(FolderName);
		}

		/// <summary>
		/// Creates a new PLSaveLoadManagerMethod and passes it to the PLSaveLoadManager
		/// </summary>
		protected virtual void InitializeSaveLoadMethod()
		{
			switch(SaveLoadMethod)
			{
				case PLSaveLoadManagerMethods.Binary:
					_saveLoadManagerMethod = new PLSaveLoadManagerMethodBinary();
					break;
				case PLSaveLoadManagerMethods.BinaryEncrypted:
					_saveLoadManagerMethod = new PLSaveLoadManagerMethodBinaryEncrypted();
					(_saveLoadManagerMethod as PLSaveLoadManagerEncrypter).Key = EncryptionKey;
					break;
				case PLSaveLoadManagerMethods.Json:
					_saveLoadManagerMethod = new PLSaveLoadManagerMethodJson();
					break;
				case PLSaveLoadManagerMethods.JsonEncrypted:
					_saveLoadManagerMethod = new PLSaveLoadManagerMethodJsonEncrypted();
					(_saveLoadManagerMethod as PLSaveLoadManagerEncrypter).Key = EncryptionKey;
					break;
			}
			PLSaveLoadManager.SaveLoadMethod = _saveLoadManagerMethod;
		}
	}
}