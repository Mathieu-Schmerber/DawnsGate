using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.IO;
using Game.Systems.Run.Rooms;

namespace Game.Systems.Run.Editor
{
    public static class RoomEditorWindow
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

			var folder = RunManager.RunSettings.RoomFolders[type];
			string roomFolderName = $"{type}-{folder.GetValidSceneNumber()}";
			Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

			scene.name = roomFolderName;
			PopulateNewScene(type, scene);
			EditorSceneManager.SaveScene(scene, Path.Combine(folder.Folder, $"{roomFolderName}.unity"));
		}

		private static void PopulateNewScene(RoomType type, Scene scene)
		{
			GameObject roomLogic = (GameObject)PrefabUtility.InstantiatePrefab(Databases.Database.Templates.Editor.RoomLogic);
			RoomInfo roomInfo = roomLogic.GetComponent<RoomInfo>();

			roomInfo.Type = type;
		}
	}
}
