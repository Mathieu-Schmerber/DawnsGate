using Game.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game
{
    public class ReplaceTextInputs : MonoBehaviour
    {
		[SerializeField] private TextMeshProUGUI _keyboardText;
		[SerializeField] private TextMeshProUGUI _controllerText;

		private void OnEnable()
		{
			InputManager.OnControlChanged += InputManager_OnControlChanged;
		}

		private void OnDisable()
		{
			InputManager.OnControlChanged -= InputManager_OnControlChanged;
		}

		private void Start()
		{
			InputManager_OnControlChanged(InputManager.Instance.InUseControl);
		}

		private void InputManager_OnControlChanged(ControlType obj)
		{
			_keyboardText.enabled = obj == ControlType.KEYBOARD;
			_controllerText.enabled = obj != ControlType.KEYBOARD;
		}
	}
}
