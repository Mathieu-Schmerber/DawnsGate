using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Systems.Dialogue.Editor
{
	[CustomEditor(typeof(DialogueData))]
	public class DialogueDataEditor : UnityEditor.Editor
	{
		private DialogueData _data;
		private Guid _currentPromptId;

		private DialoguePrompt _currentPrompt => _data.Prompts[_currentPromptId];

		public override void OnInspectorGUI()
		{
			// Get latest version
			serializedObject.Update();

			_data = target as DialogueData;

			if (_data.Prompts.Count > 0)
			{
				if (_currentPromptId == Guid.Empty)
					_currentPromptId = _data.Prompts.First().Key;
				DrawPrompt();
			}
			else if (GUILayout.Button("Create first prompt"))
				_data.Prompts.Add(Guid.NewGuid(), new DialoguePrompt());

			// Save
			serializedObject.ApplyModifiedProperties();

			if (GUILayout.Button("Save"))
			{
				EditorUtility.SetDirty(target);
				AssetDatabase.SaveAssets();
			}
		}

		private void DrawPrompt()
		{
			_currentPrompt.Author = GUILayout.TextField(_currentPrompt.Author);
			_currentPrompt.RichText = GUILayout.TextArea(_currentPrompt.RichText, GUILayout.Height(EditorGUIUtility.singleLineHeight * 5));
		}
	}
}