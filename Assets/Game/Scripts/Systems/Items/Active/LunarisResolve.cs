using Game.Entities.Player;
using Game.Entities.Shared.Health;
using Nawlian.Lib.Systems.Pooling;
using UnityEngine;

namespace Game.Systems.Items.Active
{
	public class LunarisResolve : ASpecialItem
	{
		public override void OnEquipped(ItemSummary item)
		{
			base.OnEquipped(item);
			Damageable.OnDeath += Damageable_OnDeath;
		}

		public override void OnUnequipped()
		{
			Damageable.OnDeath -= Damageable_OnDeath;
			base.OnUnequipped();
		}

		private void Damageable_OnDeath(Damageable enemy)
		{
			if (enemy.gameObject.layer == _entity.gameObject.layer)
				return;
			OnEnemyKilled(enemy);
		}

		private void OnEnemyKilled(Damageable enemy)
		{
			float attackDamage = _entity.Scale(_data.Stages[Quality].Damage, Entities.Shared.StatModifier.AttackDamage);
			var inRange = Physics.OverlapSphere(enemy.transform.position, 1000);

			foreach (var collider in inRange)
			{
				if (collider.gameObject == _entity.gameObject || collider.gameObject == enemy.gameObject)
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