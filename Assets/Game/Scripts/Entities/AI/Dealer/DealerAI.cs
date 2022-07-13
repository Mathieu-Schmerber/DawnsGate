using Game.Entities.Shared;
using Game.Managers;
using Game.Systems.Combat.Attacks;
using Game.Systems.Run.Rooms;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Animations;
using Nawlian.Lib.Utils;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Entities.AI.Dealer
{
	public class DealerAI : EnemyAI, IAnimationEventListener
	{
		[SerializeField] private LayerMask _wallMask;
		private DealerStatData _stats;

		#region Unity builtins

		protected override void OnEnable()
		{
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		protected override void Init(object data)
		{
			base.Init(data);
			_stats = _entity.Stats as DealerStatData;

			_attackNumber = 0;
			State = EntityState.STUN;
			_entity.SetInvulnerable(true);
		}

		#endregion

		#region Movement

		protected override bool UsesPathfinding => true;
		protected override float TriggerRange => Mathf.Infinity;
		protected override float UnTriggerRange => Mathf.Infinity;

		#endregion

		#region States

		protected override void ResetStates()
		{
			base.ResetStates();
			_gfxAnim.SetBool("IsDashing", false);
		}

		public void TakeAction()
		{
			_room.Activate();

			// TODO: game feel before starting the fight here

			// Set AI as active
			ResetStates();
		}

		#endregion

		#region Attacks

		protected override float AttackRange => _stats.DashAttack.AttackRange;
		protected override float AttackCooldown => 0.1f;

		private int _attackNumber = 0;
		private int _dashToPerform;

		private bool _isDashAttack => _attackNumber % 2 == 0;

		protected override void Attack()
		{
			if (_isDashAttack)
			{
				if (_dashToPerform == 0)
					_dashToPerform = Random.Range(_stats.ConsecutiveDashes.x, _stats.ConsecutiveDashes.y + 1);
				_dashToPerform--;
				_gfxAnim.Play(_stats.StartDashAnimation.name);
			}
			else
			{
				// Go to the center of the map
				// perform laser atack
				OnAttackEnd(); // tmp
			}
			if (_dashToPerform == 0)
				_attackNumber++;
		}

		public void OnAnimationEvent(string animationArg)
		{
			if (_isDashAttack && animationArg == "Attack")
			{
				ModularAttack instance = AttackBase.Spawn(_stats.DashAttack, transform.position, Quaternion.LookRotation(GetAimNormal()), new()
				{
					Caster = _entity,
					Data = _stats.DashAttack
				}).GetComponent<ModularAttack>();
				instance.OnStart(Vector3.zero, 0);
				DashToWall();
			}
		}

		public void OnAnimationEnter(AnimatorStateInfo stateInfo)
		{
			if (stateInfo.IsName(_stats.StartDashAnimation.name))
			{
				LockTarget(GameManager.Player.transform);
				LockMovement = true;
			}
			else if (stateInfo.IsName("Dash"))
			{
				LockTarget(GameManager.Player.transform, true);
				LockAim = true;
			}
		}

		public void OnAnimationExit(AnimatorStateInfo stateInfo)
		{
			if (stateInfo.IsName("Dash") && !_gfxAnim.GetBool("IsDashing") && LockMovement)
			{
				ResetStates();
				OnAttackEnd();
			}
		}

		private float GetDistanceToWall()
		{
			if (Physics.Raycast(transform.position, GetAimNormal(), out RaycastHit hit, Mathf.Infinity, _wallMask))
				return Vector3.Distance(transform.position.WithY(_room.GroundLevel), hit.point.WithY(_room.GroundLevel));
			return 0;
		}

		private void DashToWall()
		{
			const float SPEED = 50f; // 5m in 0.1s
			float distanceToWall = GetDistanceToWall();
			float dashTime = distanceToWall / SPEED;

			_gfxAnim.SetBool("IsDashing", true);
			Dash(GetAimNormal(), distanceToWall, dashTime, false, false);
			Awaiter.WaitAndExecute(dashTime, () =>
			{
				ResetStates();
				OnAttackEnd();
			});
		}

		#endregion
	}
}
