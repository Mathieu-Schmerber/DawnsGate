using Game.Entities.Shared;
using Game.Managers;
using Game.Tools;
using Pixelplacement;
using Plugins.Nawlian.Lib.Systems.Menuing;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class PlayerUi : AMenu
	{
		[Title("References")]
		[SerializeField] private TextMeshProUGUI _runMoney;
		[SerializeField] private TextMeshProUGUI _lobbyMoney;
		[SerializeField] private TextMeshProUGUI _healthText;
		[SerializeField] private Image _healthFill;
		[SerializeField] private Image _armorFill;

		public override bool RequiresGameFocus => false;

		private void Start()
		{
			_runMoney.text = GameManager.RunMoney.ToString();
			_lobbyMoney.text = GameManager.LobbyMoney.ToString();
			_healthText.text = $"{GameManager.PlayerIdentity.CurrentHealth}/{GameManager.PlayerIdentity.MaxHealth}";
			_healthFill.fillAmount = GameManager.PlayerIdentity.CurrentHealth / GameManager.PlayerIdentity.MaxHealth;
			_armorFill.fillAmount = 0;
		}

		private void OnEnable()
		{
			GameManager.PlayerIdentity.OnHealthChanged += UpdateHealthDisplay;
			GameManager.PlayerIdentity.OnArmorChanged += UpdateArmorDisplay;
			GameManager.OnLobbyMoneyUpdated += UpdateLobbyMoneyDisplay;
			GameManager.OnRunMoneyUpdated += UpdateRunMoneyDisplay; ;
		}

		private void OnDisable()
		{
			if (GameManager.PlayerIdentity != null)
			{
				GameManager.PlayerIdentity.OnHealthChanged -= UpdateHealthDisplay;
				GameManager.PlayerIdentity.OnArmorChanged -= UpdateArmorDisplay;
			}
			GameManager.OnLobbyMoneyUpdated -= UpdateLobbyMoneyDisplay;
			GameManager.OnRunMoneyUpdated -= UpdateRunMoneyDisplay; ;
		}

		private void UpdateHealthDisplay()
		{
			float ratio = GameManager.PlayerIdentity.CurrentHealth / GameManager.PlayerIdentity.MaxHealth;

			_healthText.text = $"{GameManager.PlayerIdentity.CurrentHealth}/{GameManager.PlayerIdentity.MaxHealth}";
			Tween.Value(_healthFill.fillAmount, ratio, (v) => _healthFill.fillAmount = v, 0.2f, 0, Tween.EaseOut);
		}

		private void UpdateArmorDisplay()
		{
			float ratio = GameManager.PlayerIdentity.MaxArmor == 0 ? 0 : GameManager.PlayerIdentity.CurrentArmor / GameManager.PlayerIdentity.MaxArmor;

			Tween.Value(_armorFill.fillAmount, ratio, (v) => _armorFill.fillAmount = v, 0.2f, 0, Tween.EaseOut);
		}

		private void UpdateRunMoneyDisplay(int before, int now) => Tween.Value(before, now, (v) => _runMoney.text = v.ToString(), 0.2f, 0);

		private void UpdateLobbyMoneyDisplay(int before, int now) => Tween.Value(before, now, (v) => _lobbyMoney.text = v.ToString(), 0.2f, 0);
	}
}
