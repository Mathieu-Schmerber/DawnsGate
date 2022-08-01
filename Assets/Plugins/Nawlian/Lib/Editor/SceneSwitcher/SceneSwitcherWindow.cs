using Nawlian.Lib.EditorTools.Helpers;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Nawlian.Lib.EditorTools.SceneSwitcher
{
	public class SceneSwitcherWindow : EditorWindow
	{
		#region Types
		[System.Serializable]
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
		#endregion

		public static class BackgroundStyle
		{
			public static GUIStyle Get(GUIStyle baseStyle, Color normal)
			{
				GUIStyle style = new(baseStyle);
				Texture2D ntexture = new Texture2D(1, 1);
				Texture2D htexture = new Texture2D(1, 1);

				ntexture.SetPixel(0, 0, normal);
				ntexture.Apply();
				style.normal.background = ntexture;

				htexture.SetPixel(0, 0, new Color(normal.r + .2f, normal.g + .2f, normal.b + .2f));
				htexture.Apply();
				style.hover.background = htexture;
				return style;
			}
		}

		private SceneItem[] _scenes;
		private Vector2 _scollPos;
		private int _activeScene;
		
		public static void Open()
		{
			SceneSwitcherWindow window = ScriptableObject.CreateInstance<SceneSwitcherWindow>();
			Vector2 size = new Vector2(225, 300);
			Rect win = EditorGUIUtility.GetMainWindowPosition();

			// Calculating window ratios to center the window next to the play button
			Vector2 pos = new Vector2(win.x + (win.width * .415f) - (size.x * .5f), win.y + EditorGUIUtility.singleLineHeight * 1.5f);

			window.Init();
			window.position = new Rect(pos.x, pos.y, size.x, window.GetMinimumHeight());
			window.ShowPopup();
			window.Focus();
		}

		private float GetMinimumHeight() => (_scenes.Length + 1) * EditorGUIUtility.singleLineHeight + 7;

		private static int GetActiveSceneIndex(SceneItem[] array)
		{
			string name = EditorSceneManager.GetActiveScene().name;

			for (int i = 0; i < array.Length; i++)
				if (array[i].Name == name)
					return i;
			return 0;
		}

		private void Init()
		{
			_scenes = SceneHelper.GetAllSceneInProject().Select(x => new SceneItem(x.Path, x.BuildIndex)).OrderBy(x => x.DisplayName).ToArray();
			_activeScene = GetActiveSceneIndex(_scenes);
		}

		private void CloseWindow() => Close();
		private void OnLostFocus() => CloseWindow();

		private void OnGUI()
		{
			var evenBtn = EditorStyles.toolbarButton;
			var oddBtn = BackgroundStyle.Get(EditorStyles.toolbarButton, new Color(.2f, .2f, .2f, 1f));
			_scollPos = EditorGUILayout.BeginScrollView(_scollPos);
			for (int i = 0; i < _scenes.Length; i++)
			{
				SceneItem item = _scenes[i];

				EditorGUILayout.BeginHorizontal(i % 2 == 0 ? evenBtn : oddBtn);
				if (GUILayout.Button(item.DisplayName, i == _activeScene ? EditorStyles.boldLabel : EditorStyles.label))
				{
					EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
					EditorSceneManager.OpenScene(item.Path, OpenSceneMode.Single);
					CloseWindow();
				}
				if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.ExpandHeight(true)))
					EditorSceneManager.OpenScene(item.Path, OpenSceneMode.Additive);
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndScrollView();
		}
	}
}