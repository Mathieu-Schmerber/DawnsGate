namespace Game.Systems.Run.Rooms
{
	public class AutoClearRoom : ARoom
	{
		public override bool RequiresNavBaking => false;

		protected override void OnRoomReady()
		{
			Activate();
		}

		protected override void OnActivate() => Clear();

		protected override void OnClear() {}
	}
}
