using Game.Entities.Shared;
using Game.Systems.Run.Rooms;
using System;

namespace Game.Entities.AI.Dealer
{
	public class DealerAI : EnemyAI
	{
		protected override bool UsesPathfinding => true;

		public ARoom Room => _room;

		private DealerStatData _stats;

		#region Unity builtins

		protected override void OnEnable()
		{
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}

		protected override void Init(object data)
		{
			base.Init(data);
			_stats = _entity.Stats as DealerStatData;

			State = EntityState.STUN;
			_entity.SetInvulnerable(true);
		}

		#endregion

		public void TakeAction()
		{
			_room.Activate();
			State = EntityState.IDLE;
			_entity.SetInvulnerable(false);
		}

		protected override void Attack()
		{
			OnAttackEnd();
		}
	}
}
