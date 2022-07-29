using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Game.Managers;
using System.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace UnityToolbarExtender.Examples
{
	[InitializeOnLoad]
	public class SceneSwitchLeftButton
	{
		public struct SceneItem
		{
			public string Name => System.IO.Path.GetFileNameWithoutExtension(Path);
			public string DisplayName => $"{Directory.GetParent(Path).Parent.Name}\\{System.IO.Path.GetFileNameWithoutExtension(Path)}";
			public string Path { get; set; }
			public SceneItem(string path) => Path = path;
		}

		static SceneSwitchLeftButton()
		{
			ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
		}

		private static int GetActiveSceneIndex(SceneItem[] array)
		{
			string name = EditorSceneManager.GetActiveScene().name;

			for (int i = 0; i < array.Length; i++)
				if (array[i].Name == name)
					return i;
			return 0;
		}

		private static void OnToolbarGUI()
		{
			SceneItem[] scenes = EditorBuildSettings.scenes.Select(x => new SceneItem(x.path)).OrderBy(x => x.DisplayName).ToArray();

			GUILayout.FlexibleSpace();

			int currentScene = GetActiveSceneIndex(scenes);
			int selected = currentScene;
				
			selected = EditorGUILayout.Popup("", selected, scenes.Select(x => x.DisplayName).ToArray());

			if (selected != currentScene)
			{
				EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
				EditorSceneManager.OpenScene(scenes[selected].Path, OpenSceneMode.Single);
			}
		}
	}
}