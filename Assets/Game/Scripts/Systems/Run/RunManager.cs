using Nawlian.Lib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Systems.Run
{
	public class RunManager : ManagerSingleton<RunManager>
	{
		#region Properties

		public static RunSettingsData RunSettings => Databases.Database.Data.Run.Settings;

		#endregion

		#region Tools



		#endregion
	}
}
