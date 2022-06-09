using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Game.Assets.Game.Scripts.Editor
{
	public static class SuccessMessageBox
	{
		public static void Show(string message)
		{
			SirenixEditorGUI.BeginInlineBox();
			{
				GUILayout.Label(new GUIContent(EditorGUIUtility.IconContent("_Help@2x").image));
			}
			SirenixEditorGUI.EndInlineBox();
		}
	}
}
