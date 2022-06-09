namespace Game.Systems.Run.Rooms
{
	public class IdleRoom : ARoom
	{
		public override bool RequiresNavBaking => false;

		protected override void Start()
		{
			base.Start();
			Activate();
		}

		protected override void OnActivate() => Clear();
	}
}
