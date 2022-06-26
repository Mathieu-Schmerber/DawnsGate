using Game.Managers;
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
		[Serializable] public class ItemDictionary : SerializedDictionary<ItemBaseData, AEquippedItem> { }
		#endregion

		[SerializeField] private GameObject _inventoryParent;
		[ShowInInspector, ReadOnly] private ItemDictionary _items = new();
		private int _slotNumber;
		private int _occupiedSlots = 0;

		public AEquippedItem[] Items => _items.Where(x => x.Value != null).Select(x => x.Value).ToArray();
		public bool HasAvailableSlot => _occupiedSlots < _slotNumber;

		public static event Action<Inventory> OnUpdated;

		private void Awake()
		{
			_slotNumber = Databases.Database.Data.Item.Settings.InventorySlots;
		}

		private void AddItemToInventory(ItemSummary item)
		{
			AEquippedItem behaviour = _inventoryParent.AddComponent(item.Data.Script.GetClass()) as AEquippedItem;

			_occupiedSlots++;
			_items[item.Data] = behaviour;
			behaviour.OnEquipped(item);

			if (item.isMerged)
			{
				AEquippedItem mergedBehaviour = _inventoryParent.AddComponent(item.Merge.Data.Script.GetClass()) as AEquippedItem;
				mergedBehaviour.OnEquipped(item.Merge);
				behaviour.MergedBehaviour = mergedBehaviour;
			}
		}

		public bool HasEquipped(ItemBaseData item) => 
			_items.ContainsKey(item) && // The inventory has the item key
			_items[item] != null &&		// The inventory has the item value
			!_items.Values.Any(x => x.Summary.isMerged && x.Summary.Merge.Data == item); // No merged item contains the item

		public void EquipItem(ItemSummary summary)
		{
			if (!HasAvailableSlot)
				return;

			AEquippedItem exists = _items.ContainsKey(summary.Data) ? _items[summary.Data] : null;

			if (exists == null)
			{
				AddItemToInventory(summary);
				OnUpdated?.Invoke(this);
			}
		}

		public static void OnUpdate() => OnUpdated?.Invoke(GameManager.Player.GetComponent<Inventory>());

		public void DropItem(ItemBaseData item)
		{
			ItemSummary summary = _items[item].Summary;

			_occupiedSlots--;
			if (summary.isMerged)
				_items[item].MergedBehaviour.OnUnequipped();
			_items[item].OnUnequipped();
			_items.Remove(_items[item].Details);
			OnUpdated?.Invoke(this);
			LootedItem.Create(transform.position, summary);
		}

		public bool TryMergeItems(AEquippedItem a, AEquippedItem b)
		{
			int cost = GetMergeCost(a, b);

			if (!CanBeMerged(a, b) || !GameManager.CanRunMoneyAfford(cost))
				return false;

			// Set merged state
			a.Summary.Merge = b.Summary;
			a.MergedBehaviour = b;

			// Remove b's data from inventory, without removing its behaviour
			_occupiedSlots--;
			_items.Remove(b.Details);
			OnUpdated?.Invoke(this);

			GameManager.PayWithRunMoney(cost);
			return true;
		}

		public static bool CanBeMerged(AEquippedItem a) => !a.Details.IsLifeItem && !a.Summary.isMerged;

		public static bool CanBeMerged(AEquippedItem a, AEquippedItem b)
		{
			AEquippedItem[] equips = { a, b };

			return CanBeMerged(a) && CanBeMerged(b) && equips.Any(x => x.Details.Type == ItemType.STAT);
		}

		public static int GetMergeCost(AEquippedItem a, AEquippedItem b) => a.NextUpgradePrice + b.NextUpgradePrice;
	}
}
