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

			_data = item.Data as StatItemData;
			foreach (var key in _data.Stages[Quality].Keys)
				_entity.Stats.Modifiers[key].BonusModifier += _data.Stages[Quality][key].Value;
		}

		public override void OnUnequipped()
		{
			foreach (var key in _data.Stages[Quality].Keys)
				_entity.Stats.Modifiers[key].BonusModifier -= _data.Stages[Quality][key].Value;
			base.OnUnequipped();
		}

		public override void OnUpgrade()
		{
			base.OnUpgrade();
			foreach (var key in _data.Stages[Quality - 1].Keys)
				_entity.Stats.Modifiers[key].BonusModifier -= _data.Stages[Quality][key].Value;
			foreach (var key in _data.Stages[Quality].Keys)
				_entity.Stats.Modifiers[key].BonusModifier += _data.Stages[Quality][key].Value;
		}
	}
}