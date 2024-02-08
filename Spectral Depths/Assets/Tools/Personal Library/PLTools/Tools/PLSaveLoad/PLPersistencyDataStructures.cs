using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace SpectralDepths.Tools
{
	/// <summary>
	/// A serializable class used to store scene data, the key is a string (the scene name), the value is a PLPersistencySceneData
	/// </summary>
	[Serializable]
	public class DictionaryStringSceneData : PLSerializableDictionary<string, PLPersistenceSceneData>
	{
		public DictionaryStringSceneData() : base() { }
		protected DictionaryStringSceneData(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	/// <summary>
	/// A serializable class used to store object data, the key is a string (the object name), the value is a string (the object data)
	/// </summary>
	[Serializable]
	public class DictionaryStringString : PLSerializableDictionary<string, string>
	{
		public DictionaryStringString() : base() { }
		protected DictionaryStringString(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
	
	/// <summary>
	/// A serializable class used to store all the data for a persistence manager, a collection of scene datas
	/// </summary>
	[Serializable]
	public class PLPersistenceManagerData
	{
		public string PersistenceID;
		public string SaveDate;
		public DictionaryStringSceneData SceneDatas;
	}
	
	/// <summary>
	/// A serializable class used to store all the data for a scene, a collection of object datas
	/// </summary>
	[Serializable]
	public class PLPersistenceSceneData
	{
		public DictionaryStringString ObjectDatas;
	}
	
	/// <summary>
	/// The various types of persistence events that can be triggered by the PLPersistencyManager
	/// </summary>
	public enum PLPersistenceEventType { DataSavedToMemory, DataLoadedFromMemory, DataSavedFromMemoryToFile, DataLoadedFromFileToMemory }

	/// <summary>
	/// A data structure used to store persistence event data.
	/// To use :
	/// PLPersistencyEvent.Trigger(PLPersistencyEventType.DataLoadedFromFileToMemory, "yourPersistencyID");
	/// </summary>
	public struct PLPersistenceEvent
	{
		public PLPersistenceEventType PersistenceEventType;
		public string PersistenceID;

		public PLPersistenceEvent(PLPersistenceEventType eventType, string persistenceID)
		{
			PersistenceEventType = eventType;
			PersistenceID = persistenceID;
		}

		static PLPersistenceEvent e;
		public static void Trigger(PLPersistenceEventType eventType, string persistencyID)
		{
			e.PersistenceEventType = eventType;
			e.PersistenceID = persistencyID;
		}
	}
}
