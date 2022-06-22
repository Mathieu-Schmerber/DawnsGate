using Game.Entities.Player;
using Game.Systems.Combat.Attacks;
using Game.VFX;
using Nawlian.Lib.Systems.Pooling;
using System;
using UnityEngine;

namespace Game.Systems.Items.Active
{
	public class BloodChains : ASpecialItem
	{
		private PlayerWeapon _weaponHolder;

		protected override void Awake()
		{
			base.Awake();
			_weaponHolder = GetComponentInParent<PlayerWeapon>();
		}

		public override void OnEquipped(ItemBaseData data, int quality)
		{
			base.OnEquipped(data, quality);
			_weaponHolder.OnAttackHit += OnAttackHit;
		}

		public override void OnUnequipped()
		{
			_weaponHolder.OnAttackHit -= OnAttackHit;
			base.OnUnequipped();
		}

		private void OnAttackHit(PlayerWeapon.AttackHitEventArgs args)
		{
			if (args.IsHeavyAttack)
				return;
			float attackDamage = _entity.Scale(args.Data.BaseDamage, Entities.Shared.StatModifier.AttackDamage); 
			var inRange = Physics.OverlapSphere(args.Victim.transform.position, _data.Stages[Quality].Range);

			foreach (var item in inRange)
			{
				if (item.gameObject == _entity.gameObject || item.gameObject == args.Victim.gameObject)
					continue;
				IDamageProcessor dmg = item.GetComponent<IDamageProcessor>();

				if (dmg == null)
					continue;
				dmg.ApplyPassiveDamage(attackDamage * (_data.Stages[Quality].Amount / 100));
				ObjectPooler.Get(_data.SpawnPrefab, null, (go) =>
				{
					var fx = go.GetComponent<BloodChainFX>();
					fx.Origin = args.Victim.transform;
					fx.Destination = item.transform;
				});
			}
		}
	}
}
