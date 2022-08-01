using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace Nawlian.Lib.Systems.Saving
{
	public static class SaveSystem
	{
		private static string _savePath => $"{Application.persistentDataPath}/save.data";

#if UNITY_EDITOR

		[MenuItem("Tools/Game/Wipe save files")]
		public static void WipeSaveFile()
		{
			Wipe();
			Debug.Log("The save files of the game were wiped.");
		}

#endif

		/// <summary>
		/// Saves the scene current state. <br/>
		/// Calling Save() will cause the system to iterate over every SaveableEntity component. <br/>
		/// Can get slow, do not call it to often.
		/// </summary>
		public static void Save()
		{
			var state = LoadFile();

			SaveSceneState(state);
			SaveFile(state);
			Debug.Log($"SaveSystem - Game Saved {_savePath}");
		}

		/// <summary>
		/// Loads the scene from the save file. <br/>
		/// Calling Load() will cause the system to iterate over every SaveableEntity component. <br/>
		/// Can get slow, do not call it to often.
		/// </summary>
		public static void Load()
		{
			var state = LoadFile();

			LoadSceneState(state);
			Debug.Log($"SaveSystem - Game Loaded {_savePath}");
		}

		/// <summary>
		/// Wipes the save file, losing all of the saved progression.
		/// </summary>
		public static void Wipe()
		{
			if (File.Exists(_savePath))
				File.Delete(_savePath);
		}

		private static void SaveFile(object state)
		{
			using (var stream = File.Open(_savePath, FileMode.Create))
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, state);
			}
		}

		private static Dictionary<string, object> LoadFile()
		{
			if (!File.Exists(_savePath))
				return new Dictionary<string, object>();

			using (var stream = File.Open(_savePath, FileMode.Open))
			{
				var formatter = new BinaryFormatter();
				return (Dictionary<string, object>)formatter.Deserialize(stream);
			}
		}

		private static void SaveSceneState(Dictionary<string, object> state)
		{
			foreach (var saveableEntity in GameObject.FindObjectsOfType<SaveableEntity>())
				state[saveableEntity.Id] = saveableEntity.Save();
		}

		private static void LoadSceneState(Dictionary<string, object> state)
		{
			foreach (var saveableEntity in GameObject.FindObjectsOfType<SaveableEntity>())
			{
				if (state.TryGetValue(saveableEntity.Id, out object value))
				{
					saveableEntity.Load(value);
				}
			}
		}
	}
}