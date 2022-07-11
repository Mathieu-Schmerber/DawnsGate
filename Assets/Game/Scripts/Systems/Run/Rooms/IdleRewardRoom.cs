namespace Game.Systems.Run.Rooms
{
	public class IdleRewardRoom : ARewardRoom
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
	}
}
