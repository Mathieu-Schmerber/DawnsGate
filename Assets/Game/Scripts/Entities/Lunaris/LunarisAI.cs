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
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Entities.Lunaris
{
	public class LunarisAI : EnemyAI, IAnimationEventListener
	{
		[SerializeField] private MeshFilter _weaponMesh;
		private LunarisStatData _stats;

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

			// Cannot damage the boss any further
			_entity.SetInvulnerable(true);

			// Restricting the boss from doing anything
			ResetStates();
			_gfxAnim.StopPlayback();
			State = Shared.EntityState.STUN;

			// Executing the switched state code after waiting for a while
			Previsualisation.Show(_stats.PassiveAttack.Previsualisation, transform.position.WithY(_room.GroundLevel), _stats.PhaseSwitchAttack.Range, _stats.PhaseSwitchTime);
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
			Vector3 pos = GameManager.Player.transform.position.WithY(_room.GroundLevel);

			for (int i = 0; i <= (int)_phaseIndex; i++)
				Previsualisation.Show(
					_stats.PassiveAttack.Previsualisation,
					pos + Random.insideUnitSphere.WithY(pos.y) * _phase.PassiveSpread,
					_stats.PassiveAttack.Range,
					_phase.PrevisualisationDuration,
					SpawnPassive);

			if (IsLastPhase)
			{
				for (int i = 0; i < 3; i++)
				{
					Previsualisation.Show(
					_stats.PassiveAttack.Previsualisation,
					_room.Info.Data.SpawnablePositions.Random(),
					_stats.PassiveAttack.Range,
					_phase.PrevisualisationDuration,
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
				_attackStack.Push(AttackType.LIGHT);
		}

		protected override void Attack()
		{
			if (_attackStack.Count == 0)
				RefillAttackStack();

			_currentAttackType = _attackStack.Pop();
			_currentAttack = _currentAttackType == AttackType.HEAVY ? _phase.HeavyAttack : _phase.LightAttack;
			_gfxAnim.SetFloat("AttackSpeed", _entity.Scale(_currentAttack.AttackSpeed, Shared.StatModifier.AttackSpeed));
			_gfxAnim.Play(_currentAttack.Animation.name);
		}

		public void OnAnimationEvent(string animationArg)
		{
			if (animationArg == "Attack")
			{
				LockTarget(GameManager.Player.transform, true);
				LockAim = true;

				ModularAttack instance = AttackBase.Spawn(_currentAttack.AttackData, transform.position, Quaternion.LookRotation(GetAimNormal()), new()
				{
					Caster = _entity,
					Data = _currentAttack.AttackData
				}).GetComponent<ModularAttack>();

				instance.OnStart(_currentAttack.StartOffset, _currentAttack.TravelDistance);

				if (_phaseIndex == LunarisPhase.KATANA && _currentAttackType == AttackType.HEAVY)
					ThrustAttack();
			}
		}

		public void OnAnimationEnter(AnimatorStateInfo stateInfo)
		{
			// Global behaviour
			if (stateInfo.IsName(_currentAttack.Animation.name))
			{
				LockTarget(GameManager.Player.transform);
				LockMovement = true;
			}
			// Light attack specifics
			if (stateInfo.IsName(_phase.LightAttack.Animation.name))
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
			if (stateInfo.IsName(_phase.LightAttack.Animation.name))
			{
			}
			// Heavy attack specifics
			else if (stateInfo.IsName(_phase.HeavyAttack.Animation.name))
			{
				if (State != Shared.EntityState.STUN)
					PutToRest();
			}
		}

		#endregion

		private void PutToRest()
		{
			LockAim = true;
			_gfxAnim.SetBool("IsTired", true);
			_gfxAnim.Play(_stats.RestAnimation.name);
			State = Shared.EntityState.STUN;
			Awaiter.WaitAndExecute(_phase.RestingTime, ResetStates);
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