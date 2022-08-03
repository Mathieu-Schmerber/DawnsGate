using Game.Entities.AI.Dealer;
using Game.Managers;
using Game.Systems.Items;
using Nawlian.Lib.Extensions;
using System.Linq;
using UnityEngine;

namespace Game.Systems.Run.Rooms.Events
{
	public class DealRoom : BossRoom
	{
		[SerializeField] private RoomTotem[] _totems;
		private DealerDialogue _dialogue;

		public override bool ActivateOnStart => false;
		public override bool ActivateOnBossSpawn => false;
		public override bool GiveBossReward => false;

		protected override void OnRoomSceneReady()
		{
			base.OnRoomSceneReady();
			_dialogue = _bossIdentity.GetComponentInChildren<DealerDialogue>();
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			_totems.ForEach(x => x.IsActive = true);
			RoomTotem.OnStateChanged += RoomTotem_OnStateChanged;
		}

		protected override void OnClear()
		{
			// Stop from getting default reward
		}

		private void RoomTotem_OnStateChanged(bool active)
		{
			_bossIdentity.SetInvulnerable(_totems.Any(x => x.IsActive));
		}

		public override void OnEnemyKilled(GameObject gameObject)
		{
			base.OnEnemyKilled(gameObject);

			switch (RoomData.Reward)
			{
				case RoomRewardType.GOLD:
					SpawnReward(_dialogue.CurrentDeal.MoneyAmount);
					break;
				case RoomRewardType.ITEM:
					SpawnReward(_dialogue.CurrentDeal.Item);
					break;
			}
			RoomTotem.OnStateChanged -= RoomTotem_OnStateChanged;
		}
	}
}
