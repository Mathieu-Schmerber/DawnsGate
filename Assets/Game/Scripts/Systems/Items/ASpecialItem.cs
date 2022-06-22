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

		public override ItemBaseData Details => _data;
		public override string GetDescription()
			=> _data.Description.Replace("{Damage:u}", $"<color=red>{_data.Stages[Quality].Damage}</color><color=orange>(+{_entity.Scale(_data.Stages[Quality].Damage, Entities.Shared.StatModifier.AttackDamage) - _data.Stages[Quality].Damage})</color>")
								.Replace("{Damage:%}", $"<color=red>{_data.Stages[Quality].Damage}%</color>")
								.Replace("{Duration}", $"<color=yellow>{_data.Stages[Quality].Duration}</color>")
								.Replace("{Amount}", $"<color=yellow>{_data.Stages[Quality].Amount}</color>")
								.Replace("{Effect}", $"<color=green><b>{(_data.ApplyEffect == null ? "No Effect" : _data.ApplyEffect.DisplayName)}</b></color>");

		public override void OnEquipped(ItemBaseData data, int quality)
		{
			base.OnEquipped(data, quality);
			_data = data as SpecialItemData;
		}
	}
}
