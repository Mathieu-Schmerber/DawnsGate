using Game.Entities.Shared;
using Game.Managers;
using Game.Systems.Run.Rooms;
using Game.Tools;
using Pixelplacement;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
	public class BossBarUi : ACloseableMenu
	{
		[SerializeField] private TextMeshProUGUI _bossNameTxt;
		[SerializeField] private Image _healthBar;

		private EntityIdentity _boss;

		public override bool RequiresGameFocus => false;

		public override void Open()
		{
			base.Open();
			RunManager.OnRunEnded += CloseBossUi;
			ARoom.OnRoomCleared += CloseBossUi;
		}

		public override void Close()
		{
			base.Close();
			if (_boss)
				_boss.OnHealthChanged -= UpdateHealth;
			RunManager.OnRunEnded -= CloseBossUi;
			ARoom.OnRoomCleared -= CloseBossUi;
		}

		private void CloseBossUi() => Close();

		public void Bind(EntityIdentity boss)
		{
			_boss = boss;
			_bossNameTxt.text = _boss?.DisplayName;

			if (_boss)
				_boss.OnHealthChanged += UpdateHealth;
		}

		private void UpdateHealth()
		{
			float ratio = _boss.CurrentHealth / _boss.MaxHealth;

			Tween.Value(_healthBar.fillAmount, ratio, (value) => _healthBar.fillAmount = value, _duration, 0, Tween.EaseOut);
		}
	}
}