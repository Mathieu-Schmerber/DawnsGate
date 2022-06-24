using Game.Entities.Player;
using Game.Managers;
using Game.Systems.Items;
using Game.Systems.Run.GPE;
using Nawlian.Lib.Extensions;
using System;
using System.Linq;
using UnityEngine;

namespace Game.Systems.Run.Rooms
{
	public class ShopRoom : IdleRoom
	{
		[SerializeField] private bool _isLifeShop;
		[SerializeField] private ItemStand[] _stands;

		private ItemBaseData[] _items;


		protected override void Awake()
		{
			base.Awake();

			Inventory inventory = GameManager.Player.GetComponent<Inventory>();

			_items = Databases.Database.Data.Item.Items.All<ItemBaseData>().Where(x => x.IsLifeItem == _isLifeShop && !inventory.HasEquipped(x)).ToArray();
		}

		protected override void Start()
		{
			_stands.ForEach(x => DefineItem(x));
			base.Start();
		}

		private void DefineItem(ItemStand x)
		{
			var item = _items?.Random();

			x.SetItem(item, 0);
			x.Cost = Databases.Database.Data.Item.Settings.ItemCosts.RandomCost(item.Type);
		}
	}
}
