using Game.Entities.Shared;
using Game.Entities.Shared.Health;
using System;

namespace Game.Entities.Player
{
	public class PlayerDamageable : Damageable
	{
		private bool _hasRevived = false;
		private bool _canRevive => _identity.Stats.Modifiers[StatModifier.ReviveHealth].Value > 0;

		public event Action OnRevived;
		public event Action OnPlayerDeath;

		public override void Kill(EntityIdentity attacker)
		{
			if (_canRevive && !_hasRevived)
			{
				_hasRevived = true;
				_identity.CurrentHealth = _identity.Scale(_identity.MaxHealth, StatModifier.ReviveHealth);
				OnRevived?.Invoke();
			}
			else
			{
				OnPlayerDeath?.Invoke();
				_hasRevived = false;
			}
		}
	}
}
