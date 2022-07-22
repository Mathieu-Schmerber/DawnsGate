using Game.Entities.Player;
using Game.Entities.Shared;
using Game.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Items
{
	public abstract class AEquippedItem : MonoBehaviour
	{
		protected EntityIdentity _entity;
		protected ItemSettingsData _settings;

		public int Quality { get => Summary.Quality; private set { Summary.Quality = value; } }
		public ItemBaseData Details => Summary.Data;
		public bool HasUpgrade => Quality < Databases.Database.Data.Item.Settings.NumberOfUpgrades - 1 && !Details.IsLifeItem && !Summary.isMerged;
		public bool IsAffordable => GameManager.CanRunMoneyAfford(NextUpgradePrice);
		public int NextUpgradePrice => _minimumPrice + (_minimumPrice / 100) * (Quality + 1) * _settings.PriceInflationPerUpgrade;
		private int _minimumPrice => _settings.ItemCosts[Details.Type].x;

		public ItemSummary Summary { get; private set; }
		public AEquippedItem MergedBehaviour { get; set; }

		protected virtual void Awake()
		{
			_entity = GetComponentInParent<EntityIdentity>();
			_settings = Databases.Database.Data.Item.Settings;
		}

		public virtual void OnEquipped(ItemSummary summary)
		{
			Summary = summary;
		}

		public virtual void OnUnequipped()
		{
			Destroy(this);
		}

		public void Upgrade()
		{
			if (HasUpgrade)
			{
				Quality++;
				OnUpgrade();
			}
		}

		protected virtual void OnUpgrade() => Inventory.OnItemUpgraded();

		protected abstract string GetItemDescription(int quality);

		public string GetDescription(int quality = -1)
		{
			int stage = quality == -1 ? Quality : quality;

			if (Summary.isMerged)
			{
				int mergedStage = quality == -1 ? MergedBehaviour.Quality : quality;
				string stat = Details.Type == ItemType.STAT ? GetItemDescription(stage) : MergedBehaviour.GetItemDescription(mergedStage);
				string other = Details.Type == ItemType.STAT ? MergedBehaviour.GetItemDescription(mergedStage) : GetItemDescription(stage);

				return $"{stat}{Environment.NewLine}{other}";
			}
			return GetItemDescription(stage);
		}
	}
}
