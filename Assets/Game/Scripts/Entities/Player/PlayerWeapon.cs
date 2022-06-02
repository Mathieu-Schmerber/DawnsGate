using Game.Entities.Shared;
using Game.Managers;
using Game.Systems.Combat.Attacks;
using Game.Systems.Combat.Weapons;
using Nawlian.Lib.Systems.Animations;
using Nawlian.Lib.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Entities.Player
{
	public class PlayerWeapon : MonoBehaviour, IAnimationEventListener
	{
		[SerializeField] private Transform _weaponJoint;

		private EntityIdentity _identity;
		private Animator _animator;
		private InputManager _inputs;
		private List<GameObject> _weaponAttackPool;
		private Timer _attackTimer = new();
		private float _minTimeBetweenAttacks = 0.1f;

		private WeaponState _weaponState;
		private Weapon _weapon => _weaponState?.EquippedState;

		#region Unity builtins

		private void Awake()
		{
			_identity = GetComponent<EntityIdentity>();
			_weaponState = GetComponentInChildren<WeaponState>();
			_animator = GetComponentInChildren<Animator>();
			_inputs = InputManager.Instance;
			_weaponAttackPool = new List<GameObject>();
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
		/// Equips a weapon.
		/// </summary>
		public void EquipWeapon(WeaponState weaponState)
		{
			// Drop current
			_weaponState?.ToItemState();

			// Position weapon on joint
			_weaponState = weaponState;
			_weaponState.transform.SetParent(_weaponJoint);
			_weaponState.transform.localPosition = Vector3.zero;
			_weaponState.transform.localRotation = Quaternion.Euler(-90, 0, 0);
			_weaponState.ToEquippedState();

			// Setup animator accordingly
			SetupWeaponConstraints();
		}

		public void SetupWeaponConstraints()
		{
			_animator.SetLayerWeight(_animator.GetLayerIndex("DefaultLocomotion"), 0f);
			_animator.SetLayerWeight(_animator.GetLayerIndex("GreatSwordLocomotion"), 0f);

			if (_weapon != null)
				_animator.SetLayerWeight(_animator.GetLayerIndex(_weapon.Data.LocomotionLayer), 1f);
			else
				_animator.SetLayerWeight(_animator.GetLayerIndex("DefaultLocomotion"), 1f);
		}

		#endregion

		public AttackBase SpawnFromPool(AttackBaseData attack, Vector3 position, Quaternion rotation)
		{
			GameObject pulled = _weaponAttackPool.FirstOrDefault(x => x.activeSelf == false && x.name == attack.name);

			if (pulled == null)
			{
				pulled = Instantiate(attack.Prefab.gameObject, position, rotation);
				pulled.name = attack.name;
				_weaponAttackPool.Add(pulled);
			}
			pulled.transform.position = position;
			pulled.transform.rotation = rotation;
			pulled.SetActive(true);

			AttackBase result = pulled.GetComponent<AttackBase>();
			result.Init(attack, _identity);

			return result;
		}

		#region Attack

		/// <summary>
		/// Determines if the entity is willing to try attacking
		/// </summary>
		/// <returns></returns>
		private bool WantsToAttack() => _inputs.IsAttackDown;

		protected virtual void TryAttack()
		{
			if (_attackTimer.IsOver() && _weapon != null)
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

			Debug.Log($"{_weapon.Data.AttackSpeed} => {_identity.Stats.Modifiers[StatModifier.AttackSpeed].Value}% = {attackSpeed}");

			if (attack == null)
				return;
			_weapon.OnAttackStart();
			_animator.SetFloat("AttackSpeed", attackSpeed);
			_attackTimer.Interval = attack.AttackAnimation.length / attackSpeed + _minTimeBetweenAttacks;
			_animator.Play(attack.AttackAnimation.name);
		}

		public void OnAnimationEvent(string animationArg) => _weapon.OnAnimationEvent(animationArg);

		public void OnAnimationEnter(AnimatorStateInfo stateInfo) => _weapon.OnAnimationEnter(stateInfo);

		public void OnAnimationExit(AnimatorStateInfo stateInfo) => _weapon.OnAnimationExit(stateInfo);

		#endregion
	}
}