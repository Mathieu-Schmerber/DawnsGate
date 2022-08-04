using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game.Entities.Player;
using Game.Managers;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Interaction;
using Nawlian.Lib.Systems.Pooling;
using Pixelplacement;
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

		public static void Create(Vector3 position, ItemSummary summary, Vector3? horizontalMotion = null)
		{
			Vector3 launch = horizontalMotion != null ? horizontalMotion.Value : new Vector3(UnityEngine.Random.Range(-1f, 1f) * 2, 0, UnityEngine.Random.Range(-1f, 1f) * 2);
			var instance = ObjectPooler.Get(Databases.Database.Data.Item.LootedItem, position, Quaternion.Euler(0, 0, 0), summary, null);
			float groundLevel = GameManager.Player.transform.position.y;
			float parabolaSpeed = 0.7f;

			Tween.Position(instance.transform, position + launch, parabolaSpeed, 0, Tween.EaseLinear);
			Tween.Value(position.y, position.y + 2, (v) => instance.transform.position = instance.transform.position.WithY(v), parabolaSpeed / 2, 0, Tween.EaseOutStrong);
			Tween.Value(position.y + 2, groundLevel, (v) => instance.transform.position = instance.transform.position.WithY(v), parabolaSpeed / 2, parabolaSpeed / 2, Tween.EaseIn);
		}
	}
}
