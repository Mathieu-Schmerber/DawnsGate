using System.Collections;
using UnityEngine;

namespace Game.Systems.Run.Rooms
{
	public class CombatRoom : ARoom
	{
		private IEnumerator Start()
		{
			yield return new WaitForSeconds(3);
			Clear();
		}

		protected override void OnActivate()
		{

		}
	}
}
