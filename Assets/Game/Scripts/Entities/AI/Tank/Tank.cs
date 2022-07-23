using Game.Entities.Shared;
using Game.Managers;
using Game.Systems.Combat.Attacks;
using Game.VFX;
using Nawlian.Lib.Systems.Animations;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Entities.AI.Tank
{
	public class Tank : EnemyAI, IAnimationEventListener
	{
		[SerializeField, Required] private EnemyAttack _basicAttack;

		private bool _isDefending;
		private TankStatData _stats;
		private AnimatorStateInfo _currentState;
		private float _restTime = 1f;
		private AEnemySpawnFX _spawnFx;

		protected override void Awake()
		{
			base.Awake();
			_spawnFx = GetComponentInChildren<AEnemySpawnFX>();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			_entity.OnArmorBroken += OnArmorBroken;
			_damageable.OnBeforeDamaged += OnBeforeDamaged;
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			_entity.OnArmorBroken -= OnArmorBroken;
			_damageable.OnBeforeDamaged -= OnBeforeDamaged;
		}

		protected override void Init(object data)
		{
			base.Init(data);
			_stats = _entity.Stats as TankStatData;
			_isDefending = false;
		}

		protected override void OnInitState() => _spawnFx.PlaySpawnFX(() => base.OnInitState());

		#region Movement

		protected override bool UsesPathfinding => true;

		protected override Vector3 GetMovementsInputs()
		{
			if (Vector3.Distance(transform.position, GameManager.Player.transform.position) < (_stats.AttackRange / 2f))
				return Vector3.zero;
			return base.GetMovementsInputs();
		}

		protected override Vector3 GetTargetPosition()
		{
			if (GetMovementNormal().magnitude == 0)
				return GameManager.Player.transform.position;
			return base.GetTargetPosition();
		}

		protected override Vector3 GetPathfindingDestination()
		{
			if (_aiState == EnemyState.PASSIVE)
				return UpdatePassivePoint();
			return GameManager.Player.transform.position;
		}

		#endregion

		#region Defense

		private void OnBeforeDamaged()
		{
			// Cannot defend against an attack if not idle
			if (State != EntityState.IDLE)
				return;
			else if (_entity.CurrentArmor == 0)
			{
				_entity.Stats.Modifiers[StatModifier.KnockbackResistance].TemporaryModifier = _stats.KnockbackResistanceGain;
				_entity.Stats.Modifiers[StatModifier.ArmorRatio].TemporaryModifier = _stats.ArmorGain;
				_entity.RefillArmor(); // Give max armor
			}
			_gfxAnim.Play("Block", 0, 0);
		}

		private void OnArmorBroken()
		{
			_entity.Stats.Modifiers[StatModifier.KnockbackResistance].TemporaryModifier -= _stats.KnockbackResistanceGain;
			_entity.Stats.Modifiers[StatModifier.ArmorRatio].TemporaryModifier -= _stats.ArmorGain;

			// Cannot defend against an attack if not idle
			if (State == EntityState.IDLE)
				Attack();
		}

		#endregion

		#region Attack

		protected override void TryAttacking()
		{
			if (_isDefending == false && Time.time > _lastAttackTime + _restTime)
				base.TryAttacking();
		}

		protected override void Attack() => _gfxAnim.Play("Slam");

		public void OnAnimationEnter(AnimatorStateInfo stateInfo)
		{
			_currentState = stateInfo;
			if (stateInfo.IsName("Slam"))
			{
				State = EntityState.ATTACKING;
				LockTarget(GameManager.Player.transform, forceRotation: true);
				LockMovement = true;
				LockAim = true;
			}
			else if (stateInfo.IsName("Block"))
			{
				_isDefending = true;
				LockMovement = true;
				LockTarget(GameManager.Player.transform, forceRotation: true);
			}
		}

		public void OnAnimationEvent(string animationArg)
		{
			if (animationArg != "Attack")
				return;
			if (_currentState.IsName("Slam"))
			{
				InputManager.VibrateController(_stats.VibrationForce, _stats.VibrationDuration);
				GameManager.Camera.Shake(Vector3.one * _stats.VibrationForce, _stats.VibrationDuration);
				_basicAttack.gameObject.SetActive(true);
			}
		}

		public void OnAnimationExit(AnimatorStateInfo stateInfo)
		{
			if (stateInfo.IsName("Slam"))
			{
				LockMovement = false;
				LockAim = false;
				UnlockTarget();
				OnAttackEnd();
			}
			else if (stateInfo.IsName("Block"))
			{
				_isDefending = false;
				LockMovement = false;
				UnlockTarget();
			}
		}

		#endregion
	}
}
