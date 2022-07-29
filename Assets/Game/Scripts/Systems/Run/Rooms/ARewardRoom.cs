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

		protected virtual void SpawnReward()
		{
			switch (RoomData.Reward)
			{
				case RoomRewardType.GOLD:
					int money = UnityEngine.Random.Range(RunManager.RunSettings.RoomMoneyReward.x, RunManager.RunSettings.RoomMoneyReward.y + 1);
					money = Mathf.RoundToInt(_player.Scale(money, Entities.Shared.StatModifier.GoldGain));
					GameManager.RewardWithRunMoney(money);
					break;
				case RoomRewardType.ITEM:
					var inventory = _player.GetComponent<Inventory>();
					var item = Databases.Database.Data.Item.All<ItemBaseData>().Where(x => !x.IsLifeItem && !inventory.HasEquipped(x)).Random();
					LootedItem.Create(_player.transform.position, new ItemSummary() { Data = item, Quality = 0 });
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
