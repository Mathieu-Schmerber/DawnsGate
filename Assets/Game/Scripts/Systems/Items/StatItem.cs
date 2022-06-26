using Game.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Items
{
	public class StatItem : AEquippedItem
	{
		private StatItemData _data;

		protected override string GetItemDescription(int quality)
		{
			string result = string.Empty;

			foreach (var key in _data.Stages[quality].Keys)
			{
				bool positive = _data.Stages[quality][key].Value > 0;
				string color = positive ? "green" : "red";
				string name = Databases.Database.Data.Item.Settings.StatGraphics[key].Name;
				string newLine = string.IsNullOrEmpty(result) ? "" : Environment.NewLine;

				result = $"{result}{newLine}{$"<color={color}>{(positive ? "+" : "-")}{Mathf.Abs(_data.Stages[quality][key].Value)}%</color> {name}"}";
			}
			return result;
		}

		public override void OnEquipped(ItemSummary item)
		{
			base.OnEquipped(item);

			float healthRatio = _entity.CurrentHealth / _entity.MaxHealth;
			float armorRatio = _entity.MaxArmor == 0 ? 1 : _entity.CurrentArmor / _entity.MaxArmor;

			_data = item.Data as StatItemData;
			foreach (var key in _data.Stages[Quality].Keys)
				_entity.Stats.Modifiers[key].BonusModifier += _data.Stages[Quality][key].Value;

			_entity.CurrentHealth = Mathf.Max(1, _entity.MaxHealth * healthRatio);
			_entity.CurrentArmor = _entity.MaxArmor * armorRatio;
		}

		public override void OnUnequipped()
		{
			float healthRatio = _entity.CurrentHealth / _entity.MaxHealth;
			float armorRatio = _entity.MaxArmor == 0 ? 1 : _entity.CurrentArmor / _entity.MaxArmor;

			foreach (var key in _data.Stages[Quality].Keys)
				_entity.Stats.Modifiers[key].BonusModifier -= _data.Stages[Quality][key].Value;

			_entity.CurrentHealth = Mathf.Max(1, _entity.MaxHealth * healthRatio);
			_entity.CurrentArmor = _entity.MaxArmor * armorRatio;
			base.OnUnequipped();
		}

		public override void OnUpgrade()
		{
			base.OnUpgrade();

			float healthRatio = _entity.CurrentHealth / _entity.MaxHealth;
			float armorRatio = _entity.MaxArmor == 0 ? 1 : _entity.CurrentArmor / _entity.MaxArmor;

			foreach (var key in _data.Stages[Quality - 1].Keys)
				_entity.Stats.Modifiers[key].BonusModifier -= _data.Stages[Quality - 1][key].Value;
			foreach (var key in _data.Stages[Quality].Keys)
				_entity.Stats.Modifiers[key].BonusModifier += _data.Stages[Quality][key].Value;

			_entity.CurrentHealth = Mathf.Max(1, _entity.MaxHealth * healthRatio);
			_entity.CurrentArmor = _entity.MaxArmor * armorRatio;
		}
	}
}