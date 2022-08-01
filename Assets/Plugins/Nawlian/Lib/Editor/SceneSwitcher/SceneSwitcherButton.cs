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
	public class SceneSwitcherButton
	{
		static SceneSwitcherButton()
		{
			ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
		}

		private static void OnToolbarGUI()
		{
			if (Application.isPlaying)
				return;

			GUILayout.FlexibleSpace();
			bool pressed = GUILayout.Button($"{EditorSceneManager.GetActiveScene().name}", EditorStyles.layerMaskField, GUILayout.Width(200), GUILayout.Height(25));

			if (pressed)
				SceneSwitcherWindow.Open();
		}
	}
}