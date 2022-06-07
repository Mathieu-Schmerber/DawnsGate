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
		[SerializeField] private PlayerController _player;
		[SerializeField] private CameraController _camera;

		public static PlayerController Player => Instance._player;
		public static CameraController Camera => Instance._camera;
	}
}
