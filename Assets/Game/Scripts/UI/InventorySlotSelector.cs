using Game.Entities.Player;
using Game.Managers;
using Nawlian.Lib.Extensions;
using Plugins.Nawlian.Lib.Systems.Selection;
using System;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Game.UI
{
	public class InventorySlotSelector : MonoBehaviour
	{
		private InventorySlotUi[] _slots;
		private bool _inUse;
		private Func<InventorySlotUi, bool> _inUsePredicate;
		public static InventorySlotUi SelectedSlot => GuiManager.InventoryUI._slots.FirstOrDefault(x => x.Selected);

		private void Awake()
		{
			_slots = GetComponentsInChildren<InventorySlotUi>();
		}

		private void OnEnable()
		{
			Inventory.OnUpdated += UpdateDisplay;
		}

		private void OnDisable()
		{
			Inventory.OnUpdated -= UpdateDisplay;
		}

		private void UpdateDisplay(Inventory.InventoryUpdateEventArgs args)
		{
			_slots.ForEach(x => x.SetItem(null));
			for (int i = 0; i < args.inventory.Items.Length; i++)
				_slots[i].SetItem(args.inventory.Items[i]);
			if (_inUse && _inUsePredicate != null)
				Select(_inUsePredicate);
		}

		public void Select(Func<InventorySlotUi, bool> predicate = null)
		{
			_inUsePredicate = predicate;
			_inUse = true;
			foreach (var slot in _slots)
			{
				slot.interactable = true;
				slot.Usable = predicate == null ? true : predicate(slot);
			}
			if (SelectedSlot == false || !SelectedSlot.Usable)
				EventSystem.current.SetSelectedGameObject(_slots.FirstOrDefault(x => x.Usable)?.gameObject);
			BindSlotNavigation();
		}

		public void Deselect()
		{
			_inUse = false;
			EventSystem.current.SetSelectedGameObject(null);
			foreach (var slot in _slots)
			{
				slot.interactable = false;
				slot.Usable = true;
				slot.Deselect();
			}
		}

		private void BindSlotNavigation()
		{
			InventorySlotUi firstAvailable = _slots.FirstOrDefault(x => x.Usable);
			InventorySlotUi lastAvailable = _slots.LastOrDefault(x => x.Usable);
			InventorySlotUi prevAvailable = null;

			for (int i = 0; i < _slots.Length; i++)
			{
				InventorySlotUi slot = _slots[i];
				Navigation nav = slot.navigation;

				if (!slot.Usable)
					nav.mode = Navigation.Mode.None;
				else
				{
					nav.mode = Navigation.Mode.Explicit;
					nav.selectOnUp = prevAvailable ?? (firstAvailable == slot ? lastAvailable : firstAvailable);
					nav.selectOnDown = null;
					for (int j = i; j < _slots.Length; j++)
					{
						if (_slots[j] != slot && _slots[j].Usable)
						{
							nav.selectOnDown = _slots[j];
							break;
						}
					}
					nav.selectOnDown = nav.selectOnDown ?? firstAvailable;
					prevAvailable = slot;
				}
				slot.navigation = nav;
			}
		}

		private void LayoutRightNavigation(Selectable navObject)
		{
			foreach (var slot in _slots)
			{
				Navigation nav = slot.navigation;

				nav.selectOnRight = navObject;
				slot.navigation = nav;
			}
		}

		public static bool IsInUse => GuiManager.InventoryUI._inUse;
		public static bool HasUsableSlot => GuiManager.InventoryUI._slots.Any(x => x.Usable);
		public static void StartUsing(Func<InventorySlotUi, bool> predicate = null) => GuiManager.InventoryUI.Select(predicate);
		public static void SetRightNavigation(Selectable navObject) => GuiManager.InventoryUI.LayoutRightNavigation(navObject);
		public static void EndUsing() => GuiManager.InventoryUI.Deselect();
	}
}
