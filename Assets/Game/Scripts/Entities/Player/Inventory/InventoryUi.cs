using Game.Managers;
using Nawlian.Lib.Extensions;
using Pixelplacement;
using Plugins.Nawlian.Lib.Systems.Menuing;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Entities.Player.Inventory
{
	public class InventoryUi : AMenu
	{
		[SerializeField] private RectTransform _menuPanel;

		private SlotUi[] _slots;

		public SlotUi SelectedSlot => _slots.FirstOrDefault(x => x.Selected);

		protected override void Awake()
		{
			base.Awake();
			_slots = GetComponentsInChildren<SlotUi>();
		}

		private void Start()
		{
			if (_menuPanel.anchoredPosition == _openPosition)
				GuiManager.CloseMenu<InventoryUi>();
		}

		private void OnEnable()
		{
			Inventory.OnUpdated += UpdateDisplay;
			InputManager.OnInventoryPressed += OpenOrClose;
		}

		private void OnDisable()
		{
			Inventory.OnUpdated -= UpdateDisplay;
			InputManager.OnInventoryPressed -= OpenOrClose;
		}

		private void UpdateDisplay(Inventory inventory)
		{
			_slots.ForEach(x => x.SetItem(null));
			for (int i = 0; i < inventory.Items.Length; i++)
				_slots[i].SetItem(inventory.Items[i]);
		}

		private void OpenOrClose()
		{
			if (IsOpen)
				GuiManager.CloseMenu<InventoryUi>();
			else
				GuiManager.OpenMenu<InventoryUi>();
		}

		public override void Open()
		{
			_isOpen = true;
			Tween.AnchoredPosition(_menuPanel, _closePosition, _openPosition, _duration, 0, Tween.EaseOut);
			EventSystem.current.SetSelectedGameObject(_slots[0].gameObject);
			_slots.ForEach(x => x.interactable = IsOpen);
		}

		public override void Close()
		{
			_isOpen = false;
			Tween.AnchoredPosition(_menuPanel, _openPosition, _closePosition, _duration, 0, Tween.EaseOut);
			_slots.ForEach(x => x.Deselect());
			EventSystem.current.SetSelectedGameObject(null);
			_slots.ForEach(x => x.interactable = IsOpen);
		}

		#region Editor

		public override void OpenEditorButton()
		{
			_menuPanel.anchoredPosition = _openPosition;
			FindObjectOfType<EventSystem>().SetSelectedGameObject(GetComponentsInChildren<SlotUi>()[0].gameObject);
			GetComponentsInChildren<SlotUi>().ForEach(x => x.interactable = IsOpen);
		}

		public override void CloseEditorButton()
		{
			_menuPanel.anchoredPosition = _closePosition;
			FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
			GetComponentsInChildren<SlotUi>().ForEach(x => x.interactable = IsOpen);
		}

		#endregion
	}
}
