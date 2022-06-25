using Game.Entities.Player;
using Game.Systems.Combat.Effects;

namespace Game.Systems.Items.Active
{
	public class BerserkerMask : ASpecialItem
	{
		private PlayerWeapon _weaponHolder;
		private EffectProcessor _effectProcessor;

		protected override void Awake()
		{
			base.Awake();
			_weaponHolder = GetComponentInParent<PlayerWeapon>();
			_effectProcessor = GetComponentInParent<EffectProcessor>();
		}

		public override void OnEquipped(ItemSummary item)
		{
			base.OnEquipped(item);
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
