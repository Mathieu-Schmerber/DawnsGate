using Game.Entities.Shared;
using Game.Entities.Player;
using System.Linq;
using UnityEngine;
using Game.Entities.Camera;
using Game.Managers;
using Game.Systems.Combat.Attacks;
using Game.Tools;
using Pixelplacement;

namespace Game.Systems.Combat.Weapons
{
	public class Weapon : MonoBehaviour
	{
		#region Properties

		public WeaponData Data { get; private set; }

		private CameraController _camera;
		protected AController _controller;
		protected PlayerWeapon _weaponManager;
		private MeshFilter _meshFilter;
		private MeshRenderer _meshRenderer;
		private RandomAudioClip _audio;
		protected int _lastAttackIndex = -1;
		private const float DASH_DURATION = 0.1f;

		/// <summary>
		/// Returns the current attack
		/// </summary>
		public WeaponData.WeaponAttack CurrentWeaponAttack
		{
			get => Data.AttackCombos[_lastAttackIndex];
		}

		/// <summary>
		/// Returns the time to input between attack animations to perform a combo
		/// </summary>
		public float ComboIntervalTime { get => Data.ComboIntervalTime; }

		#endregion

		#region Unity builtins

		private void Awake()
		{
			_camera = GameManager.Camera;
			_meshFilter = GetComponent<MeshFilter>();
			_meshRenderer = GetComponent<MeshRenderer>();
			_controller = GetComponentInParent<AController>();
			_weaponManager = GetComponentInParent<PlayerWeapon>();
			_audio = GetComponent<RandomAudioClip>();
		}

		#endregion

		public void SetData(WeaponData data)
		{
			Data = data;
			if (data != null)
			{
				transform.localPosition = data.InHandPosition;
				transform.localEulerAngles = data.InHandRotation;
			}
			_meshFilter.mesh = data?.Mesh;
			_meshRenderer.material = data?.Material;
			_lastAttackIndex = -1;
		}

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
			if (_lastAttackIndex + 1 < Data.AttackCombos.Count && continueCombo)
			{
				_lastAttackIndex++;
				return CurrentWeaponAttack;
			}
			_lastAttackIndex = 0;
			return Data.AttackCombos.First();
		}

		private void PerformAttack()
		{
			// Getting the attack from pool
			AttackBase attack = _weaponManager.SpawnFromPool(CurrentWeaponAttack.Attack.AttackData, _controller.transform.position, Quaternion.identity);

			// Notify weapon holder that the attack did hit
			attack.OnAttackHitEvent = (data, victim, damage) =>
			{
				_audio.PlayRandom(data.OnHitAudios);
				_weaponManager.OnHit(data, victim, Data.IsHeavy(CurrentWeaponAttack), damage);
			};

			// Aim assist
			if (CurrentWeaponAttack.Attack.AimAssist)
				ActivateAimAssist(attack.Range, 50);

			// Managing attack instance
			Vector3 aimDir = _controller.GetAimNormal();

			attack.transform.rotation = Quaternion.LookRotation(aimDir);
			attack.OnStart(CurrentWeaponAttack.Attack.StartOffset, CurrentWeaponAttack.Attack.TravelDistance);

			// Locks
			_controller.LockAim = CurrentWeaponAttack.Attack.LockAim;
			_controller.LockMovement = CurrentWeaponAttack.Attack.LockMovement;

			// FX
			_camera.Shake(CurrentWeaponAttack.FX.CameraShakeForce * Vector3.one, CurrentWeaponAttack.FX.CameraShakeDuration);
			InputManager.VibrateController(CurrentWeaponAttack.FX.VibrationForce, CurrentWeaponAttack.FX.VibrationDuration);

			// Aim assist
			if (CurrentWeaponAttack.Attack.AimAssist)
				DeactivateAimAssist();
		}

		private void MoveInHand(Vector3 localPos, Vector3 localRot, bool smooth = false)
		{
			if (!smooth)
			{
				transform.localPosition = localPos;
				transform.localEulerAngles = localRot;
			}
			else
			{
				Tween.Value(transform.localPosition, localPos, (v) => transform.localPosition = v, 0.5f, 0);
				Tween.Value(transform.localEulerAngles, localRot, (v) => transform.localEulerAngles = v, 0.5f, 0);
			}
		}

		public virtual void OnAnimationEvent(string animation)
		{
			if (animation == nameof(WeaponAttackEvent.Attack))
				PerformAttack();
			else if (animation == nameof(WeaponAttackEvent.Dash) && (_controller.GetMovementNormal().magnitude > 0 || !CurrentWeaponAttack.Dash.OnlyWhenMoving))
				_controller.Dash(_controller.GetAimNormal(), CurrentWeaponAttack.Dash.Distance, DASH_DURATION, false, false);
		}

		public virtual void OnAnimationEnter(AnimatorStateInfo stateInfo)
		{
			_controller.State = EntityState.ATTACKING;
			if (CurrentWeaponAttack.Attack.UseCustomHandPosition)
				MoveInHand(CurrentWeaponAttack.Attack.InHandPosition, CurrentWeaponAttack.Attack.InHandRotation, CurrentWeaponAttack.Attack.SmoothHandPlacement);
		}

		public virtual void OnAnimationExit(AnimatorStateInfo stateInfo)
		{
			if (_controller.State == EntityState.ATTACKING)
				_controller.State = EntityState.IDLE;
			_controller.LockMovement = false;
			_controller.LockAim = false;
			MoveInHand(Data.InHandPosition, Data.InHandRotation, Data.SmoothHandPlacement);
		}

		#endregion
	}
}