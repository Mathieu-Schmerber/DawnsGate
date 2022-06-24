using Game.Managers;
using Game.Systems.Run.GPE;
using Game.Systems.Run.Rooms;
using TMPro;
using UnityEngine;

namespace Game.VFX
{
	public class ItemStandEffect : MonoBehaviour
	{
		private ItemStand _stand;
		[SerializeField] private GameObject _itemGfx;
		[SerializeField] private TextMeshPro _price;

		private void Awake()
		{
			_stand = GetComponentInParent<ItemStand>();
			_itemGfx.SetActive(false);
		}

		private void OnEnable()
		{
			GameManager.OnMoneyUpdated += UpdateItemPriceDisplay;
			ARoom.OnRoomActivated += ShowItem;
			_stand.OnItemPaid += RemovePrice;
			_stand.OnEquipped += HideItem;
		}

		private void OnDisable()
		{
			GameManager.OnMoneyUpdated -= UpdateItemPriceDisplay;
			ARoom.OnRoomActivated -= ShowItem;
			_stand.OnItemPaid -= RemovePrice;
			_stand.OnEquipped -= HideItem;
		}

		private void UpdateItemPriceDisplay(int before, int now) => _price.color = _stand.IsBuyable ? Color.white : Color.red;

		private void RemovePrice()
		{
			_price.gameObject.SetActive(false);
		}

		private void ShowItem()
		{
			// TODO: Show item on the stand in a smooth way
			_itemGfx.SetActive(true);
			_price.text = _stand.Cost.ToString();
			UpdateItemPriceDisplay(0, 0);
		}

		private void HideItem()
		{
			// TODO: Make item disappear in a smooth way
			_itemGfx.SetActive(false);
		}
	}
}
