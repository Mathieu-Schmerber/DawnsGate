using Game.Entities.Player;
using Game.Managers;
using Game.Systems.Items;
using Game.Systems.Run.GPE;
using Nawlian.Lib.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Systems.Run.Rooms
{
	public class ShopRoom : AutoClearRoom
	{
		[SerializeField] private bool _isLifeShop;
		[SerializeField] private ItemStand[] _stands;

		private List<ItemBaseData> _items;


		protected override void Awake()
		{
			base.Awake();

			Inventory inventory = GameManager.Player.GetComponent<Inventory>();

			_items = Databases.Database.Data.Item.All<ItemBaseData>().Where(x => x.IsLifeItem == _isLifeShop && !inventory.HasEquipped(x)).ToList();
		}

		protected override void OnRoomReady()
		{
			_stands.ForEach(x => DefineItem(x));
			base.OnRoomReady();
		}

		private void DefineItem(ItemStand x)
		{
			if (_items.Count == 0)
			{
				x.SetItem(null);
				return;
			}

			var item = _items?.Random();

			_items.Remove(item);
			if (item.IsLifeItem)
			{
				x.SetItem(new ItemSummary() { Data = item, Quality = Databases.Database.Data.Item.Settings.NumberOfUpgrades - 1 });
				x.Cost = (int)item.LifeCost;
			}
			else
			{
				x.SetItem(new ItemSummary() { Data = item, Quality = 0 });
				x.Cost = Databases.Database.Data.Item.Settings.ItemCosts.RandomCost(item.Type);
			}
		}
	}
}
