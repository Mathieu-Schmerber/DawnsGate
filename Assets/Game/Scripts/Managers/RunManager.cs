using Game.Systems.Run;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Game.Managers
{
	public enum RunState
	{
		LOBBY,
		IN_RUN
	}

	public class RunManager : ManagerSingleton<RunManager>
	{
		public static RunSettingsData RunSettings => Databases.Database.Data.Run.Settings;

		#region Properties

		private string _currentRoomScene;
		private int _currentRoom;
		private Room _room;
		private RunState _runState = RunState.LOBBY;
		private Dictionary<RoomType, string[]> _scenes = new();

		public static Room CurrentRoom => Instance._room;
		public static bool IsLastRoom => Instance._currentRoom == RunSettings.RoomRules.Length - 1;
		public static string CurrentRoomScene;
		public static RunState RunState => Instance._runState;

		#endregion

		private void Awake()
		{
			_scenes.Add(RoomType.COMBAT, Directory.GetFiles(RunSettings.RoomFolders[RoomType.COMBAT].Folder, "*.unity"));
			_scenes.Add(RoomType.EVENT, Directory.GetFiles(RunSettings.RoomFolders[RoomType.EVENT].Folder, "*.unity"));
			_scenes.Add(RoomType.SHOP, Directory.GetFiles(RunSettings.RoomFolders[RoomType.SHOP].Folder, "*.unity"));
			_scenes.Add(RoomType.UPGRADE, Directory.GetFiles(RunSettings.RoomFolders[RoomType.UPGRADE].Folder, "*.unity"));
			_scenes.Add(RoomType.LIFE_SHOP, Directory.GetFiles(RunSettings.RoomFolders[RoomType.LIFE_SHOP].Folder, "*.unity"));
			_scenes.Add(RoomType.BOSS, Directory.GetFiles(RunSettings.RoomFolders[RoomType.BOSS].Folder, "*.unity"));
		}

		#region Tools

		public static void StartNewRun()
		{
			Instance._currentRoom = 0;
			Instance._room = new() {
				Type = RunSettings.FirstRoom.Type,
				Reward = RunSettings.FirstRoom.Reward,
			};
			Instance._currentRoomScene = RunSettings.LobbySceneName;
			Instance._room.DefineExitsFromRule(RunSettings.RoomRules[Instance._currentRoom]);
			Instance._runState = RunState.IN_RUN;

			ChangeScene(RunSettings.FirstRoom.Type);
		}

		public static void SelectNextRoom(Room selected)
		{
			if (IsLastRoom)
				EndRun();
			else
			{
				Instance._currentRoom++;
				Instance._room = new()
				{
					Type = selected.Type,
					Reward = selected.Reward
				};
				Instance._room.DefineExitsFromRule(RunSettings.RoomRules[Instance._currentRoom]);

				ChangeScene(selected.Type);
			}
		}

		private static void EndRun()
		{
			ChangeScene(RunSettings.LobbySceneName);
			Instance._runState = RunState.LOBBY;
		}

		private static string GetRandomRoomScene(RoomType type) => Path.GetFileNameWithoutExtension(Instance._scenes[type].Random());

		private static void ChangeScene(RoomType type)
		{
			string nextScene = GetRandomRoomScene(type);

			ChangeScene(nextScene);
		}

		private static void ChangeScene(string scene)
		{
			SceneManager.UnloadSceneAsync(Instance._currentRoomScene);
			SceneManager.LoadScene(scene, LoadSceneMode.Additive);
			Instance._currentRoomScene = scene;
		}

		#endregion
	}
}
