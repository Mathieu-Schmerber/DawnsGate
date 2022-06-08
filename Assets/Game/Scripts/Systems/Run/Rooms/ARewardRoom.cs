using System;
using UnityEngine;

namespace Game.Systems.Run.Rooms
{
	public abstract class ARewardRoom : ARoom
	{
		protected override void OnClear()
		{
			base.OnClear();
			SpawnReward();
		}

		protected virtual void SpawnReward()
		{
			// TODO: spawn reward
		}
	}
}
