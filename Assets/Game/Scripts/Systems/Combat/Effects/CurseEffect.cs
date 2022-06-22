using Game.Entities.Shared.Health;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Systems.Combat.Effects
{
	public class CurseEffect : BasicEffect
	{
		private Damageable _damageable;
		private float _registeredDamage;

		protected override void Awake()
		{
			base.Awake();
			_damageable = _identity.GetComponent<Damageable>();
		}

		protected override void OnActivation()
		{
			base.OnActivation();
			if (_damageable)
				_damageable.OnDamaged += OnDamaged;
		}

		protected override void OnEnd()
		{
			base.OnEnd();
			if (_damageable)
			{
				_damageable.OnDamaged -= OnDamaged;
				_damageable?.ApplyPassiveDamage(_registeredDamage * 0.1f);
			}
		}

		private void OnDamaged(float damage) => _registeredDamage += damage;
	}
}
