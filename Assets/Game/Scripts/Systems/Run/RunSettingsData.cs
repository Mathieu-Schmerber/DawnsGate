using Game.Systems.Run.Rooms;
using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Systems.Run
{
	[CreateAssetMenu(menuName = "Data/Run/Run settings")]
    public class RunSettingsData : ScriptableObject
    {
		#region Types

        [System.Serializable]
        public class RoomTypeRewardPair
		{
            public RoomType Type;
            [ShowIf("@Type == RoomType.COMBAT || Type == RoomType.EVENT")] public RoomRewardType Reward;
		}

        [System.Serializable] [InlineProperty]
        public class RoomFolder
		{
            [ValidateInput(nameof(IsFolderEmpty), "This folder contains no scene.")]
            [FolderPath(RequireExistingPath = true), LabelText("@GetFolderDisplayName()")] public string Folder;
            [ReadOnly, ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, ShowIndexLabels = false, ElementColor = nameof(GetElementColor))] 
            public List<RoomInfoData> RoomDatas;

#if UNITY_EDITOR

			private string GetFolderDisplayName() => $"{nameof(Folder)} ({GetValidSceneNumber()})";

            public int GetValidSceneNumber() => Directory.GetFiles(Folder, "*.unity", SearchOption.AllDirectories).Length;

            private bool IsFolderEmpty() => GetValidSceneNumber() > 0;

			private Color GetElementColor(int index, Color defaultColor)
			{
				if (RoomDatas[index] == null)
					return defaultColor;
				if (RoomDatas[index].HasErrors)
					return Color.red;
                return defaultColor;
			}

			[OnInspectorInit]
            private void CheckValidity()
			{
                //FindAssets uses tags check documentation for more info
                string[] guids = AssetDatabase.FindAssets("t:" + typeof(RoomInfoData).Name);
               
                RoomDatas = new List<RoomInfoData>();
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);

                    if (path.Contains(Folder))
                        RoomDatas.Add(AssetDatabase.LoadAssetAtPath<RoomInfoData>(path));
                }
            }

#endif
        }

        [System.Serializable] public class RoomFolderDictionary : SerializedDictionary<RoomType, RoomFolder> { }

		#endregion

		[Title("General")]
        [MinValue(0)] public int MaxExitNumber;
        [Sirenix.OdinInspector.FilePath(RequireExistingPath = true)] public string BootScenePath;
        public string LobbySceneName;
        public RoomFolderDictionary RoomFolders;

        [Title("Rules"), ValidateInput("@ValidateMaxExit().Item1", "@ValidateMaxExit().Item2")]
        public RoomTypeRewardPair FirstRoom;
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
