using Game.Entities.Miscellaneous;
using Nawlian.Lib.Systems.Pooling;
using Nawlian.Lib.Utils;
using UnityEngine;

namespace Game.Systems.Items.Passive
{
	public class LavaPotion : ASpecialItem
	{
		private Timer _timer;

		public override void OnEquipped(ItemBaseData data, int quality)
		{
			base.OnEquipped(data, quality);
			_timer = new();
			_timer.Start(_data.Stages[Quality].Duration, true, OnTick);
		}

		public override void OnUnequipped()
		{
			_timer.Stop();
			base.OnUnequipped();
		}

		public override void OnUpgrade()
		{
			base.OnUpgrade();
			_timer.Interval = _data.Stages[Quality].Duration;
		}

		private void OnTick()
		{
			ObjectPooler.Get(_data.SpawnPrefab, _entity.transform.position, Quaternion.identity, _data.Stages[Quality],
				(go) => go.GetComponent<Lava>().Caster = _entity);
		}
	}
}