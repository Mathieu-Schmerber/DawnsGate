using Game.Systems.Run;
using Nawlian.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Managers
{
	public class RunManager : ManagerSingleton<RunManager>
	{
		public static RunSettingsData RunSettings => Databases.Database.Data.Run.Settings;

		#region Properties

		private int _currentRoom;

		#endregion

		#region Tools

		public static void StartNewRun()
		{
			Instance._currentRoom = 0;
		}

		public static void SelectNextRoom()
		{
			Instance._currentRoom++;
		}

		public static RoomRuleData GetNextRule() => RunSettings.RoomRules[Instance._currentRoom + 1];

		#endregion
	}
}
