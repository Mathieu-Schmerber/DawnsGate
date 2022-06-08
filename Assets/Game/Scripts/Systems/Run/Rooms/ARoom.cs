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

		public static event Action OnRoomActivated;
		public static event Action OnRoomCleared;

		protected RoomInfo _roomInfo => _info;
		public bool Cleared { get; protected set; }
		public Room RoomData => RunManager.CurrentRoom;


		protected virtual void Awake()
		{
			_info = GetComponent<RoomInfo>();
		}

		protected virtual void Start()
		{
			SceneManager.SetActiveScene(gameObject.scene);

			for (int i = 0; i < RoomData.NextRooms.Count; i++)
				_info.Doors[i].LeadToRoom = RoomData.NextRooms[i];
		}

		protected abstract void OnActivate();

		protected virtual void OnClear() => _info.Doors.ForEach(x => x.Activate());

		public void Activate()
		{
			OnRoomActivated?.Invoke();
			OnActivate();
		}

		public void Clear()
		{
			OnRoomCleared?.Invoke();
			Cleared = true;
			OnClear();
		}
	}
}
