using Nawlian.Lib.Extensions;
using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEngine;

namespace Game.Systems.Run
{
	[CreateAssetMenu(menuName = "Data/Run/Room Rule")]
	public class RoomRuleData : ScriptableObject
	{
		[System.Serializable, InlineProperty]
		public class RangeInt
		{
			public bool Mandatory;
			[Range(0, 100), ShowIf("@Mandatory == false")] public int Probability;

			public RangeInt(int value) => Probability = value;
		}

		[System.Serializable]
		public class RoomDictionary : SerializedDictionary<RoomType, RangeInt> {}

		[TextArea] public string RuleDescription;
		[ReadOnly, ShowInInspector] public int MinRoomChoice
		{
			get
			{
				return Mathf.Max(1, RoomProbabilities.Count(x => x.Value.Mandatory));
			}
		}
		[ReadOnly, ShowInInspector] public int MaxRoomChoice
		{
			get
			{
				int nonReward = RoomProbabilities.Count(x => !Room.IsRewardRoom(x.Key));
				int reward = RoomProbabilities.Count - nonReward;

				return Mathf.Min(nonReward + (reward * Room.REWARD_NUMBER), Databases.Database.Data.Run.Settings.MaxExitNumber);
			}
		}

		[Space]

		[ValidateInput(nameof(ValidateEditor), "The sum of all probabilities should be 100%")]
		public RoomDictionary RoomProbabilities;

		public RoomType GetRandomNonMandatory()
		{
			var nonMandatories = RoomProbabilities.Where(x => !x.Value.Mandatory).ToArray();

			if (nonMandatories.Length == 0)
				return RoomProbabilities.Random().Key;

			int total = nonMandatories.Select(x => x.Value.Probability).Sum();
			int random = UnityEngine.Random.Range(0, total);
			int weight = 0;

			for (int i = 0; i < nonMandatories.Length; i++)
			{
				weight += nonMandatories[i].Value.Probability;
				if (random <= weight)
					return nonMandatories[i].Key;
			}
			throw new Exception("GetRandomNonMandatory(): No item got picked, make sure the sum of probabilities is 100%.");
		}

		#region Editor tools

#if UNITY_EDITOR

		private bool ValidateEditor()
		{
			if (RoomProbabilities.Any(x => !x.Value.Mandatory))
				return RoomProbabilities.Where(x => !x.Value.Mandatory).Select(x => x.Value.Probability).Sum() == 100;
			return true;
		}

#endif

		#endregion
	}
}