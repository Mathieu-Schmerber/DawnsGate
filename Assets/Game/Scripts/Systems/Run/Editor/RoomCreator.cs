using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.IO;
using Game.Systems.Run.Rooms;
using Game.Managers;
using System.Linq;

namespace Game.Systems.Run.Editor
{
	public static class RoomCreator
	{
		[MenuItem("Tools/Game/Create Room/Combat room", false)] public static void CreateC() => CreateRoom(RoomType.COMBAT);
		[MenuItem("Tools/Game/Create Room/Event room", false)] public static void CreateE() => CreateRoom(RoomType.EVENT);
		[MenuItem("Tools/Game/Create Room/Shop room", false)] public static void CreateS() => CreateRoom(RoomType.SHOP);
		[MenuItem("Tools/Game/Create Room/Life shop room", false)] public static void CreateLS() => CreateRoom(RoomType.LIFE_SHOP);
		[MenuItem("Tools/Game/Create Room/Upgrade room", false)] public static void CreateU() => CreateRoom(RoomType.UPGRADE);
		[MenuItem("Tools/Game/Create Room/Boss room", false)] public static void CreateB() => CreateRoom(RoomType.BOSS);

		private static void CreateRoom(RoomType type)
		{
			if (RunManager.RunSettings.RoomFolders == null || !RunManager.RunSettings.RoomFolders.ContainsKey(type))
			{
				EditorUtility.DisplayDialog("Error", $"No folder path have been set within the run settings '{RunManager.RunSettings.name}'", "ok");
				return;
			}

			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
			EditorSceneManager.OpenScene(RunManager.RunSettings.BootScenePath, OpenSceneMode.Single);

			RunSettingsData.RoomFolder folder = RunManager.RunSettings.RoomFolders[type];
			string roomFolderName = $"{type}-{folder.GetValidSceneNumber()}";
			string folderPath = Path.Combine(folder.Folder, roomFolderName);
			string scenePath = Path.Combine(folderPath, $"{roomFolderName}.unity");
			Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);

			AssetDatabase.CreateFolder(folder.Folder, roomFolderName);

			scene.name = roomFolderName;
			PopulateNewScene(type, folderPath);
			EditorSceneManager.SaveScene(scene, scenePath);
			AddToBuild(scenePath);
		}

		private static void AddToBuild(string scenePath)
		{
			List<EditorBuildSettingsScene> editorBuildSettingsScenes = EditorBuildSettings.scenes.ToList();

			editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
			EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
		}

		private static RoomInfoData CreateRoomData(string sceneFolderPath)
		{
			// Create RoomNavigationData asset
			RoomInfoData asset = ScriptableObject.CreateInstance<RoomInfoData>();
			string sceneName = Path.GetFileName(sceneFolderPath);
			string soPath = Path.Combine(sceneFolderPath, $"{sceneName}.asset");

			Debug.Log(soPath);
			if (File.Exists(soPath))
				File.Delete(soPath);
			AssetDatabase.CreateAsset(asset, soPath);
			AssetDatabase.SaveAssets();

			// Refresh project directory
			AssetDatabase.Refresh();

			return asset;
		}

		private static void PopulateNewScene(RoomType type, string folderPath)
		{
			GameObject roomLogic = (GameObject)PrefabUtility.InstantiatePrefab(Databases.Database.Templates.Editor.RoomLogic);
			RoomInfo roomInfo = roomLogic.GetComponent<RoomInfo>();

			// Unpack prefab, so the _navigation doesn't get overwritten
			PrefabUtility.UnpackPrefabInstance(roomLogic, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
			roomInfo.Type = type;
			roomInfo.Data = CreateRoomData(folderPath);

			EditorUtility.SetDirty(roomInfo.Data);
			AssetDatabase.SaveAssets();
		}
	}
}
