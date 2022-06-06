using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Nawlian.Lib.Utils.Database.Editor
{
    [CustomEditor(typeof(DatabaseData))]
    public class DatabaseDataEditor : OdinEditor
    {
		private DatabaseData _data;

		protected override void OnEnable()
		{
			_data = target as DatabaseData;
		}

		public override void OnInspectorGUI() => Draw();

		public static void SaveAsset(DatabaseData data)
		{
			bool canGenerate = !string.IsNullOrEmpty(data.DatabaseFolder) && !string.IsNullOrEmpty(data.AssetPathFromResources);

			if (string.IsNullOrEmpty(data.DatabaseFolder))
				Debug.LogError("Nawlian/Lib - The Database class folder must be defined.");
			if (string.IsNullOrEmpty(data.AssetPathFromResources))
				Debug.LogError("Nawlian/Lib - Any database asset should be within a 'Resources' folder");
			if (canGenerate)
			{
				Debug.Log($"Nawlian/Lib - Saving database asset '{data.name}'...");
				EditorUtility.SetDirty(data);
				AssetDatabase.SaveAssets();
				DatabaseCodeGenerator.GenerateCode(data);
			}
		}

		public void Draw()
		{
			#region Generation

			bool canGenerate = true;

			SirenixEditorGUI.Title("Generation", "", TextAlignment.Left, true);

			_data.DatabaseFolder = SirenixEditorFields.FolderPathField(new GUIContent("Database folder"), _data.DatabaseFolder, "", false, true);
			GUI.enabled = false;
			SirenixEditorFields.FolderPathField(new GUIContent("Database class"), $"{_data.name}.cs", _data.DatabaseFolder, false, true);
			SirenixEditorFields.FolderPathField(new GUIContent("Resource path"), _data.AssetPathFromResources, _data.DatabaseFolder, false, true);
			GUI.enabled = true;

			canGenerate = !string.IsNullOrEmpty(_data.DatabaseFolder) && !string.IsNullOrEmpty(_data.AssetPathFromResources);

			if (string.IsNullOrEmpty(_data.DatabaseFolder))
				EditorGUILayout.HelpBox("The Database class folder must be defined.", MessageType.Error);
			if (string.IsNullOrEmpty(_data.AssetPathFromResources))
				EditorGUILayout.HelpBox("Any database asset should be within a 'Resources' folder", MessageType.Error);
			if (canGenerate && GUILayout.Button("Save database"))
				SaveAsset(_data);
			if (canGenerate && GUILayout.Button("Open database panel"))
				DatabaseDataEditorWindow.OpenEditor(_data);
			#endregion
		}
	}
}