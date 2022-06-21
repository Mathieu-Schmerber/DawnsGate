using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Utils;
using Game.VFX;
using System;
using Pixelplacement;

namespace Game.Entities.Shared
{
	public enum EntityState
	{
		IDLE,
		DASH,
		STUN,
		ATTACKING
	}

	public struct DashParameters
	{
		public Vector3 Direction { get; set; }
		public float Distance { get; set; }
		public float Time { get; set; }
		public bool CanDashThroughWalls { get; set; }
	}

	[RequireComponent(typeof(EntityIdentity))]
	public abstract class AController : SerializedMonoBehaviour
	{
		#region Properties

		[SerializeField] private float _rotationSpeed = 6f;

		private Vector3 _lockedAim;
		private Vector3 _gfxAnchor;

		protected EntityIdentity _entity;
		protected Rigidbody _rb;
		protected Quaternion _desiredRotation;
		protected GameObject _graphics;
		protected Animator _gfxAnim;
		private Transform _target;
		private bool _lockMovement = false;
		private bool _lockAim = false;

		public event Action<DashParameters> OnDashStarted;

		public bool LockMovement { get => _lockMovement; 
			set { 
				_lockMovement = value; 
				if (_lockMovement == true)
					_rb.velocity = Vector3.zero; 
			} 
		}

		public Transform Graphics => _graphics.transform;
		protected Transform _lockedTarget => _target;
		[ShowInInspector, ReadOnly] public bool CanMove => !LockMovement && State != EntityState.DASH && State != EntityState.STUN;
		
		[ShowInInspector, ReadOnly] public bool LockAim { get => _lockAim; 
			set {
				if (value)
					_lockedAim = GetAimNormal();
				_lockAim = value;
			}
		}

		[ShowInInspector, ReadOnly] public EntityState State { get; set; }

		#endregion

		#region Unity builtins

		// Get references
		protected virtual void Awake()
		{
			_entity = GetComponent<EntityIdentity>();
			_rb = GetComponent<Rigidbody>();
			_gfxAnim = GetComponentInChildren<Animator>();
			_graphics = _gfxAnim?.gameObject;
			_desiredRotation = transform.rotation;
			_gfxAnchor = _graphics?.transform.localPosition ?? Vector3.zero;
			State = EntityState.IDLE;
		}

		// Initialization
		protected virtual void Update()
		{
			Vector3 dir = GetAimNormal();

			if (_graphics && !LockAim)
				ApplySmoothRotation(_graphics.transform, _gfxAnchor);

			Vector3 inputs = CanMove ? GetMovementsInputs() : Vector3.zero;

			_gfxAnim?.SetFloat("Speed", inputs.magnitude);
			if (!LockAim)
				_gfxAnim?.SetFloat("Angle", Vector3.Angle(inputs, dir));

			Move();
		}

		#endregion

		/// <summary>
		/// Gets the movement inputs
		/// </summary>
		/// <returns></returns>
		protected abstract Vector3 GetMovementsInputs();

		/// <summary>
		/// Gets the position of a target. <br/>
		/// Called if no target was locked.
		/// </summary>
		/// <returns></returns>
		protected abstract Vector3 GetTargetPosition();

		public Vector3 GetAimNormal()
		{
			if (LockAim)
				return _lockedAim;
			else if (_target != null)
				return (_target.position - _rb.position).normalized;
			return (GetTargetPosition() - _rb.position).normalized;
		}

		public Vector3 GetMovementNormal() => GetMovementsInputs().normalized;

		public void LockTarget(Transform target, bool forceRotation = false)
		{
			_target = target;
			if (forceRotation)
				_graphics.transform.rotation = Quaternion.LookRotation(GetAimNormal());
		}

		public void UnlockTarget() => _target = null;

		/// <summary>
		/// Rotation angle calculation and lerping
		/// </summary>
		protected void ApplySmoothRotation(Transform tr, Vector3 anchor)
		{
			_desiredRotation = Quaternion.LookRotation(GetAimNormal());
			tr.rotation = Quaternion.Slerp(tr.transform.rotation, _desiredRotation, _rotationSpeed * Time.deltaTime);
			tr.localPosition = anchor;
		}

		/// <summary>
		/// Movement speed calculation, and object motion
		/// </summary>
		protected virtual void Move()
		{
			if (!CanMove) return;

			transform.Translate(GetMovementNormal().WithY(0) * _entity.CurrentSpeed * Time.deltaTime);
		}

		public void Stun(float duration)
		{
			if (duration == 0)
				return;
			State = EntityState.STUN;
			Awaiter.WaitAndExecute(duration, () => State = EntityState.IDLE);
		}

		public void Dash(DashParameters parameters)
		{
			if (parameters.Distance == 0)
				return;

			Vector3 destination = (_rb.position + parameters.Direction * parameters.Distance).WithY(_rb.position.y);
			float speed = parameters.Distance / parameters.Time;

			OnDashStarted?.Invoke(parameters);
			_entity.SetInvulnerable(parameters.Time); // Invulnerable during dash

			if (!parameters.CanDashThroughWalls)
			{
				_rb.velocity = parameters.Direction.normalized * speed;
				State = EntityState.DASH;
				Awaiter.WaitAndExecute(parameters.Time, () =>
				{
					_rb.velocity = Vector3.zero;
					State = EntityState.IDLE;
				});
			}
			else
			{
				Tween.Position(transform, destination, parameters.Time, 0,
				startCallback: () =>
				{
					State = EntityState.DASH;
				},
				completeCallback: () =>
				{
					State = EntityState.IDLE;
				});
			}
		}

		public void Dash(Vector3 direction, float distance, float time, bool canDashWalls) => Dash(new DashParameters()
		{
			Direction = direction,
			Distance = distance,
			Time = time,
			CanDashThroughWalls = canDashWalls
		});

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying)
				return;
			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(transform.position, GetAimNormal() * 2);

			Gizmos.color = Color.green;
			Gizmos.DrawSphere(GetTargetPosition(), 0.2f);
		}
	}
}