using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Nawlian.Lib.Utils.ResourcePanel
{
	public class ResourcePanelMenuEditor : OdinMenuEditorWindow
	{
		[MenuItem("Tools/Nawlian/Resource Panel")]
		public static void OpenWindow() => GetWindow<ResourcePanelMenuEditor>("Resource Panel").Show();

		/// <summary>
		/// Building the side bar
		/// </summary>
		/// <returns></returns>
		protected override OdinMenuTree BuildMenuTree()
		{
			OdinMenuTree tree = new OdinMenuTree();

			tree.Config.DrawSearchToolbar = true;
			tree.Config.DefaultMenuStyle.Height = 18;

			tree.AddAllAssetsAtPath("", "Assets/Game/Resources/Data", typeof(ScriptableObject), true);
			return tree;
		}
	}
}