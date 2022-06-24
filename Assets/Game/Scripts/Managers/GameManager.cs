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
		[SerializeField] private int _money;

		public static event Action<int, int> OnMoneyUpdated;

		public static int Money => Instance._money;
		public static bool CanMoneyAfford(int cost) => Money >= cost;
		public static void PayWithMoney(int cost)
		{
			int before = Instance._money;

			Instance._money -= cost;
			OnMoneyUpdated?.Invoke(before, Instance._money);
		}

		public static PlayerController Player => Instance._player;
		public static CameraController Camera => Instance._camera;
	}
}
