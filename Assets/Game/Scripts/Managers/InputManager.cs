using Nawlian.Lib.Utils;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Managers
{
	public enum ControlType
	{
		XBOX,
		PS4,
		KEYBOARD
	}

	/// <summary>
	/// Input handler. <br></br>
	/// Stores the inputs states
	/// </summary>
	public class InputManager : ManagerSingleton<InputManager>
	{
		private PlayerInput _input;
		private Controls _controls;
		public Vector2 MovementAxis => _controls.Player.Movement.ReadValue<Vector2>();
		public bool IsAttackDown { get; private set; }

		public static event Action OnDashPressed;
		public static event Action OnInteractPressed;
		public static event Action OnInventoryPressed;
		public static event Action OnCancelPressed;
		public static event Action OnSubmitPressed;
		public static event Action<ControlType> OnControlChanged;

		#region Unity builtins

		private void OnEnable()
		{
			_controls.Player.Enable();
			_controls.UI.Enable();
			_input.onControlsChanged += OnControlsChanged;
		}

		private void OnDisable()
		{
			_controls.Player.Disable();
			_controls.UI.Disable();
			_input.onControlsChanged -= OnControlsChanged;
		}

		private void Awake()
		{
			_input = GetComponent<PlayerInput>();

			_controls = new Controls();

			_controls.Player.Attack.performed += (ctx) => IsAttackDown = true;
			_controls.Player.Attack.canceled += (ctx) => IsAttackDown = false;
			_controls.Player.Dash.performed += (ctx) => OnDashPressed?.Invoke();
			_controls.Player.Interact.performed += (ctx) => OnInteractPressed?.Invoke();
			_controls.UI.Inventory.performed += (ctx) => OnInventoryPressed?.Invoke();
			_controls.UI.Cancel.performed += (ctx) => OnCancelPressed?.Invoke();
			_controls.UI.Submit.performed += (ctx) => OnSubmitPressed?.Invoke();
		}

		private void Start() => OnControlsChanged(_input);

		#endregion

		private void OnControlsChanged(PlayerInput input)
		{
			if (input.devices[0].device.displayName.Contains("Keyboard"))
				OnControlChanged?.Invoke(ControlType.KEYBOARD);
			else if (Gamepad.current is UnityEngine.InputSystem.XInput.XInputController) // XBOX
				OnControlChanged?.Invoke(ControlType.XBOX);
			else if (Gamepad.current is UnityEngine.InputSystem.DualShock.DualShockGamepad) // PS4
				OnControlChanged?.Invoke(ControlType.PS4);
		}

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