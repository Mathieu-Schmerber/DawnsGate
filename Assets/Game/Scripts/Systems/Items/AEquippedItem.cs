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
		protected int _quality;
		protected ItemSettingsData _settings;

		public int Quality => _quality;
		public abstract ItemBaseData Details { get; }
		public bool HasUpgrade => Quality < Databases.Database.Data.Item.Settings.NumberOfUpgrades - 1 && !Details.IsLifeItem;
		public bool IsAffordable => GameManager.CanRunMoneyAfford(NextUpgradePrice);
		public int NextUpgradePrice => _minimumPrice + ((_minimumPrice / 100) * (_quality + 1)) * _settings.PriceInflationPerUpgrade;
		private int _minimumPrice => _settings.ItemCosts[Details.Type].x;

		protected virtual void Awake()
		{
			_entity = GetComponentInParent<EntityIdentity>();
			_settings = Databases.Database.Data.Item.Settings;
		}

		public virtual void OnEquipped(ItemBaseData data, int quality)
		{
			_quality = quality;
		}

		public virtual void OnUnequipped()
		{
			Destroy(this);
		}

		public virtual void OnUpgrade()
		{
			if (HasUpgrade && IsAffordable)
			{
				GameManager.PayWithRunMoney(NextUpgradePrice);
				_quality++;
				Inventory.OnUpdate();
			}
		}

		public abstract string GetDescription();
	}
}
