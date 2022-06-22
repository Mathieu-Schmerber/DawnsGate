using Game.Entities.Player;
using Game.Systems.Combat.Effects;

namespace Game.Systems.Items.Active
{
	public class BerserkerMask : AActiveItem
	{
		private PlayerWeapon _weaponHolder;
		private EffectProcessor _effectProcessor;

		protected override void Awake()
		{
			base.Awake();
			_weaponHolder = GetComponentInParent<PlayerWeapon>();
			_effectProcessor = GetComponentInParent<EffectProcessor>();
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
			_effectProcessor.ApplyEffect(_data.ApplyEffect, _data.Stages[Quality].Duration);
		}
	}
}
