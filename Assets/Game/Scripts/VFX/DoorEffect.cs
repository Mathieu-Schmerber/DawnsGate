using Game.Systems.Run.Rooms;
using UnityEngine;

namespace Game.VFX
{
	public class DoorEffect : MonoBehaviour
	{
		[SerializeField] private Material _activatedMaterial;
		private RoomDoor _door;
		private MeshRenderer _meshRenderer;

		#region Unity builtins

		private void Awake()
		{
			_door = GetComponentInParent<RoomDoor>();
			_meshRenderer = GetComponent<MeshRenderer>();
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
		}
	}
}
