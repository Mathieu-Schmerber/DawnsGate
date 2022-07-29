using Game.Entities.Miscellaneous;
using Game.Entities.Shared.Health;
using Nawlian.Lib.Systems.Pooling;
using System.Linq;
using UnityEngine;

namespace Game.Systems.Items.Active
{
	public class LunarisRing : ASpecialItem
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
			if (Random.Range(0, 100) > _data.Stages[Quality].Amount)
				return;
			ObjectPooler.Get(_data.SpawnPrefab, enemy.transform.position, Quaternion.identity, _data.Stages[Quality],
				(soul) => soul.GetComponent<TrackerSoul>().Caster = _entity);
		}
	}
}