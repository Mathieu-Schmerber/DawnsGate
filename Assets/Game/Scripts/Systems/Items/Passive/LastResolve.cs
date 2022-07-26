using Game.Entities.Shared;

namespace Game.Systems.Items.Passive
{
	public class LastResolve : ASpecialItem
	{
		private bool _boosted = false;

		private void CheckHealthLevel(float before, float now)
		{
			float ratio = _entity.CurrentHealth / _entity.MaxHealth;

			if (ratio * 100 < _data.Stages[Quality].Amount && !_boosted)
				Boost();
			else if (ratio * 100 >= _data.Stages[Quality].Amount && _boosted)
				UnBoost();
		}

		private void UnBoost()
		{
			_boosted = false;
			_entity.Stats.Modifiers[StatModifier.AttackDamage].TemporaryModifier -= 50;
			_entity.Stats.Modifiers[StatModifier.AttackSpeed].TemporaryModifier -= 25;
		}

		private void Boost()
		{
			_boosted = true;
			_entity.Stats.Modifiers[StatModifier.AttackDamage].TemporaryModifier += 50;
			_entity.Stats.Modifiers[StatModifier.AttackSpeed].TemporaryModifier += 25;
		}

		public override void OnEquipped(ItemSummary summary)
		{
			base.OnEquipped(summary);
			_boosted = false;
			CheckHealthLevel(0, 0);
		}

		public override void OnUnequipped()
		{
			base.OnUnequipped();
			_entity.OnHealthChanged -= CheckHealthLevel;
			if (_boosted)
				UnBoost();
		}
	}
}