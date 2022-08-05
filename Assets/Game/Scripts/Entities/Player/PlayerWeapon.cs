using Game.Entities.Shared;
using Game.Entities.Shared.Health;
using Game.Managers;
using Game.Systems.Combat.Attacks;
using Game.Systems.Combat.Weapons;
using Game.VFX;
using Nawlian.Lib.Systems.Animations;
using Nawlian.Lib.Systems.Pooling;
using Nawlian.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Entities.Player
{
	public class PlayerWeapon : MonoBehaviour, IAnimationEventListener
	{
		public class AttackHitEventArgs : EventArgs
		{
			public AttackBaseData Data { get; set; }
			public Damageable Victim { get; set; }
			public float DamageApplied { get; set; }
			public bool IsHeavyAttack { get; set; }
		}

		private EntityIdentity _identity;
		private Animator _animator;
		private InputManager _inputs;
		private List<GameObject> _weaponAttackPool;
		private Timer _attackTimer = new();
		private float _minTimeBetweenAttacks = 0.1f;
		private Weapon _weapon;
		private AController _controller;

		public WeaponData CurrentWeapon => _weapon.Data;

		public event Action<AttackHitEventArgs> OnAttackHit;
		public event Action OnAttackLaunched;

		#region Unity builtins

		private void Awake()
		{
			_identity = GetComponent<EntityIdentity>();
			_inputs = InputManager.Instance;
			_animator = GetComponentInChildren<Animator>();
			_weapon = GetComponentInChildren<Weapon>();
			_weaponAttackPool = new List<GameObject>();
			_controller = GetComponent<AController>();
		}

		private void Start()
		{
			_attackTimer.Start(0, false);
			SetupWeaponConstraints();
		}

		protected virtual void Update()
		{
			if (WantsToAttack())
				TryAttack();
		}

		#endregion

		#region Equip / UnEquip

		/// <summary>
		/// Equips a weapon or unequips if null is passed as a parameter.
		/// </summary>
		public void EquipWeapon(WeaponData weaponData)
		{
			_weapon.SetData(weaponData);
			SetupWeaponConstraints();
		}

		public void SetupWeaponConstraints()
		{
			_animator.SetLayerWeight(_animator.GetLayerIndex("DefaultLocomotion"), 0f);
			if (_weapon.Data != null)
				_animator.SetLayerWeight(_animator.GetLayerIndex(_weapon.Data.LocomotionLayer), 1f);
			else
				_animator.SetLayerWeight(_animator.GetLayerIndex("DefaultLocomotion"), 1f);
		}

		#endregion

		public AttackBase SpawnFromPool(AttackBaseData attack, Vector3 position, Quaternion rotation)
		{
			GameObject instance = ObjectPooler.Get(attack.Prefab.gameObject, position, rotation, 
				new AttackBase.InitData() 
				{ 
					Data = attack, 
					Caster = _identity
				});

			return instance.GetComponent<AttackBase>();
		}

		#region Attack

		/// <summary>
		/// Determines if the entity is willing to try attacking
		/// </summary>
		/// <returns></returns>
		private bool WantsToAttack() => _inputs.IsAttackDown;

		protected virtual void TryAttack()
		{
			if (_controller.State != EntityState.STUN && !GuiManager.IsMenuing && _attackTimer.IsOver() && _weapon.Data != null)
			{
				OnMeleeAttack();
				_attackTimer.Restart();
			}
		}

		/// <summary>
		/// Check if a combo attack is possible and adapt attack cooldown to the clip's length
		/// </summary>
		protected virtual void OnMeleeAttack()
		{
			float delta = Time.time - _attackTimer.LastTickFrame;
			bool canCombo = delta - _attackTimer.Interval <= _weapon.ComboIntervalTime;
			var attack = _weapon.GetNextAttack(canCombo);
			float attackSpeed = _identity.Scale(_weapon.Data.AttackSpeed, StatModifier.AttackSpeed);

			if (attack == null)
				return;
			_weapon.OnAttackStart();
			_animator.SetFloat("AttackSpeed", attackSpeed);
			_attackTimer.Interval = attack.AttackAnimation.length / attackSpeed + _minTimeBetweenAttacks;
			_animator.Play(attack.AttackAnimation.name);
			OnAttackLaunched?.Invoke();
		}

		public void OnHit(AttackBaseData data, Damageable collider, bool isHeavy, float damage)
		{
			QuickText.ShowDamageText(collider.transform.position, damage);
			OnAttackHit?.Invoke(new() { Data = data, Victim = collider, IsHeavyAttack = isHeavy, DamageApplied = damage });
		}

		public void OnAnimationEvent(string animationArg)
		{
			if (animationArg == "Hide Weapon")
				_weapon.Hide();
			else if (animationArg == "Show Weapon")
				_weapon.Show();
			else
				_weapon.OnAnimationEvent(animationArg);
		}

		public void OnAnimationEnter(AnimatorStateInfo stateInfo) => _weapon.OnAnimationEnter(stateInfo);

		public void OnAnimationExit(AnimatorStateInfo stateInfo) => _weapon.OnAnimationExit(stateInfo);

		#endregion
	}
}