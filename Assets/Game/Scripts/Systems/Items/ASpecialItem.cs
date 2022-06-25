using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Systems.Items
{
	public abstract class ASpecialItem : AEquippedItem
	{
		protected SpecialItemData _data;

		protected override string GetItemDescription(int quality)
			=> _data.Description.Replace("{Damage:u}", $"<color=red>{_data.Stages[quality].Damage}</color><color=orange>(+{_entity.Scale(_data.Stages[quality].Damage, Entities.Shared.StatModifier.AttackDamage) - _data.Stages[quality].Damage})</color>")
								.Replace("{Damage:%}", $"<color=red>{_data.Stages[quality].Damage}%</color>")
								.Replace("{Duration}", $"<color=yellow>{_data.Stages[quality].Duration}</color>")
								.Replace("{Amount}", $"<color=yellow>{_data.Stages[quality].Amount}</color>")
								.Replace("{Effect}", $"<color=green><b>{(_data.ApplyEffect == null ? "No Effect" : _data.ApplyEffect.DisplayName)}</b></color>");

		public override void OnEquipped(ItemSummary summary)
		{
			base.OnEquipped(summary);
			_data = summary.Data as SpecialItemData;
		}
	}
}
