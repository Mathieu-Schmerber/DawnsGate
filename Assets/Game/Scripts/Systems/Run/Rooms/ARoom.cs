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

		public bool Cleared { get; protected set; }

		protected virtual void Awake()
		{
			Debug.Log("Awake");
			_info = GetComponent<RoomInfo>();
		}

		protected virtual void Start()
		{
			Room room = RunManager.CurrentRoom;

			for (int i = 0; i < room.NextRooms.Count; i++)
				_info.Doors[i].LeadToRoom = room.NextRooms[i];
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
			Debug.Log("Clear");
			OnRoomCleared?.Invoke();
			Cleared = true;
			OnClear();
		}
	}
}
