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

		private void DisplayItemDetails(InventorySlotUi obj)
		{
			_itemDescription.Describe(obj?.Item?.Details, obj?.Item?.Quality ?? 0);

			_effectDescription.gameObject.SetActive(false);
			if (obj != null && obj.Item != null && obj.Item.Details != null && obj.Item.Details.Type != ItemType.STAT)
			{
				SpecialItemData data = obj.Item.Details as SpecialItemData;

				if (data.ApplyEffect != null)
				{
					_effectDescription.gameObject.SetActive(true);
					_effectDescription.Describe(data.ApplyEffect);
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
			if (InventoryUi.IsInUse)
				return;
			base.Open();
			InventoryUi.StartUsing();
			InventorySlotUi.OnSelected += DisplayItemDetails;
			InventorySlotUi.OnSubmitted += OnSlotSubmitted;
			DisplayItemDetails(InventoryUi.SelectedSlot);
		}

		public override void Close()
		{
			base.Close();
			InventoryUi.EndUsing();
			InventorySlotUi.OnSelected -= DisplayItemDetails;
			InventorySlotUi.OnSubmitted -= OnSlotSubmitted;
		}
	}
}
