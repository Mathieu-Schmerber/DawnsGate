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
	[CreateAssetMenu(menuName = "Data/Items/Active item")]
	public class ActiveItemData : UpgradableItemBaseData<ActiveItemData.ActiveStage>
	{
		[Serializable]
		public struct ActiveStage
		{
			public float Damage;
			public float Duration;
			public float Amount;
			public float Range;
		}

		public GameObject SpawnPrefab;
		public AEffectBaseData ApplyEffect;

		[TextArea] public string Description;

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
			string richDescription = Description.Replace("{Damage:u}", $"<color='red'>{Stages[0].Damage}</color>")
												.Replace("{Damage:%}", $"<color='red'>{Stages[0].Damage}%</color>")
												.Replace("{Duration}", $"<color='yellow'>{Stages[0].Duration}</color>")
												.Replace("{Amount}", $"<color='yellow'>{Stages[0].Amount}</color>")
												.Replace("{Range}", $"<color='yellow'>{Stages[0].Range}</color>");

			rich.richText = true;
			GUILayout.Label(richDescription, rich);
		}

#endif
	}
}
