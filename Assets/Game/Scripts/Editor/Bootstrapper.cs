using Game.Managers;
using System.Linq;
using UnityEngine;

namespace Game.Editor
{
	public static class Bootstrapper
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void Execute()
		{
			//var essentials = Databases.Database.Data.Boostrap.Essentials;
			//var managers = Databases.Database.Data.Boostrap.Managers;

			//EnsureSingle(managers);
			//var essentialsInstance = EnsureSingle(essentials);

			//if (essentials != null)
			//	GameManager.Init((GameObject)essentialsInstance);
		}
	}
}