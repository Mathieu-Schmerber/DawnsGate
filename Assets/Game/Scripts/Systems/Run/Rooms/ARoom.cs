using Game.Entities.Shared;
using Game.Managers;
using Nawlian.Lib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Systems.Run.Rooms
{
	public abstract class ARoom : MonoBehaviour, IRoom
	{
		private RoomInfo _info;
		protected EntityIdentity _player;

		public static event Action OnRoomActivated;
		public static event Action OnRoomCleared;

		public RoomInfo Info => _info;
		public bool Cleared { get; protected set; }
		public abstract bool RequiresNavBaking { get; }
		public float GroundLevel => _info.Data.GroundLevel;

		public Room RoomData => RunManager.CurrentRoom;


		protected virtual void Awake()
		{
			_info = GetComponent<RoomInfo>();
			_player = GameManager.Player.GetComponent<EntityIdentity>();
			RunManager.CurrentRoomInstance = this;
		}

		protected virtual void Start()
		{
			SceneManager.SetActiveScene(gameObject.scene);

			if (RoomData.NextRooms != null && RoomData.NextRooms.Count > 0)
			{
				for (int i = 0; i < Mathf.Min(RoomData.NextRooms.Count, _info.Doors.Length); i++)
					_info.Doors[i].LeadToRoom = RoomData.NextRooms[i];
			}
		}

		protected abstract void OnActivate();

		protected abstract void OnClear();

		public void Activate()
		{
			_player.RefillArmor();
			OnRoomActivated?.Invoke();
			OnActivate();
		}

		public void Clear()
		{
			AudioManager.PlayTheme(RunManager.RunSettings.RunTheme, false);
			_player.RefillArmor();
			OnRoomCleared?.Invoke();
			Cleared = true;
			_info.Doors.ForEach(x => x.Activate());
			OnClear();
		}
	}
}
