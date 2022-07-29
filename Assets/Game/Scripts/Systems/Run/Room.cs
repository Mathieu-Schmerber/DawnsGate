using Nawlian.Lib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Systems.Run
{
	public enum RoomType
	{
        COMBAT,
        EVENT,
        SHOP,
        LIFE_SHOP,
        BOSS
    }

    public enum RoomRewardType { GOLD, ITEM, NONE }

    public class Room
    {
        public static readonly int REWARD_NUMBER = 2;
        public RoomType Type { get; set; }
        public RoomRewardType Reward { get; set; }
        public bool RewardRoom => IsRewardRoom(Type);
        public List<Room> NextRooms { get; set; }

        public static bool IsRewardRoom(RoomType type) => type == RoomType.COMBAT || type == RoomType.EVENT;
        public static bool IsUniqueRoom(RoomType type) => !IsRewardRoom(type);

        public void DefineExitsFromRule(RoomRuleData rule)
		{
            NextRooms = new List<Room>(Random.Range(rule.MinRoomChoice, rule.MaxRoomChoice));
            RoomType[] mandatoryRooms = rule.RoomProbabilities.Where(x => x.Value.Mandatory).Select(x => x.Key).ToArray();
            RoomDictionary possibleRooms = (RoomDictionary)rule.RoomProbabilities.Clone();

            foreach (RoomType room in mandatoryRooms)
			{
                NextRooms.Add(new Room()
                {
                    Type = room,
                    Reward = IsRewardRoom(room) ? (Random.Range(0, 2) == 0 ? RoomRewardType.GOLD : RoomRewardType.ITEM) : RoomRewardType.NONE
                });

                if (IsUniqueRoom(room))
                    ExcludeType(possibleRooms, room);
			}

            for (int i = 0; i < NextRooms.Capacity - mandatoryRooms.Length; i++)
			{
                RoomType type = possibleRooms.GetRandomNonMandatory();
                RoomRewardType reward = RoomRewardType.NONE;
                bool alreadyExists = NextRooms.Any(x => x.Type == type);

                reward = IsRewardRoom(type) ? GetUniqueReward(type) : RoomRewardType.NONE;
                if (reward == RoomRewardType.NONE && IsRewardRoom(type))
                {
                    ExcludeType(possibleRooms, type);
                    i--;
                    continue;
                }

                NextRooms.Add(new Room()
                {
                    Type = type,
                    Reward = reward
                });
            }
		}

        private void ExcludeType(RoomDictionary original, RoomType excluded)
		{
            int max = original.Select(x => x.Value.Probability).Sum();
            int exclusionValue = original[excluded].Probability;

            if (exclusionValue == 0)
                return;

            float ratio = max / exclusionValue;

            original.Remove(excluded);
            original.Values.ForEach(x => x.Probability = Mathf.FloorToInt(x.Probability * ratio));
		}

		private RoomRewardType GetUniqueReward(RoomType type)
		{
            var possibleRewards = ((RoomRewardType[])Enum.GetValues(typeof(RoomRewardType))).ToList();

            possibleRewards.Remove(RoomRewardType.NONE);
			foreach (var room in NextRooms)
			{
                if (room.Type == type)
                    possibleRewards.Remove(room.Reward);
			}
            if (possibleRewards.Count == 0)
                return RoomRewardType.NONE;
            return possibleRewards.Random();
        }
    }
}
