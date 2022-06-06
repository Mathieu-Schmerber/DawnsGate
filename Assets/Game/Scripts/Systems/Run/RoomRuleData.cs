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
		[ReadOnly, ShowInInspector] public int MinRoomChoice => RoomProbabilities.Where(x => x.Value.Mandatory).Count();

		[Space]

		[ValidateInput(nameof(ValidateEditor), "The sum of all probabilities should be 100%")]
		public RoomDictionary RoomProbabilities;

		private bool ValidateEditor()
		{
			if (RoomProbabilities.Any(x => !x.Value.Mandatory))
				return RoomProbabilities.Where(x => !x.Value.Mandatory).Select(x => x.Value.Probability).Sum() == 100;
			return true;
		}
	}
}