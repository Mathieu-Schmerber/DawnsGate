using Game.Entities.Shared;
using Game.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Entities.AI
{
	public class EnemyUi : MonoBehaviour
	{
		[SerializeField] private Image _healthBar;
		[SerializeField] private Image _armorBar;

		private EntityIdentity _identity;
		private Canvas _canvas;

		private void Awake()
		{
			_identity = GetComponentInParent<EntityIdentity>();
			_canvas = GetComponentInChildren<Canvas>();

			_canvas.worldCamera = GameManager.Camera.GetComponent<UnityEngine.Camera>();
		}

		private void OnEnable()
		{
			_identity.OnHealthChanged += _identity_OnHealthChanged;
			_identity.OnArmorChanged += _identity_OnArmorChanged;
		}

		private void OnDisable()
		{
			_identity.OnHealthChanged -= _identity_OnHealthChanged;
			_identity.OnArmorChanged -= _identity_OnArmorChanged;
		}

		private void _identity_OnArmorChanged()
		{
			_armorBar.fillAmount = _identity.CurrentArmor / _identity.MaxArmor;
		}

		private void _identity_OnHealthChanged()
		{
			_healthBar.fillAmount = _identity.CurrentHealth / _identity.MaxHealth;
		}
	}
}
