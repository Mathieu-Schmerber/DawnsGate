using System.Collections;
using UnityEngine;

namespace Game.Systems.Run.Rooms
{
	public class CombatRoom : ARoom
	{
		protected override void Start()
		{
			base.Start();
			StartCoroutine(ClearTest());
		}

		private IEnumerator ClearTest()
		{
			yield return new WaitForSeconds(1);
			Clear();
		}

		protected override void OnActivate()
		{
		}
	}
}
