using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Systems.Items.Active
{
	public abstract class AActiveItem : AEquippedItem
	{
		protected ActiveItemData _data;

		public override ItemBaseData Details => _data;
		public override string GetDescription() 
			=> _data.Description.Replace("{Damage:u}", $"<color=red>{_entity.Scale(_data.Stages[Quality].Damage, Entities.Shared.StatModifier.AttackDamage)}</color>")
								.Replace("{Damage:%}", $"<color=red>{_data.Stages[Quality].Damage}%</color>")
								.Replace("{Duration}", $"<color=yellow>{_data.Stages[Quality].Duration}</color>")
								.Replace("{Amount}", $"<color=yellow>{_data.Stages[Quality].Amount}</color>");

		public override void OnEquipped(ItemBaseData data, int quality)
		{
			base.OnEquipped(data, quality);
			_data = data as ActiveItemData;
		}
	}
}
