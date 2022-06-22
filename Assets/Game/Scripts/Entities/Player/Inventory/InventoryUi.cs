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
		[SerializeField] private Vector2 _openedPos;
		[SerializeField] private Vector2 _closedPos;
		[SerializeField] private float _animationTime;

		private SlotUi[] _slots;

		public SlotUi SelectedSlot => _slots.FirstOrDefault(x => x.Selected);

		private void Awake()
		{
			_slots = GetComponentsInChildren<SlotUi>();
		}

		private void Start()
		{
			_slots.ForEach(x => x.interactable = IsOpen);
			_menuPanel.anchoredPosition = _closedPos;
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
			base.Open();

			Tween.AnchoredPosition(_menuPanel, _closedPos, _openedPos, _animationTime, 0, Tween.EaseOut);
			EventSystem.current.SetSelectedGameObject(_slots[0].gameObject);
			_slots.ForEach(x => x.interactable = IsOpen);
		}

		public override void Close()
		{
			base.Close();

			Tween.AnchoredPosition(_menuPanel, _openedPos, _closedPos, _animationTime, 0, Tween.EaseOut);
			_slots.ForEach(x => x.Deselect());
			EventSystem.current.SetSelectedGameObject(null);
			_slots.ForEach(x => x.interactable = IsOpen);
		}
	}
}
