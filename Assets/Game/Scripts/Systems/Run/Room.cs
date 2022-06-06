using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Systems.Run
{
	public enum RoomType
	{
        COMBAT,
        EVENT,
        SHOP,
        LIFE_SHOP,
        UPGRADE,
        BOSS
    }

    public enum RoomRewardType { STARS, ITEM }

    public class Room
    {
        public RoomType Type { get; set; }
        public RoomRewardType Reward { get; set; }
        public List<Room> NextRooms { get; set; }

        public void DefineExitsFromRule(int maxExit, RoomRuleData rule)
		{
            NextRooms = new List<Room>(Random.Range(rule.MinRoomChoice, maxExit));
            RoomType[] mandatoryRooms = rule.RoomProbabilities.Where(x => x.Value.Mandatory).Select(x => x.Key).ToArray();

            foreach (RoomType room in mandatoryRooms)
			{
                NextRooms.Add(new Room()
                {
                    Type = room,
                    Reward = Random.Range(0, 2) == 0 ? RoomRewardType.STARS : RoomRewardType.ITEM // 50% rate
                });
			}

            for (int i = 0; i < NextRooms.Capacity - mandatoryRooms.Length; i++)
			{
                RoomType type = rule.GetRandomNonMandatory();
                
                NextRooms.Add(new Room()
                {
                    Type = type,
                    Reward = Random.Range(0, 2) == 0 ? RoomRewardType.STARS : RoomRewardType.ITEM // 50% rate
                });
            }
		}
    }
}
