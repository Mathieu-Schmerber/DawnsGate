using Game.Entities.Shared;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace Game.Systems.Items
{
	[CreateAssetMenu(menuName = "Data/Items/Stat item")]
	public class StatItemData : UpgradableItemBaseData<StatDictionary>
	{
#if UNITY_EDITOR

		[Button(Name = "Scale stage 1 to bottom", Style = ButtonStyle.FoldoutButton)]
		public void ApplyScaling(float scalePercent = 50)
		{
			for (int i = 1; i < Stages.Length; i++)
			{
				Stages[i].Clear();
				foreach (var key in Stages[0].Keys)
				{
					float value = i * scalePercent;
					float result = Mathf.Ceil(Stages[0][key].Value * (value / 100) + Stages[0][key].Value);

					if (Stages[i].ContainsKey(key))
						Stages[i][key] = new(result);
					else
						Stages[i].Add(key, new(result));
				}
			}
		}

		protected override void DrawItemDescriptionPreview()
		{
			GUIStyle rich = new GUIStyle(GUI.skin.label);
			rich.richText = true;

			foreach (var key in Stages[0].Keys)
			{

				bool positive = Stages[0][key].Value > 0;
				string color = positive ? "green" : "red";
				string name = Databases.Database.Data.Item.Settings.StatGraphics[key].Name;

				GUILayout.Label($"<color='{color}'>{(positive ? "+" : "-")}{Mathf.Abs(Stages[0][key].Value)}%</color> {name}", rich);
			}
		}
#endif
	}
}
