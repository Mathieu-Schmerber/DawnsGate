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

	[RequireComponent(typeof(EntityIdentity), typeof(Damageable))]
	public abstract class EnemyAI : APoolableObject
	{
		protected CombatRoom _room;
		protected Damageable _damageable;
		protected EntityIdentity _identity;
		protected NavMeshPath _path;
		protected Rigidbody _rb;
		protected EnemyState _aiState;

		protected abstract bool UsesPathfinding { get; }

		#region Unity builtins

		public override void Init(object data)
		{
			_aiState = EnemyState.PATROL;
			_room = (CombatRoom)data;
			_identity.ResetStats();
			_path.ClearCorners();
		}

		protected void Awake()
		{
			_damageable = GetComponent<Damageable>();
			_identity = GetComponent<EntityIdentity>();
			_rb = GetComponent<Rigidbody>();
			_path = new NavMeshPath();
		}

		protected virtual void OnEnable()
		{
			_damageable.OnDeath += OnDeath;
		}

		protected virtual void OnDisable()
		{
			_damageable.OnDeath -= OnDeath;
		}

		protected void Update()
		{
			if (UsesPathfinding)
			{
				// TODO: cache GetTargetPosition() result, and only calculate path if the destination hasn't changed much
				NavMesh.CalculatePath(transform.position, GetTargetPosition(), NavMesh.AllAreas, _path);

				// Debug
				for (int i = 0; i < _path.corners.Length - 1; i++)
					Debug.DrawLine(_path.corners[i], _path.corners[i + 1], Color.red);
			}
			Move();
		}

		#endregion

		#region Movement system

		protected virtual void Move()
		{
			if (_path.status == NavMeshPathStatus.PathInvalid || _path.corners.Length <= 1)
				return;

			Vector3 destination = _path.corners[1].WithY(transform.position.y);
			Vector3 dir = (destination - transform.position).normalized;

			// Calculate how fast we should be moving
			Vector3 targetVelocity = transform.TransformDirection(dir);
			targetVelocity *= _identity.CurrentSpeed;

			// Apply a force that attempts to reach our target velocity
			var velocity = _rb.velocity;
			var velocityChange = targetVelocity - velocity;
			velocityChange.x = Mathf.Clamp(velocityChange.x, -_identity.CurrentSpeed, _identity.CurrentSpeed);
			velocityChange.z = Mathf.Clamp(velocityChange.z, -_identity.CurrentSpeed, _identity.CurrentSpeed);
			velocityChange.y = 0;
			_rb.AddForce(velocityChange, ForceMode.VelocityChange);

			// We apply gravity manually for more tuning control
			_rb.AddForce(new Vector3(0, -14f * _rb.mass, 0));
		}

		#endregion

		protected virtual void OnDeath()
		{
			_room.OnEnemyKilled(gameObject);
			Release();
		}

		protected abstract Vector3 GetTargetPosition();

		protected virtual float GetAgentSpeed() => _identity.CurrentSpeed;
	}
}
