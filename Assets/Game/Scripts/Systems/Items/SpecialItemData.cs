using Game.Systems.Combat.Effects;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Items
{
	[CreateAssetMenu(menuName = "Data/Items/Special item")]
	public class SpecialItemData : UpgradableItemBaseData<SpecialItemData.Stage>
	{
		[Serializable]
		public struct Stage
		{
			public float Damage;
			public float Duration;
			public float Amount;
			public float Range;
		}

		public GameObject SpawnPrefab;
		public AEffectBaseData ApplyEffect;
		[TextArea] public string Description;

		public override string GetRichDescription(int quality) =>
			Description.Replace("{Damage:u}", $"<color=red>{Stages[quality].Damage}</color>")
						.Replace("{Damage:%}", $"<color=red>{Stages[quality].Damage}%</color>")
						.Replace("{Duration}", $"<color=yellow>{Stages[quality].Duration}</color>")
						.Replace("{Amount}", $"<color=yellow>{Stages[quality].Amount}</color>")
						.Replace("{Effect}", $"<color=green><b>{(ApplyEffect == null ? "No Effect" : ApplyEffect.DisplayName)}</b></color>");

#if UNITY_EDITOR

		[Button(Name = "Scale stage 1 to bottom", Style = ButtonStyle.FoldoutButton)]
		public void ApplyScaling(float scalePercent = 50)
		{
			for (int i = 1; i < Stages.Length; i++)
			{
				Stages[i].Damage = Mathf.Ceil(Stages[0].Damage * ((i * scalePercent) / 100) + Stages[0].Damage);
				Stages[i].Duration = Mathf.Ceil(Stages[0].Duration * ((i * scalePercent) / 100) + Stages[0].Duration);
				Stages[i].Amount = Mathf.Ceil(Stages[0].Amount * ((i * scalePercent) / 100) + Stages[0].Amount);
				Stages[i].Range = Mathf.Ceil(Stages[0].Range * ((i * scalePercent) / 100) + Stages[0].Range);
			}
		}

		protected override void DrawItemDescriptionPreview()
		{
			if (Stages == null || Stages.Length == 0)
				return;

			GUIStyle rich = new GUIStyle(GUI.skin.label);
			string richDescription = GetRichDescription(0);

			rich.richText = true;
			GUILayout.Label(richDescription, rich);
		}

#endif
	}
}
