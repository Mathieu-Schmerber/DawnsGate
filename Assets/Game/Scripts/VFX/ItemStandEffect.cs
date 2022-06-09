using Game.Systems.Run.GPE;
using Game.Systems.Run.Rooms;
using UnityEngine;

namespace Game.VFX
{
	public class ItemStandEffect : MonoBehaviour
	{
		private ItemStand _stand;
		private GameObject _itemGfx;

		private void Awake()
		{
			_stand = GetComponentInParent<ItemStand>();
			// TMP, test purpose
			_itemGfx = transform.GetChild(0).gameObject;
			_itemGfx.SetActive(false);
		}

		private void OnEnable()
		{
			ARoom.OnRoomActivated += ShowItem;
			_stand.OnItemPaid += OnItemSold;
		}

		private void OnDisable()
		{
			ARoom.OnRoomActivated -= ShowItem;
			_stand.OnItemPaid -= OnItemSold;
		}

		private void ShowItem()
		{
			// TODO: Show item on the stand
			// TMP, test purpose
			_itemGfx.SetActive(true);
		}

		private void OnItemSold()
		{
			// TODO: Make item dissapear
			// TMP, test purpose
			_itemGfx.SetActive(false);
		}
	}
}
