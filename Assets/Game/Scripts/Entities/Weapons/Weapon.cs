using Game.Entities.Shared;
using Game.Entities.Player;
using Game.Entities.Shared.Attacks;
using Game.Scriptables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.Entities.Camera;
using Game.Managers;
using Game.Inputs;

namespace Game.Entities.Weapons
{
	public class Weapon : MonoBehaviour
	{
		#region Properties

		[SerializeField] private WeaponData _data;
		public WeaponData Data => _data;

		private WeaponState _state;
		private CameraController _camera;
		protected AController _controller;
		protected PlayerWeapon _weaponManager;
		protected int _lastAttackIndex = -1;

		/// <summary>
		/// Returns the current attack
		/// </summary>
		public WeaponData.WeaponAttack CurrentWeaponAttack
		{
			get => _data.AttackCombos[_lastAttackIndex];
		}

		/// <summary>
		/// Returns the time to input between attack animations to perform a combo
		/// </summary>
		public float ComboIntervalTime { get => _data.ComboIntervalTime; }

		#endregion

		#region Unity builtins

		private void Awake()
		{
			_camera = GameManager.Instance.Camera;
			_state = GetComponent<WeaponState>();
		}

		private void OnEnable()
		{
			_state.OnEquipStateEnabled += OnEquipState;
		}

		private void OnDisable()
		{
			_state.OnEquipStateEnabled -= OnEquipState;
		}

		public void OnEquipState()
		{
			_weaponManager = GetComponentInParent<PlayerWeapon>();
			_controller = GetComponentInParent<AController>();
			_lastAttackIndex = 0;
		}

		#endregion

		#region Aim assist

		private void FaceClosestTarget(float range, float maxAngle)
		{
			Transform[] inRange = Physics.OverlapSphere(_controller.transform.position, range)
				.Where(x => x.gameObject != _controller.gameObject && x.GetComponent<IDamageProcessor>() != null)
				.Select(x => x.transform)
				.OrderBy(x => Vector3.Distance(_controller.transform.position, x.position)).ToArray();

			if (inRange.Length == 0) return;
			foreach (Transform entity in inRange)
			{
				Vector3 dir = (entity.position - _controller.transform.position).normalized;
				float angle = Vector3.Angle(_controller.GetAimNormal(), dir);

				if (angle <= maxAngle)
				{
					_controller.LockTarget(entity, true);
					return;
				}
			}
		}

		protected void ActivateAimAssist(float range, float angleAssist) => FaceClosestTarget(range, angleAssist);
		protected void DeactivateAimAssist() => _controller.UnlockTarget();

		#endregion

		#region Attacking

		public virtual void OnAttackStart() { }

		/// <summary>
		/// Returns the next attack
		/// </summary>
		/// <param name="continueCombo">Should it get the next combo part ?</param>
		/// <returns></returns>
		public virtual WeaponData.WeaponAttack GetNextAttack(bool continueCombo = true)
		{
			if (_lastAttackIndex + 1 < _data.AttackCombos.Count && continueCombo)
			{
				_lastAttackIndex++;
				return CurrentWeaponAttack;
			}
			_lastAttackIndex = 0;
			return _data.AttackCombos.First();
		}

		private bool IsDashedAttack() =>
			CurrentWeaponAttack.Dash.Distance > 0 &&
			(_controller.GetMovementNormal().magnitude > 0 || !CurrentWeaponAttack.Dash.OnlyWhenMoving) &&
			!CurrentWeaponAttack.Dash.OnAnimationEventOnly;

		private void PerformAttack()
		{
			// Getting the attack from pool
			AttackBase attack = _weaponManager.SpawnFromPool(CurrentWeaponAttack.Attack.AttackData, _controller.transform.position, Quaternion.identity);

			// Aim assist
			if (CurrentWeaponAttack.Attack.AimAssist)
				ActivateAimAssist(attack.Range, 50);

			// Managing attack instance
			Vector3 aimDir = _controller.GetAimNormal();
			Vector3 dashOffset = IsDashedAttack() && !attack.FollowCaster ? attack.transform.forward * CurrentWeaponAttack.Dash.Distance : Vector3.zero;
			
			attack.transform.rotation = Quaternion.LookRotation(aimDir);
			attack.OnStart(CurrentWeaponAttack.Attack.StartOffset + dashOffset, CurrentWeaponAttack.Attack.TravelDistance);

			// Locks
			_controller.IsAimLocked = CurrentWeaponAttack.Attack.LockAim;
			_controller.CanMove = !CurrentWeaponAttack.Attack.LockMovement;

			// FX
			_camera.Shake(CurrentWeaponAttack.FX.CameraShakeForce, CurrentWeaponAttack.FX.CameraShakeDuration);
			InputHandler.VibrateController(CurrentWeaponAttack.FX.VibrationForce, CurrentWeaponAttack.FX.VibrationDuration);

			// Dashed attack
			if (IsDashedAttack())
				_controller.Dash(aimDir, CurrentWeaponAttack.Dash.Distance, CurrentWeaponAttack.Dash.Duration, CurrentWeaponAttack.Dash.AfterImages);

			// Aim assist
			if (CurrentWeaponAttack.Attack.AimAssist)
				DeactivateAimAssist();
		}

		public virtual void OnAnimationEvent(string animation)
		{
			if (animation == "Attack")
				PerformAttack();
			else if (animation == "Dash" && CurrentWeaponAttack.Dash.OnAnimationEventOnly && (_controller.GetMovementNormal().magnitude > 0 || !CurrentWeaponAttack.Dash.OnlyWhenMoving))
				_controller.Dash(_controller.GetAimNormal(), CurrentWeaponAttack.Dash.Distance, CurrentWeaponAttack.Dash.Duration, CurrentWeaponAttack.Dash.AfterImages);
		}

		public virtual void OnAnimationEnter(AnimatorStateInfo stateInfo) => _controller.CanMove = false;

		public virtual void OnAnimationExit(AnimatorStateInfo stateInfo)
		{
			_controller.CanMove = true;
			_controller.IsAimLocked = false;
		}

		#endregion
	}
}