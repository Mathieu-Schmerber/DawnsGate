using Game.Entities.Player;
using Game.Managers;
using Game.Systems.Items;
using Game.Tools;
using Pixelplacement;
using Plugins.Nawlian.Lib.Systems.Menuing;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI
{
	public class InventoryMenuPanel : AClosableMenu
	{
		[Title("References")]
		[SerializeField] private SimpleDescriptorUi _itemDescription;
		[SerializeField] private SimpleDescriptorUi _effectDescription;

		private Inventory _inventory;

		protected override void OnEnable()
		{
			base.OnEnable();
			InputManager.OnInventoryPressed += OpenOrClose;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			InputManager.OnInventoryPressed -= OpenOrClose;
		}

		protected override void Awake()
		{
			base.Awake();
			_inventory = GameManager.Player.GetComponent<Inventory>();
		}

		private void Start()
		{
			if (_rect.anchoredPosition == _openPosition)
				GuiManager.CloseMenu<InventoryMenuPanel>();
		}

		private void OnSlotSubmitted(InventorySlotUi slot)
		{
			_inventory.DropItem(slot.Item.Details);
			DisplayItemDetails(slot);
		}

		private void DisplayItemDetails(InventorySlotUi slot)
		{
			_itemDescription.DescribeItem(slot.Item);

			_effectDescription.gameObject.SetActive(false);
			if (slot != null && slot.Item != null && slot.Item.Details != null && slot.Item.Details.Type != ItemType.STAT)
			{
				SpecialItemData data = slot.Item.Details as SpecialItemData;

				if (data.ApplyEffect != null)
				{
					_effectDescription.gameObject.SetActive(true);
					_effectDescription.DescribeEffect(data.ApplyEffect);
				}
			}
		}

		private void OpenOrClose()
		{
			if (IsOpen)
				GuiManager.CloseMenu<InventoryMenuPanel>();
			else
				GuiManager.OpenMenu<InventoryMenuPanel>();
		}

		public override void Open()
		{
			if (InventorySlotSelector.IsInUse)
				return;
			base.Open();
			InventorySlotSelector.StartUsing();
			InventorySlotUi.OnSelected += DisplayItemDetails;
			InventorySlotUi.OnSubmitted += OnSlotSubmitted;
			DisplayItemDetails(InventorySlotSelector.SelectedSlot);
		}

		public override void Close()
		{
			base.Close();
			InventorySlotSelector.EndUsing();
			InventorySlotUi.OnSelected -= DisplayItemDetails;
			InventorySlotUi.OnSubmitted -= OnSlotSubmitted;
		}
	}
}
