using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using System;
using UnityEngine;

namespace Game.Systems.Scaling
{
	public abstract class UpgradableItemBase<T> : ItemBase
	{
		[OnInspectorGUI(nameof(DrawPreview), append: true)]
		public Sprite Graphics;

		[Space]
		public T[] Stages;

#if UNITY_EDITOR

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
		public void InitStages()
		{
			int nbStage = Databases.Database.Data.Item.Settings.NumberOfUpgrades;

			if (Stages == null || Stages.Length == 0)
				Stages = new T[nbStage];
			else if (Stages.Length != nbStage)
				Array.Resize(ref Stages, nbStage);
		}

#endif
	}
}
