using Game.Entities.Shared;
using Game.Entities.Shared.Health;
using Game.Managers;
using Game.Systems.Run.Rooms;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Pooling;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Entities.AI
{
	public enum EnemyState
	{
		PATROL,
		CHASE
	}

	[RequireComponent(typeof(Damageable))]
	public abstract class EnemyAI : AController, IPoolableObject
	{
		protected CombatRoom _room;
		protected NavMeshPath _path;
		protected EnemyState _aiState;
		protected Damageable _damageable;

		protected abstract bool UsesPathfinding { get; }

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

		#region Unity builtins

		protected virtual void Init(object data)
		{
			_aiState = EnemyState.PATROL;
			_room = (CombatRoom)data;
			_entity.ResetStats();
			_path.ClearCorners();
		}

		protected override void Awake()
		{
			base.Awake();
			_path = new NavMeshPath();
			_damageable = GetComponent<Damageable>();
		}

		protected virtual void OnEnable()
		{
			_damageable.OnDeath += OnDeath;
		}

		protected virtual void OnDisable()
		{
			_damageable.OnDeath -= OnDeath;
		}

		protected override void Update()
		{
			if (UsesPathfinding)
			{
				// TODO: cache GetTargetPosition() result, and only calculate path if the destination hasn't changed much
				NavMesh.CalculatePath(transform.position, GetPathfindingDestination(), NavMesh.AllAreas, _path);

				// Debug
				for (int i = 0; i < _path.corners.Length - 1; i++)
					Debug.DrawLine(_path.corners[i], _path.corners[i + 1], Color.red);
			}
			base.Update();
		}

		#endregion

		#region Movement system

		protected override Vector3 GetMovementsInputs()
		{
			if (UsesPathfinding && _path.status != NavMeshPathStatus.PathInvalid)
				return (_path.corners[1] - transform.position);
			return Vector3.zero;
		}

		protected override Vector3 GetTargetPosition()
		{
			if (UsesPathfinding && _path.status != NavMeshPathStatus.PathInvalid)
				return _path.corners[1];
			return Vector3.zero;
		}

		protected virtual Vector3 GetPathfindingDestination() => GameManager.Player.transform.position;

		#endregion

		protected virtual void OnDeath()
		{
			_room.OnEnemyKilled(gameObject);
			Release();
		}
	}
}
