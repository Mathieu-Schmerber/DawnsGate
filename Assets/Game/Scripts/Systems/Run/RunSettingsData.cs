using Game.Entities.Shared;
using Game.Scripts.Tools;
using Game.Systems.Run.Rooms;
using Nawlian.Lib.Extensions;
using Nawlian.Lib.Utils;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
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
            [HideInInspector] public bool HasError;
            [ReadOnly, ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, ShowIndexLabels = false, ElementColor = nameof(GetElementColor))]
            public List<RoomInfoData> RoomDatas;

			#region Editor
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

            public void Refresh()
			{
                bool state = HasError;

                CheckValidity();
                HasError = RoomDatas.Any(x => x.HasErrors);

                if (HasError != state)
				{
                    EditorUtility.SetDirty(Databases.Database.Data.Run.Settings);
                    AssetDatabase.SaveAssets();
                }
            }

			[OnInspectorInit]
            private void CheckValidity()
			{
                string[] guids = AssetDatabase.FindAssets("t:" + typeof(RoomInfoData).Name);

                HasError = false;
                RoomDatas = new List<RoomInfoData>();
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);

                    if (path.Contains(Folder))
                    {
                        RoomInfoData rid = AssetDatabase.LoadAssetAtPath<RoomInfoData>(path);
                        RoomDatas.Add(rid);
                    }
                }
            }

#endif
			#endregion
		}

		[System.Serializable] public class RoomFolderDictionary : SerializedDictionary<RoomType, RoomFolder>
        {
            public void Refresh() => Values.ForEach(x => x.Refresh());
        }

		#endregion

		[Title("General", "Essentials settings and room scene setups")]

        [@Tooltip("Maximum number of doors that can be activated in a room")]
        [MinValue(0)] public int MaxExitNumber;
        [Sirenix.OdinInspector.FilePath(RequireExistingPath = true)] public string BootScenePath;
        [OnInspectorGUI(nameof(ValidateFolders))]
        public string LobbySceneName;
        public RoomFolderDictionary RoomFolders;

        [Title("Rules", "Run generation rules")]

        [ValidateInput("@ValidateMaxExit().Item1", "@ValidateMaxExit().Item2")]
        public RoomTypeRewardPair FirstRoom;
        [LabelText("Room Order")] 
        public RoomRuleData[] RoomRules;

        #region Editor check

#if UNITY_EDITOR

        public void ValidateFolders()
		{
			RoomFolder error = RoomFolders.Values.FirstOrDefault(x => x.HasError);

            if (error != null)
                SirenixEditorGUI.ErrorMessageBox($"Some {RoomFolders.Keys.First(x => RoomFolders[x] == error)} rooms contain errors which will affect the gameplay.");
        }

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
