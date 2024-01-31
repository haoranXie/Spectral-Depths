using UnityEngine;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// This component, on Awake or on demand, will force a SaveLoadMethod on the PLSaveLoadManager, changing the way it saves data to file.
	/// This will impact all classes that use the PLSaveLoadManager (unless they change that method before saving or loading).
	/// If you change the method, your previously existing data files won't be compatible, you'll need to delete them and start with new ones.
	/// </summary>
	public class PLSaveLoadManagerMethod : MonoBehaviour
	{
		[Header("Save and load method")]
		[PLInformation("This component, on Awake or on demand, will force a SaveLoadMethod on the PLSaveLoadManager, changing the way it saves data to file. " +
		               "This will impact all classes that use the PLSaveLoadManager (unless they change that method before saving or loading)." +
		               "If you change the method, your previously existing data files won't be compatible, you'll need to delete them and start with new ones.", 
						PLInformationAttribute.InformationType.Info,false)]

		/// the method to use to save to file
		[Tooltip("the method to use to save to file")]
		public PLSaveLoadManagerMethods SaveLoadMethod = PLSaveLoadManagerMethods.Binary;
		/// the key to use to encrypt the file (if using an encryption method)
		[Tooltip("the key to use to encrypt the file (if using an encryption method)")]
		public string EncryptionKey = "ThisIsTheKey";

		protected IMMSaveLoadManagerMethod _saveLoadManagerMethod;

		/// <summary>
		/// On Awake, we set the PLSaveLoadManager's method to the chosen one
		/// </summary>
		protected virtual void Awake()
		{
			SetSaveLoadMethod();
		}
		
		/// <summary>
		/// Creates a new PLSaveLoadManagerMethod and passes it to the PLSaveLoadManager
		/// </summary>
		public virtual void SetSaveLoadMethod()
		{
			switch(SaveLoadMethod)
			{
				case PLSaveLoadManagerMethods.Binary:
					_saveLoadManagerMethod = new PLSaveLoadManagerMethodBinary();
					break;
				case PLSaveLoadManagerMethods.BinaryEncrypted:
					_saveLoadManagerMethod = new PLSaveLoadManagerMethodBinaryEncrypted();
					((PLSaveLoadManagerEncrypter)_saveLoadManagerMethod).Key = EncryptionKey;
					break;
				case PLSaveLoadManagerMethods.Json:
					_saveLoadManagerMethod = new PLSaveLoadManagerMethodJson();
					break;
				case PLSaveLoadManagerMethods.JsonEncrypted:
					_saveLoadManagerMethod = new PLSaveLoadManagerMethodJsonEncrypted();
					((PLSaveLoadManagerEncrypter)_saveLoadManagerMethod).Key = EncryptionKey;
					break;
			}
			PLSaveLoadManager.SaveLoadMethod = _saveLoadManagerMethod;
		}
	}    
}

