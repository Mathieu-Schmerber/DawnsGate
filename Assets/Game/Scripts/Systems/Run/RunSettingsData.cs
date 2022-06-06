using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Systems.Run
{

	[CreateAssetMenu(menuName = "Data/Run/Run settings")]
    public class RunSettingsData : ScriptableObject
    {
        [Title("General")]
        [MinValue(0)] public int MaxExitNumber;

        [Title("Rules"), ValidateInput("@ValidateMaxExit().Item1", "@ValidateMaxExit().Item2")]
        [LabelText("Room Order")] public RoomRuleData[] RoomRules;

#if UNITY_EDITOR

        public (bool, string) ValidateMaxExit()
		{
            RoomRuleData maxChoicesRule = RoomRules.Aggregate((a, b) => a.MinRoomChoice > b.MinRoomChoice ? a : b);

            if (MaxExitNumber < maxChoicesRule.MinRoomChoice)
                return (false, $"{nameof(MaxExitNumber)} set to {MaxExitNumber}, while the rule '{maxChoicesRule.name}' requires at least {maxChoicesRule.MinRoomChoice} exits.");
            return (true, null);
        }

        public string GetError()
		{
            if (RoomRules.Any(x => x is null))
                return "A room rule is null !";
            return null;
		}

        public bool HasNoError() => GetError() == null;

#endif
    }
}
