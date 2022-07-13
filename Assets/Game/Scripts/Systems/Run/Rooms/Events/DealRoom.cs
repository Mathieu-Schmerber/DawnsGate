using Game.Entities.AI.Dealer;
using Game.Systems.Items;
using UnityEngine;

namespace Game.Systems.Run.Rooms.Events
{
	public class DealRoom : BossRoom
	{
		private DealerDialogue _dialogue;

		public override bool ActivateOnStart => false;
		public override bool ActivateOnBossSpawn => false;
		public override bool GiveBossReward => false;

		protected override void Start()
		{
			base.Start();
			_dialogue = _bossIdentity.GetComponentInChildren<DealerDialogue>();
		}

		protected override void OnClear()
		{
			// Stop from getting default reward
		}

		public override void OnEnemyKilled(GameObject gameObject)
		{
			base.OnEnemyKilled(gameObject);

			switch (RoomData.Reward)
			{
				case RoomRewardType.STARS:
					SpawnReward(_dialogue.CurrentDeal.MoneyAmount);
					break;
				case RoomRewardType.ITEM:
					SpawnReward(_dialogue.CurrentDeal.Item);
					break;
			}
		}
	}
}
