using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Game.Systems.Run
{
	[CreateAssetMenu(menuName = "Data/Run/Run settings")]
    public class RunSettingsData : ScriptableObject
    {
		#region Types

        [System.Serializable] [InlineProperty]
        public class RoomFolder
		{
            [ValidateInput(nameof(IsFolderEmpty), "This folder contains no scene.")]
            [FolderPath(RequireExistingPath = true), LabelText("@GetFolderDisplayName()")] public string Folder;

#if UNITY_EDITOR

            private string GetFolderDisplayName() => $"{nameof(Folder)} ({GetValidSceneNumber()})";

            public int GetValidSceneNumber() => Directory.GetFiles(Folder, "*.unity", SearchOption.TopDirectoryOnly).Length;

            private bool IsFolderEmpty() => GetValidSceneNumber() > 0;

#endif
        }

        [System.Serializable] public class RoomFolderDictionary : SerializedDictionary<RoomType, RoomFolder> { }

		#endregion

		[Title("General")]
        [MinValue(0)] public int MaxExitNumber;
        public RoomFolderDictionary RoomFolders;

        [Title("Rules"), ValidateInput("@ValidateMaxExit().Item1", "@ValidateMaxExit().Item2")]
        [LabelText("Room Order")] public RoomRuleData[] RoomRules;

        #region Editor check

#if UNITY_EDITOR

        public (bool, string) ValidateMaxExit()
        {
            if (GetError() != null)
                return (false, GetError());

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

		#endregion
	}
}
