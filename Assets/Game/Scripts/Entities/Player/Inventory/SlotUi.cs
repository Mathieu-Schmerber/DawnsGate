using Game.Managers;
using Game.Systems.Items;
using Nawlian.Lib.Extensions;
using Plugins.Nawlian.Lib.Systems.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Entities.Player.Inventory
{
	public class SlotUi : Selectable, ISubmitHandler
	{
		[SerializeField] private Image _itemImage;
		[SerializeField] private Transform _starList;
		[SerializeField] private Color _selectionColor;
		[SerializeField] private Color _unselectionColor;

		private Image[] _stars;
		private Outline _outline;
		private AEquippedItem _item;
		private Inventory _inventory;

		public static event Action<SlotUi> OnSelected;
		public AEquippedItem Item => _item;
		public bool Selected { get; private set; }

		protected override void Awake()
		{
			base.Awake();
			_outline = GetComponent<Outline>();
			_itemImage.enabled = false;
			_stars = _starList.GetComponentsInChildren<Image>(true);
			_stars.ForEach(x => x.gameObject.SetActive(false));
			_inventory = GameManager.Player.GetComponent<Inventory>();
			_outline.effectColor = _unselectionColor;
		}

		public void SetItem(AEquippedItem item)
		{
			if (item == null)
			{
				_item = null;
				_itemImage.enabled = false;
				_stars.ForEach(x => x.gameObject.SetActive(false));
				return;
			}
			_item = item;
			_itemImage.enabled = true;
			_itemImage.sprite = _item.Details.Graphics;
			for (int i = 0; i < _stars.Length; i++)
				_stars[i].gameObject.SetActive(i <= item.Quality);
		}

		public void Deselect()
		{
			_outline.effectColor = _unselectionColor;
		}

		public override void OnSelect(BaseEventData eventData)
		{
			Selected = true;
			base.OnSelect(eventData);
			_outline.effectColor = _selectionColor;
			OnSelected?.Invoke(this);
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			Selected = false;
			base.OnDeselect(eventData);
			_outline.effectColor = _unselectionColor;
		}

		public void OnSubmit(BaseEventData eventData)
		{
			if (_item == null || !interactable)
				return;
			_inventory.DropItem(_item.Details);
			OnSelected?.Invoke(this);
		}
	}
}
