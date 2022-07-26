using Nawlian.Lib.Extensions;
using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using ReadOnlyAttribute = Sirenix.OdinInspector.ReadOnlyAttribute;

namespace Game.Entities.Shared
{
	public class EntityIdentity : MonoBehaviour
	{
		[SerializeField] protected BaseStatData _stats;

		protected BaseStatData _cachedStat;
		private float _currentHealth;
		private float _currentArmor;

		public event Action OnArmorBroken;
		public event Action<float> OnArmorGained;

		public event Action<float, float> OnHealthChanged;
		public event Action<float, float> OnArmorChanged;

		public string DisplayName => _stats.DisplayName;

		[ShowInInspector, ReadOnly]
		public bool IsInvulnerable { get; private set; }

		[ShowInInspector, ReadOnly]
		public float CurrentHealth { get => _currentHealth;
			set
			{
				float before = _currentHealth;

				_currentHealth = Mathf.Clamp(value, 0, MaxHealth);
				OnHealthChanged?.Invoke(before, _currentHealth);
			}
		}

		[ShowInInspector, ReadOnly]
		public float CurrentArmor { get => _currentArmor;
			set
			{
				float before = _currentArmor;

				_currentArmor = Mathf.Clamp(value, 0, MaxArmor);
				if (_currentArmor == 0 && before > _currentArmor)
					OnArmorBroken?.Invoke();
				else if (_currentArmor > 0 && before == 0)
					OnArmorGained?.Invoke(_currentArmor);
				OnArmorChanged?.Invoke(before, _currentArmor);
			}
		}
		public float MaxHealth => Mathf.Max(1, Scale(Stats.StartHealth, StatModifier.MaxHealth));
		public float MaxArmor => Scale(MaxHealth, StatModifier.ArmorRatio);
		public float CurrentSpeed => Scale(Stats.MovementSpeed, StatModifier.MovementSpeed);
		public float CurrentDashRange => Scale(Stats.DashRange, StatModifier.DashRange);
		public float CurrentDashCooldown
		{
			get
			{
				float value = Stats.DashCooldown - Mathf.Abs(Stats.DashCooldown - Scale(Stats.DashCooldown, StatModifier.DashCooldown));
				
				return value;
			}
		}

		public virtual void ResetStats()
		{
			if (_cachedStat == null)
				_cachedStat = _stats.Clone() as BaseStatData; // Clone scriptable object so that we can edit it
			else
			{
				_cachedStat.Modifiers.ForEach(x => {
					x.Value.BonusModifier = 0;
					x.Value.TemporaryModifier = 0;
				});
			}
			CurrentHealth = MaxHealth;
			CurrentArmor = MaxArmor;
		}

		public void RefillArmor()
		{
			CurrentArmor = MaxArmor;
		}

		public BaseStatData Stats => _cachedStat;

		protected virtual void Awake() => ResetStats();

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

		public void SetInvulnerable(bool state) => IsInvulnerable = state;
	}
}