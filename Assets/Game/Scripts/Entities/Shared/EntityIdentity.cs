using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Game.Entities.Shared
{
	public class EntityIdentity : MonoBehaviour
	{
		[SerializeField] private BaseStatData _stats;

		private BaseStatData _cachedStat;
		private float _currentHealth;
		private float _currentArmor;

		public bool IsInvulnerable { get; private set; }
		public float CurrentHealth { get => _currentHealth; set => _currentHealth = Mathf.Clamp(value, 0, _stats.StartHealth); }
		public float CurrentArmor { get => _currentArmor; set => _currentArmor = Mathf.Clamp(value, 0, Scale(Stats.StartHealth, StatModifier.ArmorRatio)); }
		public BaseStatData Stats => _cachedStat;

		private void Awake()
		{
			_cachedStat = _stats.Clone() as BaseStatData; // Clone scriptable object so that we can edit it
			CurrentHealth = Stats.StartHealth;
			CurrentArmor = Scale(Stats.StartHealth, StatModifier.ArmorRatio);
		}

		public float Scale(float baseValue, StatModifier modifier)
		{
			float modifierStat = Stats.Modifiers[modifier].Value;

			return baseValue * (modifierStat / 100f);
		}

		public void SetInvulnerable(float duration)
		{
			IsInvulnerable = true;
			Awaiter.WaitAndExecute(duration, () => IsInvulnerable = false);
		}
	}
}