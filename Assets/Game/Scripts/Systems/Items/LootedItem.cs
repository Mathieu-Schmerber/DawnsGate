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

		[SerializeField] private ItemBaseData _data;
		[SerializeField] private int _quality;

		private SpriteRenderer _spr;

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

		private void Awake()
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

			_data = init.Data;
			_quality = init.Quality;
			_spr.sprite = _data.Graphics;
		}

		public override void Interact(IInteractionActor actor)
		{
			Inventory inventory = (actor as MonoBehaviour).GetComponent<Inventory>();

			inventory.EquipItem(_data, _quality);
			actor.UnSuggestInteraction(this);
			Release();
		}

		public static void Create(Vector3 position, ItemBaseData data, int quality)
		{
			ObjectPooler.Get(Databases.Database.Data.Item.LootedItem, position, Quaternion.identity, new InitData() { 
				Data = data,
				Quality = quality
			}, null);
		}
	}
}
