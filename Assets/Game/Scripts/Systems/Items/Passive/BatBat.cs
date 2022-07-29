using Game.Entities.Shared;
using Nawlian.Lib.Utils;
using System;
using UnityEngine;

namespace Game.Systems.Items.Passive
{
	public class BatBat : ASpecialItem
	{
		private float _attackGain = 0;

		private void ApplyNewGain()
		{
			float loss = (_entity.MaxHealth / 100) * (_entity.MaxHealth - _entity.CurrentHealth);

			// Remove old gain
			_entity.Stats.Modifiers[StatModifier.AttackDamage].BonusModifier -= _attackGain;

			// Calculate new gain
			_attackGain = (_data.Stages[Quality].Damage * loss) / 100f;
			
			// Apply
			_entity.Stats.Modifiers[StatModifier.AttackDamage].BonusModifier += _attackGain;
		}

		public override void OnEquipped(ItemSummary summary)
		{
			base.OnEquipped(summary);
			_entity.OnHealthChanged += OnHealthChanged;
			ApplyNewGain();
		}

		public override void OnUnequipped()
		{
			_entity.OnHealthChanged -= OnHealthChanged;
			ApplyNewGain();
			_entity.Stats.Modifiers[StatModifier.AttackDamage].BonusModifier -= _attackGain;
			base.OnUnequipped();
		}

		protected override void OnUpgrade()
		{
			base.OnUpgrade();
			ApplyNewGain();
		}

		private void OnHealthChanged(float prev, float now) => ApplyNewGain();
	}
}