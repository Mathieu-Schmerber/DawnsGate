using Game.Managers;
using Game.Systems.Run.GPE;
using Game.Systems.Run.Rooms;
using Nawlian.Lib.Systems.Pooling;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.VFX
{
	public class ItemStandEffect : MonoBehaviour
	{
		private ItemStand _stand;
		private AudioSource _source;
		[SerializeField] private GameObject _purchaseFx;
		[SerializeField] private GameObject _itemGfx;
		[SerializeField] private TextMeshPro _price;

		private void Awake()
		{
			_stand = GetComponentInParent<ItemStand>();
			_itemGfx.SetActive(false);
			_source = GetComponent<AudioSource>();
		}

		private void OnEnable()
		{
			GameManager.OnRunMoneyUpdated += UpdateItemPriceDisplay;
			ARoom.OnRoomActivated += ShowItem;
			_stand.OnItemPaid += RemovePrice;
			_stand.OnEquipped += HideItem;
		}

		private void OnDisable()
		{
			GameManager.OnRunMoneyUpdated -= UpdateItemPriceDisplay;
			ARoom.OnRoomActivated -= ShowItem;
			_stand.OnItemPaid -= RemovePrice;
			_stand.OnEquipped -= HideItem;
		}

		private void UpdateItemPriceDisplay(int before, int now)
		{
			if (_stand.WasBought)
			{
				_price.enabled = false;
				return;
			}
			_price.text = $"{ _stand.Cost}{(_stand.Item.IsLifeItem ? "<color=red>♥</color>" : "")}";
			_price.color = _stand.IsBuyable ? Color.white : Color.red;
		}

		private void RemovePrice()
		{
			_source.Play();
			ObjectPooler.Get(_purchaseFx, _itemGfx.transform.position, Quaternion.identity, null);
			_price.gameObject.SetActive(false);
		}

		private void ShowItem()
		{
			if (_stand.Item == null)
				return;
			_price.gameObject.SetActive(true);
			_itemGfx.SetActive(true);
			UpdateItemPriceDisplay(0, 0);
		}

		private void HideItem()
		{
			_itemGfx.SetActive(false);
		}
	}
}
