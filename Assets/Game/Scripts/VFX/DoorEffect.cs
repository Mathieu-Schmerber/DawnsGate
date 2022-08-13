using Game.Managers;
using Game.Systems.Run;
using Game.Systems.Run.GPE;
using Game.Systems.Run.Rooms;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.VFX
{
	public class DoorEffect : SerializedMonoBehaviour
	{
		private const string ACTIVATE_ANIMATOR = "Activated";

		private RoomDoor _door;

		[SerializeField] private AnimationClip _doorOpenAnimation;
		[SerializeField] private Animator _roomClearAnimator = null;
		[SerializeField] private ParticleSystem[] _totems;
		[SerializeField] private GameObject _defaultDoorGfx;
		[SerializeField] private Dictionary<RoomType, GameObject> _typedDoors = new();
		[SerializeField] private Dictionary<RoomRewardType, GameObject> _rewardDoors = new();

		private Animator _doorAnimator;

		#region Unity builtins

		private void Awake()
		{
			_door = GetComponentInParent<RoomDoor>();
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
			_doorAnimator.enabled = true;
			GameManager.Player.Restrict();
			GameManager.Camera.LockTemporaryTarget(GameManager.Player.transform, 0.8f);
			Awaiter.WaitAndExecute(_doorOpenAnimation.length + 0.5f, () =>
			{
				GameManager.Player.UnRestrict();
				GameManager.Camera.UnlockTarget();
				_door.EnterNextRoom();
			});
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
					_doorAnimator = _rewardDoors[_door.LeadToRoom.Reward].GetComponent<Animator>();
					break;
				default:
					_typedDoors[_door.LeadToRoom.Type].SetActive(true);
					_doorAnimator = _typedDoors[_door.LeadToRoom.Type].GetComponent<Animator>();
					break;
			}
		}

		private void OnActivated()
		{
			if (_door.LeadToRoom == null)
				return;
			
			_totems.ForEach(x => x.gameObject.SetActive(true));
			if (_roomClearAnimator != null)
				_roomClearAnimator.SetBool(ACTIVATE_ANIMATOR, true);
		}
	}
}
