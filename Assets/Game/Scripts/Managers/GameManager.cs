using Game.Entities.Camera;
using Game.Entities.Player;
using Game.Entities.Shared;
using Game.Systems.Run.Lobby;
using Game.UI;
using Nawlian.Lib.Systems.Saving;
using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Managers
{
	public class GameManager : ManagerSingleton<GameManager>, ISaveable
	{
		#region Types

		[Serializable]
		internal struct SaveData
		{
			public int LobbyMoney;
			public Dictionary<string, int> TraitUpgrades;
		}

		#endregion

		[Title("Gameplay references")]
		[SerializeField] private PlayerController _player;
		[SerializeField] private PlayerIdentity _playerEntity;
		[SerializeField] private CameraController _cameraController;
		[SerializeField] private Transform _cameraParent;

		[Title("Currencies")]
		[SerializeField] private int _runMoney;
		[SerializeField] private int _lobbyMoney;

		private ResourceSwitcher _switcher;

		public static Camera ActiveCamera => Instance._cameraParent.GetComponentsInChildren<Camera>().FirstOrDefault(x => x.gameObject.activeSelf);

		#region Game state

		private void Awake()
		{
			_switcher = GetComponent<ResourceSwitcher>();
			BootInit();
		}

		private void Start()
		{
			SaveSystem.Load();
#if UNITY_EDITOR
			if (!SceneManager.GetSceneByName("Main Menu").IsValid())
				StartGameFromEditor();
#endif
		}

#if UNITY_EDITOR
		public static void StartGameFromEditor()
		{
			GuiManager.DisplayGameplayUI();
			Instance._switcher.SwitchGameplayResources(true);
			RunManager.ArtificiallyLaunchScene();
		}
#endif

		private void BootInit() => _switcher.SwitchGameplayResources(false);

		public static void StartGame()
		{
			GuiManager.DisplayGameplayUI();
			SceneTransition.StartTransition(() =>
			{
				SceneManager.UnloadSceneAsync("Main Menu");
				var operation = SceneManager.LoadSceneAsync(RunManager.RunSettings.LobbySceneName, LoadSceneMode.Additive);

				operation.completed += (op) =>
				{
					Instance._switcher.SwitchGameplayResources(true);
					SceneManager.SetActiveScene(SceneManager.GetSceneByName(RunManager.RunSettings.LobbySceneName));
					SceneTransition.EndTransition(() => RunManager.OnLobbyEntered());
				};
			});
		}

		public static void StopGame()
		{
			SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
			SceneManager.LoadScene("_Boot", LoadSceneMode.Additive);
			SceneManager.LoadScene("_UI", LoadSceneMode.Additive);
		}

		#endregion

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

		public static void ApplySingleTraitUpgrade(TraitUpgradeData trait)
		{
			float healthRatio = Instance._playerEntity.CurrentHealth / Instance._playerEntity.MaxHealth;
			float armorRatio = Instance._playerEntity.MaxArmor == 0 ? 1 : Instance._playerEntity.CurrentArmor / Instance._playerEntity.MaxArmor;

			Instance._playerEntity.Stats.Modifiers[trait.StatModified].BonusModifier += trait.IncrementPerUpgrade;
			Instance._playerEntity.CurrentHealth = Mathf.Max(1, Instance._playerEntity.MaxHealth * healthRatio);
			Instance._playerEntity.CurrentArmor = Instance._playerEntity.MaxArmor * armorRatio;
		}

		public static void ResetTraits()
		{
			float healthRatio = Instance._playerEntity.CurrentHealth / Instance._playerEntity.MaxHealth;
			float armorRatio = Instance._playerEntity.MaxArmor == 0 ? 1 : Instance._playerEntity.CurrentArmor / Instance._playerEntity.MaxArmor;

			foreach (TraitUpgradeData trait in Instance._traitUpgrades.Keys)
				Instance._playerEntity.Stats.Modifiers[trait.StatModified].BonusModifier -= trait.IncrementPerUpgrade * Instance._traitUpgrades[trait];
			Instance._playerEntity.CurrentHealth = Mathf.Max(1, Instance._playerEntity.MaxHealth * healthRatio);
			Instance._playerEntity.CurrentArmor = Instance._playerEntity.MaxArmor * armorRatio;

			Instance._traitUpgrades.Clear();
		}

		public static void ApplyMultipleTraitUpgrades(TraitUpgradeData trait)
		{
			for (int i = 0; i < Instance._traitUpgrades[trait]; i++)
				ApplySingleTraitUpgrade(trait);
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
			ApplySingleTraitUpgrade(trait);
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
		public static CameraController Camera => Instance?._cameraController;

		#endregion

		#region Data lifecycle

		private void OnEnable()
		{
			RunManager.OnRunEnded += RunManager_OnRunEnded;
		}

		private void OnDisable()
		{
			RunManager.OnRunEnded -= RunManager_OnRunEnded;
		}

		private void RunManager_OnRunEnded()
		{
			PayWithRunMoney(_runMoney);
		}

		public void Load(object data)
		{
			SaveData save = (SaveData)data;

			ResetTraits();
			if (save.TraitUpgrades != null)
			{
				foreach (string id in save.TraitUpgrades.Keys)
				{
					TraitUpgradeData trait = Databases.Database.Data.Lobby.All<TraitUpgradeData>().FirstOrDefault(x => x.Id == id);

					if (trait != null)
					{
						_traitUpgrades[trait] = save.TraitUpgrades[id];
						ApplyMultipleTraitUpgrades(trait);
					}
				}
			}
			_lobbyMoney = 0;
			RewardWithLobbyMoney(save.LobbyMoney);
		}

		public object Save()
		{
			return new SaveData()
			{
				LobbyMoney = _lobbyMoney,
				TraitUpgrades = _traitUpgrades.ToDictionary(x => x.Key.Id, x => x.Value)
			};
		}

		private void OnApplicationQuit() => SaveSystem.Save();

		#endregion
	}
}
