using Game.Managers;
using Game.Tools;
using Pixelplacement;
using Plugins.Nawlian.Lib.Systems.Menuing;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class ItemUpgradeMenuUi : ACloseableMenu
	{
		[Title("References")]
		[SerializeField] private Transform _globalPanel;
		[SerializeField] private Transform _emptyPanel;
		[SerializeField] private SimpleDescriptorUi _currentItemStage;
		[SerializeField] private SimpleDescriptorUi _nextItemStage;
		[SerializeField] private TextMeshProUGUI _priceTxt;
		[SerializeField] private Transform _interactionBox;
		[SerializeField] private Button _purchaseBtn;

		[Title("Feedback")]
		[SerializeField] private Color _flashColor;
		[SerializeField] private float _bumpIntensity;
		[SerializeField] private AudioClip _errorAudio;

		private Color _currentDefaultColor;
		private Color _nextDefaultColor;
		private Image _currentRenderer;
		private Image _nextRenderer;

		private InventorySlotUi _lastSelectedSlot;

		protected override void Awake()
		{
			base.Awake();
			_currentRenderer = _currentItemStage.GetComponent<Image>();
			_nextRenderer = _nextItemStage.GetComponent<Image>();
			_currentDefaultColor = _currentRenderer.color;
			_nextDefaultColor = _nextRenderer.color;
		}

		private void Start()
		{
			_purchaseBtn.onClick.AddListener(Submit);
		}

		#region Upgrade

		private void Feedback()
		{
			// Flash
			Tween.Value(_currentDefaultColor, _flashColor, (c) => _currentRenderer.color = c, _duration / 2, 0);
			Tween.Value(_flashColor, _currentDefaultColor, (c) => _currentRenderer.color = c, _duration / 2, _duration / 2);
			Tween.Value(_nextDefaultColor, _flashColor, (c) => _nextRenderer.color = c, _duration / 2, 0);
			Tween.Value(_flashColor, _nextDefaultColor, (c) => _nextRenderer.color = c, _duration / 2, _duration / 2);

			// Bump
			Tween.LocalScale(_interactionBox, Vector3.one * _bumpIntensity, _duration / 2, 0, Tween.EaseBounce);
			Tween.LocalScale(_interactionBox, Vector3.one, _duration / 2, _duration / 2, Tween.EaseBounce);
		}

		private void Submit()
		{
			InventorySlotUi slot = _lastSelectedSlot;

			if (slot != null && slot.IsEmpty)
				return;
			else if (!slot.Item.IsAffordable || !slot.Item.HasUpgrade)
			{
				_source.PlayOneShot(_errorAudio);
				return;
			}
			GameManager.PayWithRunMoney(slot.Item.NextUpgradePrice);
			slot.Item.Upgrade();
			slot.Select();
			Feedback();
		}

		#endregion

		#region Display

		private void DisplayStages(InventorySlotUi slot)
		{
			if (slot.IsEmpty)
			{
				_currentItemStage.DescribeItem(null);
				_nextItemStage.DescribeItem(null);
				return;
			}
			_currentItemStage.DescribeItem(slot.Item);
			if (slot.Item.HasUpgrade)
				_nextItemStage.DescribeItem(slot.Item, slot.Item.Quality + 1);
			else
				_nextItemStage.Describe(slot.Item.Details.name, "Maximum quality reached.", 0);
		}

		private void DisplayInteractionBox(InventorySlotUi slot)
			=> _purchaseBtn.interactable = slot.Item != null && slot.Item.HasUpgrade && slot.Item.IsAffordable && !slot.IsEmpty;

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
			_lastSelectedSlot = slot;
			_globalPanel.gameObject.SetActive(InventorySlotSelector.HasUsableSlot);
			_emptyPanel.gameObject.SetActive(!InventorySlotSelector.HasUsableSlot);
			if (slot == null)
				return;
			DisplayStages(slot);
			DisplayPrice(slot);
			DisplayInteractionBox(slot);
		}

		#endregion

		public override void Open()
		{
			if (InventorySlotSelector.IsInUse)
				return;
			base.Open();
			_globalPanel.gameObject.SetActive(InventorySlotSelector.HasUsableSlot);
			_emptyPanel.gameObject.SetActive(!InventorySlotSelector.HasUsableSlot);
			InventorySlotSelector.StartUsing(x => !x.IsEmpty && x.Item.HasUpgrade);
			InventorySlotSelector.SetRightNavigation(_purchaseBtn);
			InventorySlotUi.OnSelected += OnItemSelected;
			OnItemSelected(InventorySlotSelector.SelectedSlot);
		}

		public override void Close()
		{
			base.Close();
			InventorySlotSelector.EndUsing();
			InventorySlotSelector.SetRightNavigation(null);
			InventorySlotUi.OnSelected -= OnItemSelected;
			//InventorySlotUi.OnSubmitted -= OnSlotSubmitted;
		}
	}
}
