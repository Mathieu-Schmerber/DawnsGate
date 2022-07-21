using Game.Managers;
using UnityEngine;

namespace Game.Systems.Run.Rooms.Events
{
	public class CasinoRoom : ARewardRoom
	{
		[SerializeField] private AudioClip _roomTheme;

		public override bool RequiresNavBaking => false;

		protected override void OnRoomReady()
		{
			base.OnRoomReady();
			Activate();
			AudioManager.PlayTheme(_roomTheme);
		}

		protected override void OnActivate()
		{

		}

		protected override void SpawnReward()
		{
			// No reward
		}
	}
}
