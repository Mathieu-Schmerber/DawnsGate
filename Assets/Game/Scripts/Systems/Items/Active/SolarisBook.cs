using Game.Entities.Player;
using Game.Systems.Combat.Effects;
using UnityEngine;

namespace Game.Systems.Items.Active
{
	public class SolarisBook : AActiveItem
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
			if (Random.Range(0, 100) > _data.Stages[Quality].Amount)
				return;
			args.Victim.GetComponent<EffectProcessor>()?.ApplyEffect(_data.ApplyEffect, _data.Stages[Quality].Duration);
		}
	}
}
