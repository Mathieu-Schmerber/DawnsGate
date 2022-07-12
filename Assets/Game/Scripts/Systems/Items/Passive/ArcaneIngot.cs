using Game.Entities.Shared;
using Nawlian.Lib.Utils;
using System;

namespace Game.Systems.Items.Passive
{
	public class ArcaneIngot : ASpecialItem
	{
		private Timer _timer;

		public override void OnEquipped(ItemSummary item)
		{
			base.OnEquipped(item);
			_timer = new();
			_timer.Start(_data.Stages[Quality].Duration, true, OnTick);
		}

		public override void OnUnequipped()
		{
			_timer.Stop();
			base.OnUnequipped();
		}

		protected override void OnUpgrade()
		{
			base.OnUpgrade();
			_timer.Interval = _data.Stages[Quality].Duration;
		}

		private void OnTick()
		{
			if (_entity.CurrentArmor > 0)
				_entity.CurrentArmor += _entity.MaxArmor * (_data.Stages[Quality].Amount / 100);
		}
	}
}