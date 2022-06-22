using Game.Entities.Player;
using System;
using UnityEngine;

namespace Game.Systems.Items.Active
{
	public class GoldenClover : AActiveItem
	{
		private PlayerWeapon _weaponHolder;
		private int _consecutiveHits;

		protected override void Awake()
		{
			base.Awake();
			_weaponHolder = GetComponentInParent<PlayerWeapon>();
		}

		public override void OnEquipped(ItemBaseData data, int quality)
		{
			base.OnEquipped(data, quality);
			_weaponHolder.OnAttackHit += OnAttackHit;
			_weaponHolder.OnAttackLaunched += OnAttackLaunched;
		}

		public override void OnUnequipped()
		{
			_weaponHolder.OnAttackHit -= OnAttackHit;
			_weaponHolder.OnAttackLaunched -= OnAttackLaunched;
			base.OnUnequipped();
		}
		private void OnAttackLaunched() => _consecutiveHits = 0;

		private void OnAttackHit(PlayerWeapon.AttackHitEventArgs args)
		{
			if (!args.IsHeavyAttack) 
				return;
			_consecutiveHits++;
			args.Victim.ApplyPassiveDamage(args.DamageApplied * ((_consecutiveHits * _data.Stages[Quality].Damage) / 100));
		}
	}
}
