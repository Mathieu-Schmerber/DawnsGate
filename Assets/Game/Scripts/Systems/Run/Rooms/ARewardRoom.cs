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
			base.OnClear();
			SpawnReward();
		}

		protected virtual void SpawnReward()
		{
			switch (RoomData.Reward)
			{
				case RoomRewardType.STARS:
					GameManager.RewardWithRunMoney(UnityEngine.Random.Range(RunManager.RunSettings.RoomMoneyReward.x, RunManager.RunSettings.RoomMoneyReward.y + 1));
					break;
				case RoomRewardType.ITEM:
					var inventory = _player.GetComponent<Inventory>();
					var item = Databases.Database.Data.Item.All<ItemBaseData>().Where(x => !x.IsLifeItem && !inventory.HasEquipped(x)).Random();
					LootedItem.Create(_player.transform.position, new ItemSummary() { Data = item, Quality = 0 });
					break;
				default:
					break;
			}
		}
	}
}
