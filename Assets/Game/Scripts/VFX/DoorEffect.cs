using Game.Systems.Run.GPE;
using TMPro;
using UnityEngine;

namespace Game.VFX
{
	public class DoorEffect : MonoBehaviour
	{
		[SerializeField] private Material _activatedMaterial;
		private RoomDoor _door;
		private MeshRenderer _meshRenderer;
		private TextMeshPro _text;

		#region Unity builtins

		private void Awake()
		{
			_door = GetComponentInParent<RoomDoor>();
			_meshRenderer = GetComponent<MeshRenderer>();
			_text = GetComponentInChildren<TextMeshPro>();
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
			_meshRenderer.material = _activatedMaterial;
			_text.text = $"{_door.LeadToRoom.Type}{System.Environment.NewLine}{_door.LeadToRoom.Reward}";
		}
	}
}
