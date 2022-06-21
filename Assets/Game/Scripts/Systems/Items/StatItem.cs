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

		public override ItemBaseData Details => _data;

		public override string GetDescription()
		{
			string result = string.Empty;

			foreach (var key in _data.Stages[_quality].Keys)
			{
				bool positive = _data.Stages[_quality][key].Value > 0;
				string color = positive ? "green" : "red";
				string name = Databases.Database.Data.Item.Settings.StatGraphics[key].Name;
				string newLine = string.IsNullOrEmpty(result) ? "" : Environment.NewLine;

				result = $"{result}{newLine}{$"<color={color}>{(positive ? "+" : "-")}{Mathf.Abs(_data.Stages[_quality][key].Value)}%</color> {name}"}";
			}
			return result;
		}

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