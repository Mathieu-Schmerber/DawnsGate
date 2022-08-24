using Game.Entities.Player;
using Game.Managers;
using Game.Systems.Items;
using Nawlian.Lib.Extensions;
using System;
using System.Linq;
using UnityEngine;

namespace Game.Systems.Run.Rooms
{
	public abstract class ARewardRoom : ARoom
	{
		protected override void OnClear()
		{
			SpawnReward();
		}

		protected void RewardWithGold(Vector2Int amountRange)
		{
			int money = UnityEngine.Random.Range(amountRange.x, amountRange.y + 1);
			GameManager.RewardWithRunMoney(money);
		}

		protected virtual void SpawnReward()
		{
			switch (RoomData.Reward)
			{
				case RoomRewardType.GOLD:
					RewardWithGold(RunManager.RunSettings.RoomMoneyReward);
					break;
				case RoomRewardType.ITEM:
					var inventory = _player.GetComponent<Inventory>();
					var item = Databases.Database.Data.Item.All<ItemBaseData>().Where(x => !x.IsLifeItem && !inventory.HasEquipped(x)).Random();
					LootedItem.Create(Info.GetClosestPosition(Info.Data.RoomCenter), new ItemSummary() { Data = item, Quality = 0 }, Vector3.zero);
					break;
				default:
					break;
			}
			GameManager.RewardWithLobbyMoney(UnityEngine.Random.Range(RunManager.RunSettings.LobbyMoneyPerRoom.x, RunManager.RunSettings.LobbyMoneyPerRoom.y + 1));
		}

		protected void SpawnReward(int runMoney) => GameManager.RewardWithRunMoney(runMoney);

		protected void SpawnReward(ItemSummary item) => LootedItem.Create(_player.transform.position, item);
	}
}
