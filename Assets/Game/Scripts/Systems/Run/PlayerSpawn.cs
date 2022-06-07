using Game.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Run
{
	public class PlayerSpawn : MonoBehaviour
	{
		private void Start()
		{
			GameManager.Player.transform.position = transform.position;
		}
	}
}
