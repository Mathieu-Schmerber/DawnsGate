using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Game.Scripts.Tools
{
	public static class SuccessMessageBox
	{
		public static void Show(string message)
		{
			var color = GUI.color;
			float height = EditorGUIUtility.singleLineHeight * 1.5f;
			Vector2 baseSize = EditorGUIUtility.GetIconSize();

			EditorGUIUtility.SetIconSize(Vector2.one * height);
			SirenixEditorGUI.BeginBox();
			{
				GUILayout.BeginHorizontal();
				{
					GUI.color = Color.green;
					GUILayout.Space(10);
					GUILayout.Label(new GUIContent(EditorGUIUtility.IconContent("Progress@2x").image), GUILayout.Height(height));
					GUI.color = color;
					GUILayout.Label(message, GUILayout.Height(height));
				}
				GUILayout.EndHorizontal();
			}
			SirenixEditorGUI.EndBox();
			EditorGUIUtility.SetIconSize(baseSize);
		}
	}
}
