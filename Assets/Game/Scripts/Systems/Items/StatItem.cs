using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Systems.Items
{
	public class StatItem : AEquippedItem
	{
		private StatItemData _data;

		public override void OnEquipped(ItemBaseData data, int quality)
		{
			base.OnEquipped(data, quality);

			_data = data as StatItemData;
			foreach (var key in _data.Stages[_quality].Keys)
				_entity.Stats.Modifiers[key].BonusModifier += _data.Stages[_quality][key].Value;
		}

		public override void OnUnequipped()
		{
			foreach (var key in _data.Stages[_quality].Keys)
				_entity.Stats.Modifiers[key].BonusModifier -= _data.Stages[_quality][key].Value;
			base.OnUnequipped();
		}

		public override void OnUpgrade()
		{
			base.OnUpgrade();
			foreach (var key in _data.Stages[_quality - 1].Keys)
				_entity.Stats.Modifiers[key].BonusModifier -= _data.Stages[_quality][key].Value;
			foreach (var key in _data.Stages[_quality].Keys)
				_entity.Stats.Modifiers[key].BonusModifier += _data.Stages[_quality][key].Value;
		}
	}
}
