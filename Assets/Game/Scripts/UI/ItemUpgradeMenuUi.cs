using Game.Managers;
using Game.Tools;
using Plugins.Nawlian.Lib.Systems.Menuing;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Game.UI
{
	public class ItemUpgradeMenuUi : AClosableMenu
	{
		[Title("References")]
		[SerializeField] private SimpleDescriptorUi _currentItemStage;
		[SerializeField] private SimpleDescriptorUi _nextItemStage;
		[SerializeField] private TextMeshProUGUI _priceTxt;
		[SerializeField] private Transform _interactionBox;

		#region Upgrade

		private void OnSlotSubmitted(InventorySlotUi slot)
		{
			if (slot.IsEmpty)
				return;
			else if (!slot.Item.IsAffordable || !slot.Item.HasUpgrade)
				return;
			slot.Item.OnUpgrade();
			OnItemSelected(slot);
		}

		#endregion

		#region Display

		private void DisplayStages(InventorySlotUi slot)
		{
			if (slot.IsEmpty)
			{
				_currentItemStage.Describe(null, 0);
				_nextItemStage.Describe(null, 0);
				return;
			}
			_currentItemStage.Describe(slot.Item.Details, slot.Item.Quality);
			if (slot.Item.HasUpgrade)
				_nextItemStage.Describe(slot.Item.Details, slot.Item.Quality + 1);
			else
				_nextItemStage.Describe(slot.Item.Details, "Maximum quality reached.");
		}

		private void DisplayInteractionBox(InventorySlotUi slot)
		{
			if (slot.IsEmpty)
				return;
			_interactionBox.gameObject.SetActive(slot.Item.IsAffordable && slot.Item.HasUpgrade);
		}

		private void DisplayPrice(InventorySlotUi slot)
		{
			if (slot.IsEmpty)
				return;
			_priceTxt.transform.parent.gameObject.SetActive(slot.Item.HasUpgrade);
			_priceTxt.text = slot.Item.NextUpgradePrice.ToString();
			_priceTxt.color = slot.Item.IsAffordable ? Color.white : Color.red;
		}

		private void OnItemSelected(InventorySlotUi slot)
		{
			DisplayStages(slot);
			DisplayPrice(slot);
			DisplayInteractionBox(slot);
		}

		#endregion

		public override void Open()
		{
			if (InventoryUi.IsInUse)
				return;
			base.Open();
			InventoryUi.StartUsing();
			InventorySlotUi.OnSelected += OnItemSelected;
			InventorySlotUi.OnSubmitted += OnSlotSubmitted;
			OnItemSelected(InventoryUi.SelectedSlot);
		}

		public override void Close()
		{
			base.Close();
			InventoryUi.EndUsing();
			InventorySlotUi.OnSelected -= OnItemSelected;
			InventorySlotUi.OnSubmitted -= OnSlotSubmitted;
		}
	}
}
