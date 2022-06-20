using Game.Systems.Items;
using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Entities.Player
{
	public class Inventory : MonoBehaviour
	{
		#region Types
		[System.Serializable] public class ItemDictionary : SerializedDictionary<ItemBaseData, AEquippedItem> { }
		#endregion

		[SerializeField] private GameObject _inventoryParent;
		[ShowInInspector, ReadOnly] private ItemDictionary _items = new();

		private AEquippedItem AddItemToInventory(ItemBaseData data)
		{
			AEquippedItem behaviour = _inventoryParent.AddComponent(data.Script.GetClass()) as AEquippedItem;

			_items[data] = behaviour;
			return behaviour;
		}

		public void EquipItem(ItemBaseData item, int quality)
		{
			AEquippedItem exists = _items.ContainsKey(item) ? _items[item] : null;

			if (exists == null)
			{
				exists = AddItemToInventory(item);
				exists.OnEquipped(item, quality);
			}
		}

		public void DropItem(ItemBaseData item)
		{
			int quality = _items[item].Quality;

			_items[item].OnUnequipped();
			_items.Remove(item);
			LootedItem.Create(transform.position, item, quality);
		}
	}
}
