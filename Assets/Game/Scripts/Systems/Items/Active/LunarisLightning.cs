using Game.Entities.Player;
using Nawlian.Lib.Systems.Pooling;
using UnityEngine;

namespace Game.Systems.Items.Active
{
	public class LunarisLightning : ASpecialItem
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

			float attackDamage = _entity.Scale(_data.Stages[Quality].Damage, Entities.Shared.StatModifier.AttackDamage);
			var inRange = Physics.OverlapSphere(args.Victim.transform.position, _data.Stages[Quality].Range);

			foreach (var collider in inRange)
			{
				if (collider.gameObject == _entity.gameObject || collider.gameObject == args.Victim.gameObject)
					continue;
				IDamageProcessor dmg = collider.GetComponent<IDamageProcessor>();

				if (dmg == null)
					continue;
				dmg.ApplyPassiveDamage(attackDamage);
				ObjectPooler.Get(_data.SpawnPrefab, collider.transform.position, Quaternion.identity, null);
			}
		}
	}
}
