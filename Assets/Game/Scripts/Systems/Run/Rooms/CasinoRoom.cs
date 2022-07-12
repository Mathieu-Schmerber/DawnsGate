namespace Game.Systems.Run.Rooms
{
	public class CasinoRoom : ARewardRoom
	{
		public override bool RequiresNavBaking => false;

		protected override void Start()
		{
			base.Start();
			Activate();
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
