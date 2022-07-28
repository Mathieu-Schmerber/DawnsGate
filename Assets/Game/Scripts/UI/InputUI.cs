using Game.Managers;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class InputUI : SerializedMonoBehaviour
	{
		[SerializeField] private Dictionary<ControlType, Sprite> _switch = new();
		[SerializeField] private Image _inputImage;

		private void OnEnable()
		{
			InputManager.OnControlChanged += InputManager_OnControlChanged;
		}

		private void OnDisable()
		{
			InputManager.OnControlChanged -= InputManager_OnControlChanged;
		}

		private void InputManager_OnControlChanged(ControlType obj)
		{
			if (_switch.ContainsKey(obj))
				_inputImage.sprite = _switch[obj];
		}
	}
}
