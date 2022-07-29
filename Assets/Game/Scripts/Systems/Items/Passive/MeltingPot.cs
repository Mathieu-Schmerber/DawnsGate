using Game.Entities.Player;
using Game.VFX;
using System;

namespace Game.Systems.Items.Passive
{
	public class MeltingPot : ASpecialItem
	{
		private PlayerWeapon _weaponHolder;

		protected override void Awake()
		{
			base.Awake();
			_weaponHolder = GetComponentInParent<PlayerWeapon>();
		}

		public override void OnEquipped(ItemSummary summary)
		{
			base.OnEquipped(summary);
			_weaponHolder.OnAttackHit += OnAttackHit;
		}

		public override void OnUnequipped()
		{
			_weaponHolder.OnAttackHit -= OnAttackHit;
			base.OnUnequipped();
		}

		private void OnAttackHit(PlayerWeapon.AttackHitEventArgs attack)
		{
			if (!attack.IsHeavyAttack)
				return;

			float supplement = attack.DamageApplied * (_data.Stages[Quality].Damage) / 100;

			attack.Victim.ApplyPassiveDamage(supplement);
		}
	}
}