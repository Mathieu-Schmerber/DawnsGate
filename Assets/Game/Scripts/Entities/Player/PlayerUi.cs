using Game.Entities.Shared;
using Game.Managers;
using Pixelplacement;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Entities.Player
{
	public class PlayerUi : MonoBehaviour
	{
		[Title("References")]
		[SerializeField] private TextMeshProUGUI _runMoney;
		[SerializeField] private TextMeshProUGUI _lobbyMoney;
		[SerializeField] private TextMeshProUGUI _healthText;
		[SerializeField] private Image _healthFill;
		[SerializeField] private Image _armorFill;

		private EntityIdentity _playerIdentity;

		private void Awake()
		{
			_playerIdentity = GameManager.Player.GetComponent<EntityIdentity>();
		}

		private void Start()
		{
			_runMoney.text = GameManager.RunMoney.ToString();
			_lobbyMoney.text = GameManager.LobbyMoney.ToString();
		}

		private void OnEnable()
		{
			_playerIdentity.OnHealthChanged += UpdateHealthDisplay;
			_playerIdentity.OnArmorChanged += UpdateArmorDisplay;
			GameManager.OnLobbyMoneyUpdated += UpdateLobbyMoneyDisplay;
			GameManager.OnRunMoneyUpdated += UpdateRunMoneyDisplay; ;
		}

		private void OnDisable()
		{
			_playerIdentity.OnHealthChanged -= UpdateHealthDisplay;
			_playerIdentity.OnArmorChanged -= UpdateArmorDisplay;
			GameManager.OnLobbyMoneyUpdated -= UpdateLobbyMoneyDisplay;
			GameManager.OnRunMoneyUpdated -= UpdateRunMoneyDisplay; ;
		}

		private void UpdateHealthDisplay()
		{
			float ratio = _playerIdentity.CurrentHealth / _playerIdentity.MaxHealth;
		
			_healthText.text = $"{_playerIdentity.CurrentHealth}/{_playerIdentity.MaxHealth}";
			Tween.Value(_healthFill.fillAmount, ratio, (v) => _healthFill.fillAmount = v, 0.2f, 0, Tween.EaseOut);
		}

		private void UpdateArmorDisplay()
		{
			float ratio = _playerIdentity.CurrentArmor / _playerIdentity.MaxArmor;

			Tween.Value(_armorFill.fillAmount, ratio, (v) => _armorFill.fillAmount = v, 0.2f, 0, Tween.EaseOut);
		}

		private void UpdateRunMoneyDisplay(int before, int now) => Tween.Value(before, now, (v) => _runMoney.text = v.ToString(), 0.2f, 0);

		private void UpdateLobbyMoneyDisplay(int before, int now) => Tween.Value(before, now, (v) => _lobbyMoney.text = v.ToString(), 0.2f, 0);
	}
}
