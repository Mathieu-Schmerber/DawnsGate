using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Run.Rooms
{
	public class RoomInfoData : ScriptableObject
	{
		[ReadOnly] public bool HasErrors;
		public List<Vector3> SpawnablePositions;
	}
}
