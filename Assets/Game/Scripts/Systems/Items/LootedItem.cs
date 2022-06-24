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
		public struct InitData
		{
			public ItemBaseData Data { get; set; }
			public int Quality { get; set; }
		}

		[SerializeField] protected ItemBaseData _data;
		[SerializeField] protected int _quality;

		protected SpriteRenderer _spr;

		public override string InteractionTitle => $"Pickup {(Data == null ? "item" : Data.name)}";
		public int Quality => _quality;
		public ItemBaseData Data => _data;

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
			if (_data != null)
				_spr.sprite = _data.Graphics;
		}

		private void Init(object data)
		{
			InitData init = (InitData)data;

			SetItem(init.Data, init.Quality);
		}

		protected virtual bool TryEquipItem(IInteractionActor actor)
		{
			Inventory inventory = (actor as MonoBehaviour).GetComponent<Inventory>();

			if (!inventory.HasAvailableSlot)
				return false;
			inventory.EquipItem(_data, _quality);
			OnEquipped?.Invoke();
			return true;
		}

		public virtual void SetItem(ItemBaseData item, int quality)
		{
			_data = item;
			_quality = quality;
			_spr.sprite = _data?.Graphics;
		}

		public override void Interact(IInteractionActor actor)
		{
			if (TryEquipItem(actor))
			{
				actor.UnSuggestInteraction(this);
				Release();
			}
		}

		public static void Create(Vector3 position, ItemBaseData data, int quality)
		{
			ObjectPooler.Get(Databases.Database.Data.Item.LootedItem, position, Quaternion.Euler(0, 0, 0), new InitData() { 
				Data = data,
				Quality = quality
			}, null);
		}
	}
}
