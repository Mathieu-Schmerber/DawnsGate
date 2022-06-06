using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Run.Rooms
{
	/// <summary>
	/// Handles room validity
	/// </summary>
	public class RoomInfo : MonoBehaviour
	{
		[SerializeField, ReadOnly] private RoomType _roomType;
		private ARoom _room;

		public RoomType Type { get => _roomType; set 
			{ 
				_roomType = value;
				OnTypeChanged();
			} 
		}

		private void OnTypeChanged()
		{
			if (_room != null)
				Destroy(_room);
			switch (Type)
			{
				case RoomType.COMBAT:
					_room = gameObject.AddComponent<CombatRoom>();
					break;
				case RoomType.EVENT:
					break;
				case RoomType.SHOP:
					break;
				case RoomType.LIFE_SHOP:
					break;
				case RoomType.UPGRADE:
					break;
				case RoomType.BOSS:
					break;
				default:
					break;
			}
		}
	}
}
