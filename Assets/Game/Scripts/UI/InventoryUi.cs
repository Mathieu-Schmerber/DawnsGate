using Game.Entities.Player;
using Game.Managers;
using Nawlian.Lib.Extensions;
using Plugins.Nawlian.Lib.Systems.Selection;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
	public class InventoryUi : MonoBehaviour, ISelectable
	{
		private InventorySlotUi[] _slots;
		private bool _inUse;
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

		private void UpdateDisplay(Inventory inventory)
		{
			_slots.ForEach(x => x.SetItem(null));
			for (int i = 0; i < inventory.Items.Length; i++)
				_slots[i].SetItem(inventory.Items[i]);
		}

		public void Select()
		{
			_inUse = true;
			EventSystem.current.SetSelectedGameObject(_slots[0].gameObject);
			_slots.ForEach(x => x.interactable = true);
		}

		public void Deselect()
		{
			_inUse = false;
			_slots.ForEach(x => x.Deselect());
			EventSystem.current.SetSelectedGameObject(null);
			_slots.ForEach(x => x.interactable = false);
		}

		public static bool IsInUse => GuiManager.InventoryUI._inUse;
		public static void StartUsing() => GuiManager.InventoryUI.Select();
		public static void EndUsing() => GuiManager.InventoryUI.Deselect();
	}
}
