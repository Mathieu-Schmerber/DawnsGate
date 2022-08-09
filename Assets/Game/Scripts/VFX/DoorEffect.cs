using Game.Systems.Run.GPE;
using TMPro;
using UnityEngine;

namespace Game.VFX
{
	public class DoorEffect : MonoBehaviour
	{
		private const string ACTIVATE_ANIMATOR = "Activated";

		private RoomDoor _door;
		private TextMeshPro _text;
		private Animator _animator;

		#region Unity builtins

		private void Awake()
		{
			_door = GetComponentInParent<RoomDoor>();
			_text = GetComponentInChildren<TextMeshPro>();
			_animator = GetComponent<Animator>();
		}

		private void OnEnable()
		{
			_door.OnActivated += OnActivated;
		}

		private void OnDisable()
		{
			_door.OnActivated -= OnActivated;
		}

		#endregion

		private void OnActivated()
		{
			_animator?.SetBool(ACTIVATE_ANIMATOR, true);
			if (_door.LeadToRoom != null)
				_text.text = $"{_door.LeadToRoom.Type}{System.Environment.NewLine}{_door.LeadToRoom.Reward}";
			else
				_text.text = "End run";
		}
	}
}
