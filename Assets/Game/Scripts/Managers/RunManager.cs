using Game.Entities.Player;
using Game.Systems.Run;
using Game.Systems.Run.Rooms;
using Game.UI;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Pooling;
using Nawlian.Lib.Systems.Saving;
using Nawlian.Lib.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Managers
{
	public enum RunState
	{
		NONE,
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
		private RunState _runState = RunState.NONE;
		private Dictionary<RoomType, string[]> _scenes = new();
		private PlayerDamageable _damageable;
		[SerializeField] private ARoom _currentInstance;

		public static Room CurrentRoom => Instance._room;
		public static ARoom CurrentRoomInstance { get => Instance._currentInstance; set => Instance._currentInstance = value; }

		public static bool IsLastRoom => Instance._currentRoom == RunSettings.RoomRules.Length - 1;
		public static string CurrentRoomScene;
		public static RunState RunState => Instance._runState;

		public List<string> ReachedRooms = new();

		public static event Action OnRunEnded;
		public static event Action OnBeforeSceneSwitched;
		public static event Action OnSceneSwitched;

		#endregion

		private void Awake()
		{
			_scenes.Add(RoomType.COMBAT, RunSettings.RoomFolders[RoomType.COMBAT].RoomDatas.Select(x => x.SceneName).ToArray());
			_scenes.Add(RoomType.EVENT, RunSettings.RoomFolders[RoomType.EVENT].RoomDatas.Select(x => x.SceneName).ToArray());
			_scenes.Add(RoomType.SHOP, RunSettings.RoomFolders[RoomType.SHOP].RoomDatas.Select(x => x.SceneName).ToArray());
			_scenes.Add(RoomType.LIFE_SHOP, RunSettings.RoomFolders[RoomType.LIFE_SHOP].RoomDatas.Select(x => x.SceneName).ToArray());
			_scenes.Add(RoomType.BOSS, RunSettings.RoomFolders[RoomType.BOSS].RoomDatas.Select(x => x.SceneName).ToArray());
			_damageable = GameManager.Player.GetComponent<PlayerDamageable>();
		}

		private void OnEnable()
		{
			_damageable.OnPlayerDeath += EndRun;
		}

		private void OnDisable()
		{
			_damageable.OnPlayerDeath -= EndRun;
		}

		#region Tools

		public static void OnLobbyEntered()
		{
			Instance._runState = RunState.LOBBY;
			AudioManager.PlayTheme(RunSettings.LobbyTheme);
			SaveSystem.Load();
		}

		public static void StartNewRun()
		{
			SaveSystem.Save();
			AudioManager.PlayTheme(RunSettings.RunTheme);
			CurrentRoomInstance = null;
			Instance.ReachedRooms.Clear();
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

		public static void EndRun()
		{
			ChangeScene(RunSettings.LobbySceneName);
			SaveSystem.Save();
			OnLobbyEntered(); // <= Maybe call this when the scene has been loaded ?
			OnRunEnded?.Invoke();
		}

		private static string GetRandomRoomScene(RoomType type)
		{
			var rooms = Instance._scenes[type];
			var notVisited = rooms.Where(x => !Instance.ReachedRooms.Contains(x)).ToArray();

			if (notVisited.Length == 0)
				return rooms.Random();
			return notVisited.Random();
		}

		private static void ChangeScene(RoomType type)
		{
			string nextScene = GetRandomRoomScene(type);

			ChangeScene(nextScene);
		}

		private static void ChangeScene(string sceneName)
		{
			SceneTransition.StartTransition(() =>
			{
				Instance.ReachedRooms.Add(sceneName);
				ObjectPooler.ReleaseAll();
				SceneManager.UnloadSceneAsync(Instance._currentRoomScene);
				var load = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

				load.completed += OnSceneLoaded;
				Instance._currentRoomScene = sceneName;
			});
		}

		private static void OnSceneLoaded(AsyncOperation obj)
		{
			SceneManager.SetActiveScene(SceneManager.GetSceneByName(Instance._currentRoomScene));
			OnBeforeSceneSwitched?.Invoke();
			Awaiter.WaitAndExecute(1f, () => SceneTransition.EndTransition(() =>
			{
				OnSceneSwitched?.Invoke();
			}));
		}

#if UNITY_EDITOR
		public static void ArtificiallyLaunchScene()
		{
			AudioManager.PlayTheme(RunSettings.LobbyTheme);
			OnBeforeSceneSwitched?.Invoke();
			OnSceneSwitched?.Invoke();
		}
#endif

		#endregion
	}
}