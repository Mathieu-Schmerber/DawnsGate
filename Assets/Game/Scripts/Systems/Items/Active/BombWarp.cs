using Game.Entities.Miscellaneous;
using Game.Entities.Shared;
using Nawlian.Lib.Systems.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Systems.Items.Active
{
	public class BombWarp : AEquippedItem
	{
		private ActiveItemData _data;
		private AController _controller;

		protected override void Awake()
		{
			base.Awake();
			_controller = GetComponentInParent<AController>();
		}

		#region AEquippedItem

		public override ItemBaseData Details => _data;
		public override string GetDescription() => _data.Description;

		public override void OnEquipped(ItemBaseData data, int quality)
		{
			base.OnEquipped(data, quality);
			_data = data as ActiveItemData;
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
