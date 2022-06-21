using TMPro;
using UnityEngine;

namespace Game.Entities.Player.Inventory
{
	public class InventoryMenuPanel : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _itemName;
		[SerializeField] private TextMeshProUGUI _itemDescription;

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
		}
	}
}
