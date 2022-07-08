using Game.Entities.AI;
using Game.Entities.Shared.Health;
using Game.Managers;
using Game.Systems.Combat.Attacks;
using Game.Systems.Run.Rooms;
using Game.VFX.Previsualisations;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Systems.Animations;
using Nawlian.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Entities.Lunaris
{
	public class LunarisAI : EnemyAI, IAnimationEventListener
	{
		[SerializeField] private MeshFilter _weaponMesh;
		private LunarisStatData _stats;
		private Timer _dashTimer = new();

		private bool _canDash => CanMove && _dashTimer.IsOver();

		private LunarisStatData.PhaseSettings _phase =>
			_phaseIndex switch
			{
				LunarisPhase.SCYTHE => _stats.ScythePhase,
				LunarisPhase.KATANA => _stats.KatanaPhase,
				LunarisPhase.STAFF => _stats.StaffPhase,
				_ => throw new ArgumentOutOfRangeException(nameof(_phaseIndex), $"Not expected value: {_phaseIndex}"),
			};

		#region Unity builtins

		protected override void Awake()
		{
			base.Awake();
		}

		protected override void Init(object data)
		{
			base.Init(data);
			_stats = _entity.Stats as LunarisStatData;
			ResetStates();
			_phaseIndex = 0;
			State = Shared.EntityState.STUN;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			ARoom.OnRoomActivated += EngageFight;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			_passiveTimer.Stop();
			ARoom.OnRoomActivated -= EngageFight;
		}

		protected override void Update()
		{
			if (!_damageable.IsDead)
				base.Update();
		}

		#endregion

		#region States

		private readonly LunarisPhase LAST_PHASE = LunarisPhase.STAFF;
		private LunarisPhase _phaseIndex = 0;
		internal bool IsLastPhase => _phaseIndex == LAST_PHASE;

		public void EngageFight()
		{
			_dashTimer.Start(_stats.DashCooldown, false);
			OnPhaseSet();
		}

		internal void SetNextPhase()
		{
			_phaseIndex++;
			OnPhaseSet();
		}

		internal void OnPhaseSet()
		{
			// Stop passive from spawning while switching phase
			_passiveTimer.Stop();
			_attackStack.Clear();

			// Cannot damage the boss any further
			_entity.SetInvulnerable(true);

			// Restricting the boss from doing anything
			ResetStates();
			_gfxAnim.StopPlayback();
			State = Shared.EntityState.STUN;

			// Executing the switched state code after waiting for a while
			AttackBase.ShowAttackPrevisu(_stats.PhaseSwitchAttack, transform.position.WithY(_room.GroundLevel), _stats.PhaseSwitchTime, this);
			Awaiter.WaitAndExecute(_stats.PhaseSwitchTime, OnReadyToStartNewPhase);
		}

		private void OnReadyToStartNewPhase()
		{
			if (RunManager.RunState == RunState.LOBBY)
				return;

			// Spawn explosion
			AttackBase.Spawn(_stats.PhaseSwitchAttack, transform.position.WithY(_room.GroundLevel), Quaternion.identity, new()
			{
				Caster = _entity,
				Data = _stats.PhaseSwitchAttack
			});

			// Reactivating the passive
			_passiveTimer.Start(_phase.SpawnRate, true, OnPassiveTick);

			// We can hurt the boss again
			_entity.CurrentHealth = _entity.MaxHealth;
			_entity.SetInvulnerable(false);

			// The boss can return to an active state
			ResetStates();

			// Graphics update
			_weaponMesh.mesh = _phase.Weapon;
		}

		private void ResetStates()
		{
			State = Shared.EntityState.IDLE;
			LockAim = false;
			LockMovement = false;
			UnlockTarget();
			_gfxAnim.SetBool("IsTired", false);
		}

		#endregion

		#region Movement

		protected override bool UsesPathfinding => true;
		protected override float TriggerRange => Mathf.Infinity;
		protected override float UnTriggerRange => Mathf.Infinity;

		#endregion

		#region Passive

		private Timer _passiveTimer = new();

		private void OnPassiveTick()
		{
			// TODO: delete the following line
			return;

			Vector3 pos = GameManager.Player.transform.position.WithY(_room.GroundLevel);

			for (int i = 0; i <= (int)_phaseIndex; i++)
				AttackBase.ShowAttackPrevisu(_stats.PassiveAttack, 
					pos + Random.insideUnitSphere.WithY(pos.y) * _phase.PassiveSpread,
					_phase.PrevisualisationDuration,
					this,
					SpawnPassive);

			if (IsLastPhase)
			{
				for (int i = 0; i < 3; i++)
				{
					AttackBase.ShowAttackPrevisu(_stats.PassiveAttack,
						_room.Info.Data.SpawnablePositions.Random(),
						_phase.PrevisualisationDuration,
						this,
						SpawnPassive);
				}
			}

		}

		private void SpawnPassive(PrevisuParameters obj)
		{
			if (RunManager.RunState == RunState.IN_RUN)
			{
				AttackBase.Spawn(_stats.PassiveAttack, obj.Position, Quaternion.identity, new()
				{
					Caster = _entity,
					Data = _stats.PassiveAttack
				});
			}
		}

		#endregion

		#region Attack

		#region General

		private enum AttackType
		{
			LIGHT,
			LIGHT2,
			HEAVY
		}

		private Stack<AttackType> _attackStack = new();
		private AttackType _currentAttackType;
		private LunarisStatData.PhaseAttack _currentAttack;
		protected override float AttackCooldown => _phase.AttackCooldown;
		protected override float AttackRange => _currentAttack?.AttackData.Range ?? _phase.LightAttack.AttackData.Range;

		private void RefillAttackStack()
		{
			int lightNumber = Random.Range(_phase.LightBeforeHeavyNumber.x, _phase.LightBeforeHeavyNumber.y + 1);

			_attackStack.Push(AttackType.HEAVY); // Always end stack with heavy attack
			for (int i = 0; i < lightNumber; i++)
				_attackStack.Push(i % 2 == 0 ? AttackType.LIGHT : AttackType.LIGHT2);
		}

		protected override void TryAttacking()
		{
			float distance = Vector3.Distance(transform.position, GameManager.Player.transform.position.WithY(transform.position.y));

			if (Time.time - _lastAttackTime >= AttackCooldown && State == Shared.EntityState.IDLE)
			{
				if (distance < AttackRange * 0.8f) // Making sure we attack with soem freedom to hit
				{
					State = Shared.EntityState.ATTACKING;
					Attack();
				}
				else
					TryDash();
			}
		}

		protected override void Attack()
		{
			if (_attackStack.Count == 0)
				RefillAttackStack();

			_currentAttackType = _attackStack.Pop();
			_currentAttack = _currentAttackType == AttackType.HEAVY ? _phase.HeavyAttack : (_currentAttackType == AttackType.LIGHT ? _phase.LightAttack : _phase.LightAttack2);
			_gfxAnim.SetFloat("AttackSpeed", _entity.Scale(_currentAttack.AttackSpeed, Shared.StatModifier.AttackSpeed));
			_gfxAnim.Play(_currentAttack.Animation.name);
		}

		public void OnAnimationEvent(string animationArg)
		{
			if (animationArg == "Attack")
			{
				ModularAttack instance = AttackBase.Spawn(_currentAttack.AttackData, transform.position, Quaternion.LookRotation(GetAimNormal()), new()
				{
					Caster = _entity,
					Data = _currentAttack.AttackData
				}).GetComponent<ModularAttack>();
				instance.OnStart(_currentAttack.StartOffset, _currentAttack.TravelDistance);

				if (_phaseIndex == LunarisPhase.KATANA && _currentAttackType == AttackType.HEAVY)
					ThrustAttack();
				LockAim = false;
			}
		}

		public void OnAnimationEnter(AnimatorStateInfo stateInfo)
		{
			float previsuTime;

			// Global behaviour
			if (stateInfo.IsName(_currentAttack.Animation.name))
			{
				LockTarget(GameManager.Player.transform, true);
				LockMovement = true;
				LockAim = true;

				// Show previsualisation
				previsuTime = _currentAttack.Animation.events.FirstOrDefault(x => x.stringParameter == "Attack").time;
				AttackBase.ShowAttackPrevisu(_currentAttack.AttackData, transform.position, previsuTime, this);
			}
			// Light attack specifics
			if (stateInfo.IsName(_phase.LightAttack.Animation.name) || stateInfo.IsName(_phase.LightAttack2.Animation.name))
			{
			}
			// Heavy attack specifics
			else if (stateInfo.IsName(_phase.HeavyAttack.Animation.name))
			{
			}
		}

		public void OnAnimationExit(AnimatorStateInfo stateInfo)
		{
			// Global behaviour
			if (stateInfo.IsName(_currentAttack.Animation.name))
			{
				if (State != Shared.EntityState.STUN)
					ResetStates();
				OnAttackEnd();
			}
			// Light attack specifics
			if (stateInfo.IsName(_phase.LightAttack.Animation.name) || stateInfo.IsName(_phase.LightAttack2.Animation.name))
			{
			}
			// Heavy attack specifics
			else if (stateInfo.IsName(_phase.HeavyAttack.Animation.name))
			{
				if (State != Shared.EntityState.STUN)
					PutToRest();
			}
		}

		private void PutToRest()
		{
			LockAim = true;
			_gfxAnim.SetBool("IsTired", true);
			_gfxAnim.Play(_stats.RestAnimation.name);
			State = Shared.EntityState.STUN;
			Awaiter.WaitAndExecute(_phase.RestingTime, ResetStates);
		}

		#endregion

		private void TryDash()
		{
			// Scythe phase specific
			if (_phaseIndex != LunarisPhase.SCYTHE)
				return;

			var dir = (GameManager.Player.transform.position.WithY(transform.position.y) - transform.position).normalized;
			var dashPoint = transform.position + dir * _stats.DashRange;

			// Dash to be at range
			if (Vector3.Distance(GameManager.Player.transform.position.WithY(transform.position.y), dashPoint) < AttackRange / 2 && _canDash)
			{
				Dash(dir, _stats.DashRange, 0.2f, false);
				_dashTimer.Restart();
			}
		}

		private void ThrustAttack()
		{
			// TODO: make dash to closest wall
			// TODO: spawn explosion at impact
		}

		#endregion

		protected override void OnDeath(Damageable damageable)
		{
			if (damageable != _damageable)
				return;

			State = Shared.EntityState.STUN;
			_passiveTimer.Stop();
			_gfxAnim.Play(_stats.DeathAnimation.name);
			_room.OnEnemyKilled(gameObject);
		}
	}
}