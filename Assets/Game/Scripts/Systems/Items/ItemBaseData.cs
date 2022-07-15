using Sirenix.OdinInspector;
using UnityEngine;
using System;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using UnityEditor;
#endif

namespace Game.Systems.Items
{
	public enum ItemType
	{
		STAT = 0,
		PASSIVE,
		ACTIVE
	}

	public abstract class ItemBaseData : SerializedScriptableObject
	{
		public ItemType Type;

		public Type Component;

		[OnInspectorGUI("DrawPreview", append: true)]
		public Sprite Graphics;

		public ItemTag Tags;

		public bool IsLifeItem = false;
		[ShowIf(nameof(IsLifeItem)), MinValue(0), MaxValue(100)] public float LifeCost;

		public abstract string GetRichDescription(int quality);

#if UNITY_EDITOR

		[OnValueChanged(nameof(OnScriptChanged)), ValidateInput(nameof(ValidateScript), "Script needs to inherit AEquippedItem.")]
		public MonoScript Script;

		private void DrawPreview()
		{
			GUILayout.BeginVertical(GUI.skin.box);
			{
				SirenixEditorGUI.Title("Preview", "", TextAlignment.Center, false);
				GUILayout.Space(10);
				GUILayout.BeginHorizontal(GUILayout.Height(100));
				{
					GUILayout.FlexibleSpace();
					if (this.Graphics != null)
					{
						GUILayout.Label(this.Graphics.texture, GUILayout.Width(100), GUILayout.Height(100));
						GUILayout.BeginVertical();
						{
							SirenixEditorGUI.Title(name, "", TextAlignment.Center, true);
							DrawItemDescriptionPreview();
						}
						GUILayout.EndVertical();
					}
					else
					{
						GUILayout.Label("No graphics was set.");
					}
					GUILayout.FlexibleSpace();
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			GUILayout.Space(10);
		}

		protected abstract void DrawItemDescriptionPreview();


		[OnInspectorInit]
		private void AssignScript()
		{
			if (Script == null)
				Script = GetDefaultScript();
		}

		private MonoScript GetDefaultScript() => Type switch
		{
			ItemType.STAT => Databases.Database.Data.Item.Settings.DefaultStatScript,
			_ => null
		};

		private bool ValidateScript() => Script != null && !Script.GetClass().IsAbstract && Script.GetClass().IsSubclassOf(typeof(AEquippedItem));

		internal void OnScriptChanged()
		{
			if (ValidateScript())
			{
				Component = Script.GetClass();
				Debug.Log($"{name}.Component set to {Component}");
				EditorUtility.SetDirty(this);
				AssetDatabase.SaveAssets();
			}
		}

#endif
	}
}
