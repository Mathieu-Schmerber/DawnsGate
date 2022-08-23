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
		protected float _lastAttackTime;

		private int _pathPointIndex;
		private bool _aggressivityLock;
		private Vector3 _nextPatrolPosition;
		private Vector3 _nextAggressivePosition;
		private Vector3 _cachedDestination;

		protected abstract bool UsesPathfinding { get; }
		protected Vector3 NextPassivePosition { get => _nextPatrolPosition.WithY(transform.position.y); set => _nextPatrolPosition = value; }
		protected Vector3 NextAggressivePosition { get => _nextAggressivePosition.WithY(transform.position.y); set => _nextAggressivePosition = value; }

		public event Action<EnemyState> OnStateChanged;

		#region Settings

		protected virtual float AttackRange => _aiSettings.AttackRange;

		public void ForceAggressivity()
		{
			_aggressivityLock = true;
			_aiState = EnemyState.AGGRESSIVE;
		}

		protected virtual float AttackCooldown => _aiSettings.AttackCooldown;
		protected virtual float TriggerRange => _aiSettings.TriggerRange;
		protected virtual float UnTriggerRange => _aiSettings.UntriggerRange;

		#endregion

		#region IPoolableObject

		public bool Released => !gameObject.activeSelf;

		public GameObject Get()
		{
			gameObject.SetActive(true);
			return gameObject;
		}

		public virtual void Release()
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
			_entity.ResetStats();
			_aiSettings = _entity.Stats as EnemyStatData;
			_aiState = EnemyState.PASSIVE;
			_aggressivityLock = false;
			_path.ClearCorners();
			_lastAttackTime = Time.time;

			NextPassivePosition = transform.position;

			// Clear old state logic, since we pulled this from the pool
			OnAttackEnd();
			ResetStates();

			// Init polished state
			_entity.SetInvulnerable(true);
			State = EntityState.STUN;
			OnInitState();
		}

		protected override void Awake()
		{
			base.Awake();
			_path = new();
			_damageable = GetComponent<Damageable>();
		}

		protected virtual void OnEnable()
		{
			Damageable.OnDeath += OnDeath;
		}

		protected virtual void OnDisable()
		{
			Damageable.OnDeath -= OnDeath;
		}

		protected override void Update()
		{
			if (GameManager.Player.GetAnimatorState("IsDead"))
				return;

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
			if (UsesPathfinding)
			{
				if (_path.status != NavMeshPathStatus.PathInvalid)
					return (_path.corners[_pathPointIndex] - transform.position).normalized;
				else
					return (_room.Info.GetPositionsAround(transform.position, 5).First() - transform.position).normalized;
			}
			return Vector3.zero;
		}

		protected override Vector3 GetTargetPosition()
		{
			if (UsesPathfinding)
			{
				if (_path.status != NavMeshPathStatus.PathInvalid)
					return _path.corners[_pathPointIndex].WithY(transform.position.y);
				else
					return _room.Info.GetPositionsAround(transform.position, 5).First().WithY(transform.position.y);
			}
			return Vector3.zero;
		}

		protected virtual Vector3 GetPathfindingDestination() => _aiState == EnemyState.PASSIVE ? UpdatePassivePoint() : UpdateAgressivePoint();

		protected void UpdatePathIndex()
		{
			if (_pathPointIndex >= _path.corners.Length)
				return;
			else if (Vector3.Distance(transform.position, _path.corners[_pathPointIndex].WithY(transform.position.y)) < 0.5f && _pathPointIndex < _path.corners.Length - 1)
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

			NavMesh.CalculatePath(transform.position, _cachedDestination, NavMesh.AllAreas, _path);
		}

		protected virtual Vector3 CalculateNextAggressivePoint()
		{
			var aroundPos = _room.Info.GetPositionsAround(GameManager.Player.transform.position, AttackRange / 2);

			if (aroundPos?.Length == 0)
				return _room.Info.Data.SpawnablePositions.Random();
			return aroundPos.Random();
		}

		protected virtual Vector3 CalculateNextPassivePoint()
		{
			var aroundPos = _room.Info.GetPositionsAround(NextPassivePosition, 5f);

			if (aroundPos.Length == 0)
				return _room.Info.Data.SpawnablePositions.Random();

			float maxDistance = aroundPos.Max(x => Vector3.Distance(x, NextPassivePosition));

			return aroundPos.Where(pos => Vector3.Distance(pos, NextPassivePosition) == maxDistance).Random();
		}

		protected virtual Vector3 UpdateAgressivePoint()
		{
			if (Vector3.Distance(transform.position, NextAggressivePosition) < 0.5f)
				NextAggressivePosition = CalculateNextAggressivePoint();
			return NextAggressivePosition;
		}

		protected Vector3 UpdatePassivePoint()
		{
			if (Vector3.Distance(transform.position, NextPassivePosition) < 0.5f)
				NextPassivePosition = CalculateNextPassivePoint();
			return NextPassivePosition;
		}

		#endregion

		#region State

		/// <summary>
		/// Called when the entity spawns from the pool, you can hook up some polish logic.<br/>
		/// At this point the entity is STUNNED and invulnerable, so the AI wont be active.<br/>
		/// Don't forget to call ResetStates() at the end of the implementation.
		/// </summary>
		protected virtual void OnInitState()
		{
			ResetStates();
			_entity.SetInvulnerable(false);
		}
	
		protected virtual void ResetStates()
		{
			State = Shared.EntityState.IDLE;
			LockAim = false;
			LockMovement = false;
			UnlockTarget();
		}

		protected virtual void UpdateAiState()
		{
			float distance = Vector3.Distance(transform.position, GameManager.Player.transform.position.WithY(transform.position.y));

			if (_aiState == EnemyState.PASSIVE && distance < TriggerRange)
			{
				if (!_aggressivityLock)
					_aiState = EnemyState.AGGRESSIVE;
				NextAggressivePosition = transform.position;
				UpdateAgressivePoint();
				OnStateChanged?.Invoke(_aiState);
			}
			else if (_aiState == EnemyState.AGGRESSIVE && distance > UnTriggerRange)
			{
				if (!_aggressivityLock)
					_aiState = EnemyState.PASSIVE;
				NextPassivePosition = transform.position;
				UpdatePassivePoint();
				OnStateChanged?.Invoke(_aiState);
			}
		}

		#endregion

		#region Attack

		protected abstract void Attack();

		protected virtual void TryAttacking()
		{
			float distance = Vector3.Distance(transform.position, GameManager.Player.transform.position.WithY(transform.position.y));

			if (distance < AttackRange && Time.time - _lastAttackTime >= AttackCooldown && State == EntityState.IDLE)
			{
				State = EntityState.ATTACKING;
				Attack();
			}
		}

		protected void OnAttackEnd()
		{
			if (State != EntityState.STUN)
				State = EntityState.IDLE;
			_lastAttackTime = Time.time;
		}

		#endregion

		protected virtual void OnDeath(Damageable damageable)
		{
			if (damageable == _damageable)
			{
				_room.OnEnemyKilled(gameObject);
				Release();
			}
		}
	}
}
