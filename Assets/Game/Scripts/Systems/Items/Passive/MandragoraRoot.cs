using Game.Systems.Run.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Items.Passive
{
	public class MandragoraRoot : ASpecialItem
	{
		public override void OnEquipped(ItemSummary item)
		{
			base.OnEquipped(item);
			ARoom.OnRoomCleared += OnRoomCleared;
		}

		public override void OnUnequipped()
		{
			base.OnUnequipped();
			ARoom.OnRoomCleared -= OnRoomCleared;
		}

		private void OnRoomCleared()
		{
			_entity.CurrentHealth += _entity.MaxHealth * (_data.Stages[Quality].Amount / 100);
		}
	}
}