using Game.Entities.Shared;
using Game.Entities.Shared.Health;
using Game.Systems.Run.Rooms;
using Nawlian.Lib.Systems.Pooling;
using UnityEngine;

namespace Game.Entities.AI
{
	public class EnemyAI : AController, IPoolableObject
	{
		private CombatRoom _room;
		private Damageable _damageable;
		private EntityIdentity _identity;

		#region Pooling

		public bool Released => !gameObject.activeSelf;

		public GameObject Get()
		{
			gameObject.SetActive(true);
			return gameObject;
		}

		public void InitFromPool(object data)
		{
			_room = (CombatRoom)data;
			_identity.ResetStats();
		}

		public void Release()
		{
			gameObject.SetActive(false);
			gameObject.transform.SetParent(ObjectPooler.Instance.transform);
		}

		#endregion

		protected override Vector3 GetMovementsInputs() => Vector3.zero;
		protected override Vector3 GetTargetPosition() => Vector3.zero;

		protected override void Awake()
		{
			base.Awake();
			_damageable = GetComponent<Damageable>();
			_identity = GetComponent<EntityIdentity>();
		}

		private void OnEnable()
		{
			_damageable.OnDeath += OnDeath;
		}

		private void OnDisable()
		{
			_damageable.OnDeath -= OnDeath;
		}

		private void OnDeath()
		{
			_room.OnEnemyKilled(gameObject);
			Release();
		}

		
	}
}
