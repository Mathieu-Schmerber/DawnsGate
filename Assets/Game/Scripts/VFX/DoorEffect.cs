using Game.Systems.Run;
using Game.Systems.Run.GPE;
using Game.Systems.Run.Rooms;
using Nawlian.Lib.Extensions;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.VFX
{
	public class DoorEffect : SerializedMonoBehaviour
	{
		private const string ACTIVATE_ANIMATOR = "Activated";

		private RoomDoor _door;
		private Animator _animator;

		[SerializeField] private ParticleSystem[] _totems;
		[SerializeField] private GameObject _defaultDoorGfx;
		[SerializeField] private Dictionary<RoomType, GameObject> _typedDoors = new();
		[SerializeField] private Dictionary<RoomRewardType, GameObject> _rewardDoors = new();

		#region Unity builtins

		private void Awake()
		{
			_door = GetComponentInParent<RoomDoor>();
			_animator = GetComponentInChildren<Animator>();

			_typedDoors.Values.ForEach(x => x.SetActive(false));
			_rewardDoors.Values.ForEach(x => x.SetActive(false));
			_defaultDoorGfx.SetActive(true);
			_totems.ForEach(x => x.gameObject.SetActive(false));
		}

		private void OnEnable()
		{
			ARoom.OnRoomLogicReady += DisplayRoomType;
			_door.OnActivated += OnActivated;
			_door.OnInteracted += OpenDoor;
		}

		private void OnDisable()
		{
			ARoom.OnRoomLogicReady -= DisplayRoomType;
			_door.OnActivated -= OnActivated;
			_door.OnInteracted -= OpenDoor;
		}

		#endregion

		private void OpenDoor()
		{
			// TODO: animate door opening
			_door.EnterNextRoom();
		}

		private void DisplayRoomType()
		{
			if (_door.LeadToRoom == null)
				return;
			_defaultDoorGfx.SetActive(false);
			switch (_door.LeadToRoom.Type)
			{
				case RoomType.EVENT:
				case RoomType.COMBAT:
					_rewardDoors[_door.LeadToRoom.Reward].SetActive(true);
					break;
				default:
					_typedDoors[_door.LeadToRoom.Type].SetActive(true);
					break;
			}
		}

		private void OnActivated()
		{
			if (_door.LeadToRoom == null)
				return;
			_animator?.SetBool(ACTIVATE_ANIMATOR, true);
			_totems.ForEach(x => x.gameObject.SetActive(true));
		}
	}
}
