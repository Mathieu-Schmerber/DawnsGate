using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityToolbarExtender;
using Nawlian.Lib.EditorTools.Helpers;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using Nawlian.Lib.Utils;

namespace Nawlian.Lib.EditorTools.SceneSwitcher
{
	[InitializeOnLoad]
	public class SceneSwitchLeftButton
	{
		public struct SceneItem
		{
			public string Name => System.IO.Path.GetFileNameWithoutExtension(Path);
			public string DisplayName => $"{Directory.GetParent(Path).Parent.Name}\\{System.IO.Path.GetFileNameWithoutExtension(Path)}";
			public string Path { get; set; }
			public int BuildIndex { get; set; }

			public SceneItem(string path, int buildIndex)
			{
				Path = path;
				BuildIndex = buildIndex;
			}
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
			SceneItem[] scenes = SceneHelper.GetAllSceneInProject().Select(x => new SceneItem(x.Path, x.BuildIndex)).OrderBy(x => x.DisplayName).ToArray();

			GUILayout.FlexibleSpace();

			int currentScene = GetActiveSceneIndex(scenes);
			int selected = currentScene;

			selected = EditorGUILayout.Popup(selected, scenes.Select(x => x.DisplayName).ToArray());

			if (selected != currentScene)
			{
				if (Application.isPlaying)
					return;

				OpenSceneMode method;
				int choice = EditorUtility.DisplayDialogComplex("Scene switcher", $"How do you want to load {scenes[selected].Name} ?", "Change scene", "Cancel", "Add scene");

				switch (choice)
				{
					case 0:
						method = OpenSceneMode.Single;
						break;
					case 2:
						method = OpenSceneMode.Additive;
						break;
					default:
						return;
				}
				EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
				EditorSceneManager.OpenScene(scenes[selected].Path, method);
			}
		}
	}
}