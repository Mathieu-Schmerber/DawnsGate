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

		private void Start()
		{
			InputManager_OnControlChanged(InputManager.Instance.InUseControl);
		}

		private void InputManager_OnControlChanged(ControlType obj)
		{
			_inputImage.enabled = true;
			if (_switch.ContainsKey(obj) && _switch[obj] != null)
				_inputImage.sprite = _switch[obj];
			else
				_inputImage.enabled = false;
		}
	}
}
