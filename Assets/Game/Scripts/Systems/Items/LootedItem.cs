using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Entities.Player;
using Nawlian.Lib.Systems.Interaction;
using Nawlian.Lib.Systems.Pooling;
using UnityEngine;

namespace Game.Systems.Items
{
	public class LootedItem : ATriggerInteractable, IPoolableObject
	{
		[SerializeField] protected ItemSummary _item;

		protected SpriteRenderer _spr;
		public override string InteractionTitle => $"Pickup {(Item == null ? "item" : Item.name)}";
		public int Quality => _item?.Quality ?? 0;
		public ItemBaseData Item => _item?.Data;
		public ItemSummary Summary => _item;

		public event Action OnEquipped;

		#region IPoolableObject

		public bool Released => !gameObject.activeSelf;


		public GameObject Get()
		{
			gameObject.SetActive(true);
			return gameObject;
		}

		public void Release()
		{
			gameObject.SetActive(false);
			gameObject.transform.SetParent(ObjectPooler.Instance.transform);
		}

		public void InitFromPool(object data) => Init(data);

		#endregion

		protected virtual void Awake()
		{
			_spr = GetComponentInChildren<SpriteRenderer>();
		}

		private void Start()
		{
			if (Item != null)
				_spr.sprite = Item.Graphics;
		}

		private void Init(object data) => SetItem((ItemSummary)data);

		protected virtual bool TryEquipItem(IInteractionActor actor)
		{
			Inventory inventory = (actor as MonoBehaviour).GetComponent<Inventory>();

			if (!inventory.HasAvailableSlot)
				return false;
			inventory.EquipItem(_item);
			OnEquipped?.Invoke();
			return true;
		}

		public virtual void SetItem(ItemSummary summary)
		{
			_item = summary;
			_spr.sprite = Item?.Graphics;
		}

		public override void Interact(IInteractionActor actor)
		{
			if (TryEquipItem(actor))
			{
				actor.UnSuggestInteraction(this);
				Release();
			}
		}

		public static void Create(Vector3 position, ItemSummary summary)
		{
			ObjectPooler.Get(Databases.Database.Data.Item.LootedItem, position, Quaternion.Euler(0, 0, 0), summary, null);
		}

#if UNITY_EDITOR

		private void OnValidate()
		{
			if (!Application.isPlaying)
				GetComponentInChildren<SpriteRenderer>().sprite = Item?.Graphics;
		}

#endif
	}
}
