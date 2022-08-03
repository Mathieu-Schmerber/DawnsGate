using Game.Entities.Shared;
using Game.Managers;
using Game.Systems.Combat.Attacks;
using Game.Systems.Run.Rooms.Events;
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
		[SerializeField] private Vector3 _defaultRotationNormal;
		[SerializeField] private LayerMask _wallMask;
		private DealerStatData _stats;
		private DealRoom _dealRoom;
		private bool _atTheMapCenter = false;
		private bool _activated = false;

		#region Unity builtins

		protected override void Init(object data)
		{
			base.Init(data);
			_stats = _entity.Stats as DealerStatData;
			_dealRoom = _room as DealRoom;
			_attackNumber = 0;
			_activated = false;
		}

		#endregion

		#region Movement

		protected override bool UsesPathfinding => true;
		protected override float TriggerRange => Mathf.Infinity;
		protected override float UnTriggerRange => Mathf.Infinity;

		protected override void Update()
		{
			base.Update();

			_atTheMapCenter = Vector3.Distance(transform.position, _dealRoom.BossSpawnPoint.WithY(transform.position.y)) <= 0.5f;
			if (!IsDashAttack)
				NextAggressivePosition = _dealRoom.BossSpawnPoint;
		}

		#endregion

		#region States

		protected override void OnInitState()
		{
			// We don't want to activate the AI here, the room should take care of it with TakeAction()
		}

		protected override Vector3 GetTargetPosition()
		{
			if (_activated)
				return base.GetTargetPosition();
			return transform.position - transform.right;
		}

		protected override void ResetStates()
		{
			base.ResetStates();
			_gfxAnim.SetBool("IsDashing", false);
			_gfxAnim.SetBool("IsCasting", false);
		}

		public void TakeAction()
		{
			_activated = true;
			_room.Activate();

			// TODO: game feel before starting the fight here

			// Set AI as active
			ResetStates();
		}

		#endregion

		#region Attacks

		protected override float AttackRange => _stats.DashAttack.AttackRange;
		protected override float AttackCooldown => 0.5f;

		private int _attackNumber = 0;
		private int _dashToPerform;
		public bool IsDashAttack => _attackNumber % 2 == 0;

		protected override void TryAttacking()
		{
			if (!IsDashAttack && !_atTheMapCenter)
				return;
			base.TryAttacking();
		}

		protected override void Attack()
		{
			if (IsDashAttack)
			{
				if (_dashToPerform == 0)
					_dashToPerform = Random.Range(_stats.ConsecutiveDashes.x + 1, _stats.ConsecutiveDashes.y + 2);
				_dashToPerform--;
				AttackBase.ShowAttackPrevisu(_stats.DashAttack, transform.position, .5f, this, 
					OnUpdate: (param) =>
					{
						param.Transform.localScale = new Vector3(1, 1, Vector3.Distance(transform.position, transform.position + GetAimNormal() * GetDistanceToWall()));
					});
				_gfxAnim.Play(_stats.StartDashAnimation.name);
				if (_dashToPerform == 0)
				{
					_attackNumber++;
				}
			}
			else
			{
				_gfxAnim.SetBool("IsCasting", true);
				LockMovement = true;
				LockAim = true;
			}
		}

		public void OnAnimationEvent(string animationArg)
		{
			if (animationArg != "Attack")
				return;
			if (IsDashAttack)
			{
				ModularAttack instance = AttackBase.Spawn(_stats.DashAttack, transform.position, Quaternion.LookRotation(GetAimNormal()), new()
				{
					Caster = _entity,
					Data = _stats.DashAttack
				}).GetComponent<ModularAttack>();
				instance.OnStart(Vector3.zero, 0);
				DashToWall();
			}
			else if (_atTheMapCenter)
			{
				ModularAttack instance = AttackBase.Spawn(_stats.LaserAttack, transform.position, Quaternion.LookRotation(GetAimNormal()), new()
				{
					Caster = _entity,
					Data = _stats.LaserAttack
				}).GetComponent<ModularAttack>();
				instance.OnStart(Vector3.zero, 0);
				Awaiter.WaitAndExecute(_stats.LaserAttack.ActiveTime, () =>
				{
					ResetStates();
					OnAttackEnd();
					_attackNumber++;
				});
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
			Ray ray = new Ray(transform.position + Vector3.up, GetAimNormal());

			if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _wallMask))
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
