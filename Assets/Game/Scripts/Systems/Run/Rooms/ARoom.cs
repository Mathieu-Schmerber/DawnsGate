using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Run.Rooms
{
	public abstract class ARoom : MonoBehaviour, IRoom
	{
		public static event Action OnRoomActivated;
		public static event Action OnRoomCleared;

		protected abstract void OnActivate();

		protected virtual void OnClear()
		{
			// TODO: Enable door logic
		}

		public void Activate()
		{
			OnRoomActivated?.Invoke();
			OnActivate();
		}

		public void Clear()
		{
			OnRoomCleared?.Invoke();
			OnClear();
		}
	}
}
