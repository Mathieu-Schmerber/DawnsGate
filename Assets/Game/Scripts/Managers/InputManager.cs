using Nawlian.Lib.Utils;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Managers
{
	/// <summary>
	/// Input handler. <br></br>
	/// Stores the inputs states
	/// </summary>
	public class InputManager : ManagerSingleton<InputManager>
	{
		private Controls _controls;
		public Vector2 MovementAxis => _controls.Player.Movement.ReadValue<Vector2>();
		public bool IsAttackDown { get; private set; }

		public static event Action OnDashPressed;
		public static event Action OnInteractPressed;
		public static event Action OnInventoryPressed;

		#region Unity builtins

		private void OnEnable() => _controls.Player.Enable();

		private void OnDisable() => _controls.Player.Disable();

		private void Awake()
		{
			_controls = new Controls();

			_controls.Player.Attack.performed += (ctx) => IsAttackDown = true;
			_controls.Player.Attack.canceled += (ctx) => IsAttackDown = false;
			_controls.Player.Dash.performed += (ctx) => OnDashPressed?.Invoke();
			_controls.Player.Interact.performed += (ctx) => OnInteractPressed?.Invoke();
			_controls.Player.Inventory.performed += (ctx) => OnInventoryPressed?.Invoke();
		}

		#endregion

		private static async void WaitAndExecute(float time, Action execute)
		{
			float start = Time.time;
			float end = start + time;

			while (Time.time < end)
				await Task.Yield();
			execute.Invoke();
		}

		public static void VibrateController(float normalizedAmount, float time)
		{
			if (time == 0 || normalizedAmount == 0 || Gamepad.current == null)
				return;
			Gamepad.current.SetMotorSpeeds(normalizedAmount, normalizedAmount);
			WaitAndExecute(time, () => Gamepad.current.SetMotorSpeeds(0, 0));
		}
	}
}