using Game.Entities.Miscellaneous;
using Game.Entities.Shared;
using Nawlian.Lib.Systems.Pooling;
using Nawlian.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Items.Active
{
	public class BombWarp : ASpecialItem
	{
		private AController _controller;

		protected override void Awake()
		{
			base.Awake();
			_controller = GetComponentInParent<AController>();
		}

		#region AEquippedItem

		public override void OnEquipped(ItemBaseData data, int quality)
		{
			base.OnEquipped(data, quality);
			_controller.OnDashStarted += OnDashStarted;
		}

		public override void OnUnequipped()
		{
			_controller.OnDashStarted -= OnDashStarted;
			base.OnUnequipped();
		}

		#endregion

		private void OnDashStarted(DashParameters dash)
		{
			ObjectPooler.Get(_data.SpawnPrefab, _controller.transform.position, Quaternion.Euler(0, 0, 0), _data.Stages[_quality], 
				(bomb) => bomb.GetComponent<Bomb>().Caster = _entity);
		}
	}
}
