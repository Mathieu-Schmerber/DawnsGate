using Game.Systems.Items;
using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Entities.Player.Inventory
{
	public class Inventory : MonoBehaviour
	{
		#region Types
		[Serializable] public class ItemDictionary : SerializedDictionary<ItemBaseData, AEquippedItem> { }
		#endregion

		[SerializeField] private GameObject _inventoryParent;
		[ShowInInspector, ReadOnly] private ItemDictionary _items = new();
		private int _slotNumber;
		private int _occupiedSlots = 0;

		public AEquippedItem[] Items => _items.Where(x => x.Value != null).Select(x => x.Value).ToArray();
		public bool HasAvailableSlot => _occupiedSlots < _slotNumber;

		public static Action<Inventory> OnUpdated;

		private void Awake()
		{
			_slotNumber = Databases.Database.Data.Item.Settings.InventorySlots;
		}

		private AEquippedItem AddItemToInventory(ItemBaseData data)
		{
			AEquippedItem behaviour = _inventoryParent.AddComponent(data.Script.GetClass()) as AEquippedItem;

			_occupiedSlots++;
			_items[data] = behaviour;
			return behaviour;
		}

		public void EquipItem(ItemBaseData item, int quality)
		{
			if (!HasAvailableSlot)
				return;

			AEquippedItem exists = _items.ContainsKey(item) ? _items[item] : null;

			if (exists == null)
			{
				exists = AddItemToInventory(item);
				exists.OnEquipped(item, quality);
				OnUpdated?.Invoke(this);
			}
		}

		public void DropItem(ItemBaseData item)
		{
			int quality = _items[item].Quality;

			_occupiedSlots--;
			_items[item].OnUnequipped();
			_items.Remove(item);
			OnUpdated?.Invoke(this);
			LootedItem.Create(transform.position, item, quality);
		}
	}
}
