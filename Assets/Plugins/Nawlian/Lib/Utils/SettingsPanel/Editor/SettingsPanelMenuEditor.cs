using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nawlian.Lib.Utils.SettingsPanel
{
	public class SettingsPanelMenuEditor : OdinMenuEditorWindow
	{
		[MenuItem("Tools/Nawlian/Settings Panel")]
		public static void OpenWindow() => GetWindow<SettingsPanelMenuEditor>("Settings Panel").Show();

		/// <summary>
		/// Building the side bar
		/// </summary>
		/// <returns></returns>
		protected override OdinMenuTree BuildMenuTree()
		{
			OdinMenuTree tree = new OdinMenuTree();

			tree.Config.DrawSearchToolbar = true;
			tree.Config.DefaultMenuStyle.Height = 18;

			tree.AddAllAssetsAtPath("", "Assets/Game/Resources/Settings", typeof(ScriptableObject), true);
			return tree;
		}
	}
}