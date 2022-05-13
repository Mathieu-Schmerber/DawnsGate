using Game.Entities.Camera;
using Game.Entities.Player;
using Nawlian.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Managers
{
	public class GameManager : ManagerSingleton<GameManager>
	{
		public PlayerController Player { get; private set; }
		public CameraController Camera { get; private set; }

		private void Awake()
		{
			Player = FindObjectOfType<PlayerController>();
			Camera = FindObjectOfType<CameraController>();
		}
	}
}
