using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using Nawlian.Lib.Utils;
using Nawlian.Lib.Extensions;
using Game.Managers;
using Game.Entities.Shared;
using Plugins.Nawlian.Lib.Systems.Menuing;
using System;

namespace Game.Entities.Player
{
	public class PlayerController : AController
	{
		#region Properties

		[SerializeField] private AudioSource _dashAudio;
		[SerializeField] private float _dashTime = 0.1f;
		[SerializeField] private ParticleSystem _dashFx;

		private Timer _dashTimer = new();
		private InputManager _inputs;
		private Vector3 _lastAimDir;

		private Plane _mousePlane;

		public bool CanDash => !LockMovement && State != EntityState.STUN && _dashTimer.IsOver();

		#endregion

		#region Unity builtins

		private void OnEnable()
		{
			InputManager.OnDashPressed += OnDashInput;
			OnDashStarted += PlayDashAudio;
		}

		private void OnDisable()
		{
			InputManager.OnDashPressed -= OnDashInput;
			OnDashStarted -= PlayDashAudio;
		}

		protected override void Awake()
		{
			base.Awake();
			_inputs = InputManager.Instance;
		}

		private void Start()
		{
			_mousePlane = new Plane(Vector3.up, transform.position);
			_dashTimer.Start(_entity.CurrentDashCooldown, false);
		}

		protected override void Update()
		{
			base.Update();
			if (State == EntityState.IDLE)
			{
				LockMovement = GuiManager.IsMenuing;
				LockAim = GuiManager.IsMenuing;
			}
		}

		#endregion

		private void PlayDashAudio(DashParameters obj)
		{
			_dashAudio.Play();
		}

		private void OnDashInput()
		{
			if (CanDash)
			{
				Vector3 direction = GetMovementNormal().magnitude > 0 ? GetMovementNormal() : GetAimNormal();

				_dashFx.Play(true);
				Dash(direction, _entity.CurrentDashRange, _dashTime, false, true);
				_dashTimer.Interval = _entity.CurrentDashCooldown;
				_dashTimer.Restart();
			}
		}

		#region Abstraction

		protected override Vector3 GetMovementsInputs() => _inputs.MovementAxis.ToVector3XZ().ToIsometric();

		protected override Vector3 GetTargetPosition()
		{
			Vector3 aimInput = _inputs.InUseControl == ControlType.KEYBOARD ? GetTargetForMouse() : GetMovementsInputs();
			bool isAiming = aimInput.magnitude > 0;

			if (isAiming)
				_lastAimDir = aimInput;
			if (LockAim)
				return _rb.position + _graphics.transform.forward;
			else if (isAiming)
				return _rb.position + aimInput;
			return _rb.position + _lastAimDir;
		}

		private Vector3 GetTargetForMouse()
		{
			if (State != EntityState.ATTACKING && State != EntityState.DASH)
				return GetMovementsInputs();

			Ray ray = GameManager.Camera.Camera.ScreenPointToRay(_inputs.MousePosition);

			if (_mousePlane.Raycast(ray, out float distance))
				return (ray.GetPoint(distance) - transform.position).normalized;
			return _lastAimDir;
		}

		#endregion
	}
}