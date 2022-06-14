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
		private Vector3 _nextPatrolPosition;

		protected abstract bool UsesPathfinding { get; }
		protected Vector3 NextPatrolPosition { get => _nextPatrolPosition.WithY(transform.position.y); set => _nextPatrolPosition = value; }

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
				// TODO: cache GetTargetPosition() result, and only calculate path if the destination hasn't changed much
				NavMesh.CalculatePath(transform.position, GetPathfindingDestination(), NavMesh.AllAreas, _path);

				// Debug
				for (int i = 0; i < _path.corners.Length - 1; i++)
					Debug.DrawLine(_path.corners[i], _path.corners[i + 1], Color.red);
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
				return (_path.corners[1] - transform.position);
			return Vector3.zero;
		}

		protected override Vector3 GetTargetPosition()
		{
			if (UsesPathfinding && _path.status != NavMeshPathStatus.PathInvalid)
				return _path.corners[1];
			return Vector3.zero;
		}

		protected virtual Vector3 UpdatePatrolPoint()
		{
			if (Vector3.Distance(transform.position, NextPatrolPosition) < 0.1f)
			{
				var aroundPos = _room.Info.GetPositionsAround(NextPatrolPosition, 5f);
				float maxDistance = aroundPos.Max(x => Vector3.Distance(x, NextPatrolPosition));

				NextPatrolPosition = aroundPos.Where(pos => Vector3.Distance(pos, NextPatrolPosition) == maxDistance).Random();
			}
			return NextPatrolPosition;
		}

		protected virtual Vector3 GetPathfindingDestination() => _aiState == EnemyState.PASSIVE ? UpdatePatrolPoint() : GameManager.Player.transform.position;

		#endregion

		#region State

		private void UpdateAiState()
		{
			float distance = Vector3.Distance(transform.position, GameManager.Player.transform.position.WithY(transform.position.y));

			if (_aiState == EnemyState.PASSIVE && distance < _aiSettings.TriggerRange)
			{
				_aiState = EnemyState.AGGRESSIVE;
				OnStateChanged?.Invoke(_aiState);
			}
			else if (_aiState == EnemyState.AGGRESSIVE && distance > _aiSettings.UntriggerRange)
			{
				_aiState = EnemyState.PASSIVE;
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

		#endregion

		protected virtual void OnDeath()
		{
			_attackCooldown.Stop();
			_room.OnEnemyKilled(gameObject);
			Release();
		}
	}
}
