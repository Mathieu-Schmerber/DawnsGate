using Game.Entities.Camera;
using Game.Entities.Player;
using Game.Entities.Shared;
using Game.Systems.Run.Lobby;
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
		[SerializeField] private PlayerIdentity _playerEntity;
		[SerializeField] private CameraController _camera;
		[SerializeField] private int _runMoney;
		[SerializeField] private int _lobbyMoney;

		#region Traits

		private Dictionary<TraitUpgradeData, int> _traitUpgrades = new();
		public static List<TraitUpgradeData> Traits => Instance._traitUpgrades.Keys.ToList();

		public static int GetTraitUpgradeCost(TraitUpgradeData trait)
		{
			if (Instance._traitUpgrades.ContainsKey(trait))
				return Mathf.RoundToInt(trait.BasePrice * (1 + (Instance._traitUpgrades[trait] * trait.PriceInflationPerUpgrade / 100)));
			return trait.BasePrice;
		}

		public static bool IsUpgradable(TraitUpgradeData trait)
			=> !Instance._traitUpgrades.ContainsKey(trait) || Instance._traitUpgrades[trait] < trait.NumberOfUpgrades;

		public static int GetTraitUpgradeCount(TraitUpgradeData trait)
		{
			if (Instance._traitUpgrades.ContainsKey(trait))
				return Instance._traitUpgrades[trait];
			return 0;
		}

		public static void ApplyTrait(TraitUpgradeData trait)
		{
			float healthRatio = Instance._playerEntity.CurrentHealth / Instance._playerEntity.MaxHealth;
			float armorRatio = Instance._playerEntity.MaxArmor == 0 ? 1 : Instance._playerEntity.CurrentArmor / Instance._playerEntity.MaxArmor;

			Instance._playerEntity.Stats.Modifiers[trait.StatModified].BonusModifier += trait.IncrementPerUpgrade;
			Instance._playerEntity.CurrentHealth = Mathf.Max(1, Instance._playerEntity.MaxHealth * healthRatio);
			Instance._playerEntity.CurrentArmor = Instance._playerEntity.MaxArmor * armorRatio;
		}

		public static bool UpgradeTrait(TraitUpgradeData trait)
		{
			int cost = GetTraitUpgradeCost(trait);

			if (!CanLobbyMoneyAfford(cost) || !IsUpgradable(trait))
				return false;
			else if (Instance._traitUpgrades.ContainsKey(trait))
				Instance._traitUpgrades[trait]++;
			else
				Instance._traitUpgrades.Add(trait, 1);
			PayWithLobbyMoney(cost);
			ApplyTrait(trait);
			return true;
		}

		#endregion

		#region Currencies

		public static event Action<int, int> OnRunMoneyUpdated;
		public static event Action<int, int> OnLobbyMoneyUpdated;

		public static int RunMoney => Instance._runMoney;
		public static int LobbyMoney => Instance._lobbyMoney;

		public static bool CanRunMoneyAfford(int cost) => RunMoney >= cost;

		public static void RewardWithRunMoney(int amount)
		{
			int before = Instance._runMoney;

			Instance._runMoney += amount;
			OnRunMoneyUpdated?.Invoke(before, Instance._runMoney);
		}

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

		public static void RewardWithLobbyMoney(int amount)
		{
			int before = Instance._lobbyMoney;

			Instance._lobbyMoney += amount;
			OnLobbyMoneyUpdated?.Invoke(before, Instance._lobbyMoney);
		}

		#endregion

		#region References

		public static PlayerController Player => Instance?._player;
		public static PlayerIdentity PlayerIdentity => Instance?._playerEntity;
		public static CameraController Camera => Instance?._camera;

		#endregion
	}
}
