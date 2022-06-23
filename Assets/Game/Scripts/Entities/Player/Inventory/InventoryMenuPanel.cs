using Game.Systems.Items;
using TMPro;
using UnityEngine;

namespace Game.Entities.Player.Inventory
{
	public class InventoryMenuPanel : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _itemName;
		[SerializeField] private TextMeshProUGUI _itemDescription;
		[SerializeField] private GameObject _effectPanel;
		[SerializeField] private TextMeshProUGUI _effectName;
		[SerializeField] private TextMeshProUGUI _effectDescription;

		private void OnEnable()
		{
			SlotUi.OnSelected += DisplayItemDetails;
		}

		private void OnDisable()
		{
			SlotUi.OnSelected -= DisplayItemDetails;
		}

		private void DisplayItemDetails(SlotUi obj)
		{
			_itemName.text = obj.Item == null ? "Empty slot" : obj.Item.Details.name;
			_itemDescription.text = obj.Item == null ? "" : obj.Item.GetDescription();

			_effectPanel.SetActive(false);
			if (obj != null && obj.Item != null && obj.Item.Details != null && obj.Item.Details.Type != ItemType.STAT)
			{
				SpecialItemData data = obj.Item.Details as SpecialItemData;

				if (data.ApplyEffect != null)
				{
					_effectPanel.SetActive(true);
					_effectName.text = data.ApplyEffect.DisplayName;
					_effectDescription.text = data.ApplyEffect.Description;
				}
			}
		}
	}
}
