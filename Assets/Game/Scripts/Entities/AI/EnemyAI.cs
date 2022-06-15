using Game.Entities.Shared;
using Game.Entities.Shared.Health;
using Game.Managers;
using Game.Systems.Run.Rooms;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Pooling;
using Nawlian.Lib.Utils;
using Pixelplacement;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Entities.AI
{
	public enum EnemyState
	{
		PASSIVE,
		AGGRESSIVE
	}

	[RequireComponent(typeof(Damageable))]
	public abstract class EnemyAI : AController, IPoolableObject
	{
		protected CombatRoom _room;
		protected NavMeshPath _path;
		protected EnemyState _aiState;
		protected Damageable _damageable;
		protected EnemyStatData _aiSettings;

		private Timer _attackCooldown;
		private int _pathPointIndex;
		private Vector3 _nextPatrolPosition;
		private Vector3 _nextAggressivePosition;
		private Vector3 _cachedDestination;

		protected abstract bool UsesPathfinding { get; }
		protected Vector3 NextPatrolPosition { get => _nextPatrolPosition.WithY(transform.position.y); set => _nextPatrolPosition = value; }
		protected Vector3 NextAggressivePosition { get => _nextAggressivePosition.WithY(transform.position.y); set => _nextAggressivePosition = value; }

		public event Action<EnemyState> OnStateChanged;

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
			_room = (CombatRoom)data;
			_aiSettings = (EnemyStatData)_entity.Stats;

			_aiState = EnemyState.PASSIVE;
			State = EntityState.IDLE;
			NextPatrolPosition = transform.position;

			_entity.ResetStats();
			_path.ClearCorners();

			_attackCooldown.AutoReset = false;
			_attackCooldown.Interval = _aiSettings.AttackCooldown;
		}

		protected override void Awake()
		{
			base.Awake();
			_path = new();
			_attackCooldown = new();
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
				CalculatePathfinding();
				UpdatePathIndex();
			}

			UpdateAiState();

			if (_aiState == EnemyState.AGGRESSIVE)
				TryAttacking();

			// Move
			base.Update();
		}
		
		#endregion

		#region Movement system

		protected override Vector3 GetMovementsInputs()
		{
			if (UsesPathfinding && _path.status != NavMeshPathStatus.PathInvalid)
				return (_path.corners[_pathPointIndex] - transform.position).normalized;
			return Vector3.zero;
		}

		protected override Vector3 GetTargetPosition()
		{
			if (UsesPathfinding && _path.status != NavMeshPathStatus.PathInvalid)
				return _path.corners[_pathPointIndex].WithY(transform.position.y);
			return Vector3.zero;
		}

		protected virtual Vector3 GetPathfindingDestination() => _aiState == EnemyState.PASSIVE ? UpdatePatrolPoint() : UpdateAgressivePoint();

		protected void UpdatePathIndex()
		{
			if (Vector3.Distance(transform.position, _path.corners[_pathPointIndex].WithY(transform.position.y)) < 0.5f && _pathPointIndex < _path.corners.Length - 1)
				_pathPointIndex++;
		}

		protected void CalculatePathfinding()
		{
			Vector3 destination = GetPathfindingDestination();

			if (_path.corners?.Length > 0)
			{
				// Debug
				for (int i = 0; i < _path.corners.Length - 1; i++)
					Debug.DrawLine(_path.corners[i], _path.corners[i + 1], Color.red);
			}

			if (_cachedDestination == destination)
				return;

			_pathPointIndex = 0;
			_cachedDestination = destination;

			Debug.Log("New path");
			NavMesh.CalculatePath(transform.position, _cachedDestination, NavMesh.AllAreas, _path);
		}

		protected virtual Vector3 UpdateAgressivePoint()
		{
			if (Vector3.Distance(transform.position, NextAggressivePosition) < 0.5f)
			{
				var aroundPos = _room.Info.GetPositionsAround(GameManager.Player.transform.position, _aiSettings.AttackRange / 2);

				NextAggressivePosition = aroundPos.Random();
			}
			return NextAggressivePosition;
		}

		protected virtual Vector3 UpdatePatrolPoint()
		{
			if (Vector3.Distance(transform.position, NextPatrolPosition) < 0.5f)
			{
				var aroundPos = _room.Info.GetPositionsAround(NextPatrolPosition, 5f);

				if (aroundPos.Length == 0)
					return NextPatrolPosition = _room.Info.Data.SpawnablePositions.Random();

				float maxDistance = aroundPos.Max(x => Vector3.Distance(x, NextPatrolPosition));

				NextPatrolPosition = aroundPos.Where(pos => Vector3.Distance(pos, NextPatrolPosition) == maxDistance).Random();
			}
			return NextPatrolPosition;
		}

		#endregion

		#region State

		private void UpdateAiState()
		{
			float distance = Vector3.Distance(transform.position, GameManager.Player.transform.position.WithY(transform.position.y));

			if (_aiState == EnemyState.PASSIVE && distance < _aiSettings.TriggerRange)
			{
				_aiState = EnemyState.AGGRESSIVE;
				NextAggressivePosition = transform.position;
				UpdateAgressivePoint();
				OnStateChanged?.Invoke(_aiState);
			}
			else if (_aiState == EnemyState.AGGRESSIVE && distance > _aiSettings.UntriggerRange)
			{
				_aiState = EnemyState.PASSIVE;
				NextPatrolPosition = transform.position;
				UpdatePatrolPoint();
				OnStateChanged?.Invoke(_aiState);
			}
		}

		#endregion

		#region Attack

		protected abstract void Attack();

		private void TryAttacking()
		{
			float distance = Vector3.Distance(transform.position, GameManager.Player.transform.position.WithY(transform.position.y));

			if (distance < _aiSettings.AttackRange && (_attackCooldown.IsOver() || !_attackCooldown.IsRunning) && State != EntityState.ATTACKING)
			{
				State = EntityState.ATTACKING;
				Attack();
				_attackCooldown.Restart();
			}
		}

		protected void OnAttackEnd()
		{
			State = EntityState.IDLE;
		}

		#endregion

		protected virtual void OnDeath()
		{
			_attackCooldown.Stop();
			_room.OnEnemyKilled(gameObject);
			Release();
		}
	}
}
