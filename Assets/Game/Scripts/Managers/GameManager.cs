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
		[SerializeField] private int _runMoney;
		[SerializeField] private int _lobbyMoney;

		public static event Action<int, int> OnRunMoneyUpdated;
		public static event Action<int, int> OnLobbyMoneyUpdated;

		public static int RunMoney => Instance._runMoney;
		public static int LobbyMoney => Instance._lobbyMoney;

		public static bool CanRunMoneyAfford(int cost) => RunMoney >= cost;
		public static void PayWithRunMoney(int cost)
		{
			int before = Instance._runMoney;

			Instance._runMoney -= cost;
			OnRunMoneyUpdated?.Invoke(before, Instance._runMoney);
		}

		public static bool CanLobbyMoneyAfford(int cost) => LobbyMoney >= cost;
		public static void PayWithLobbyMoney(int cost)
		{
			int before = Instance._lobbyMoney;

			Instance._lobbyMoney -= cost;
			OnLobbyMoneyUpdated?.Invoke(before, Instance._lobbyMoney);
		}

		public static PlayerController Player => Instance._player;
		public static CameraController Camera => Instance._camera;
	}
}
