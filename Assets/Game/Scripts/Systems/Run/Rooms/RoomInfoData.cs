using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Systems.Run.Rooms
{
	public class RoomInfoData : ScriptableObject
	{
		[HideInInspector] public bool HasErrors;
		[ReadOnly] public NavMeshData NavMesh;
		public List<Vector3> SpawnablePositions;
	}
}
