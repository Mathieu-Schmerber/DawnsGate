using Game.Entities.Shared;
using Game.Entities.Shared.Health;
using Game.Systems.Combat.Attacks;
using Nawlian.Lib.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Systems.Items.Active
{
	public class SharpenedBlade : ASpecialItem
	{
		private AController _controller;
		private bool _applyDamage;
		private List<Collider> _hits = new();

		protected override void Awake()
		{
			base.Awake();
			_controller = GetComponentInParent<AController>();
		}

		#region AEquippedItem

		public override void OnEquipped(ItemSummary item)
		{
			base.OnEquipped(item);
			_controller.OnDashStarted += OnDashStarted;
		}

		public override void OnUnequipped()
		{
			_controller.OnDashStarted -= OnDashStarted;
			base.OnUnequipped();
		}

		#endregion

		private void Update()
		{
			if (!_applyDamage) return;

			var inRange = Physics.OverlapSphere(transform.position, 1f);

			foreach (var obj in inRange)
			{
				if (_hits.Contains(obj))
					continue;
				_hits.Add(obj);
				AttackBase.ApplyDamageLogic(_entity, obj.GetComponent<Damageable>(), KnockbackDirection.FORWARD, _data.Stages[Quality].Damage, 1f, _data.SpawnPrefab);
			}
		}

		private void OnDashStarted(DashParameters dash)
		{
			_applyDamage = true;
			Awaiter.WaitAndExecute(dash.Time, OnDashDone);
		}

		private void OnDashDone()
		{
			_applyDamage = false;
			_hits.Clear();
		}
	}
}
