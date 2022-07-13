namespace Game.Systems.Run.Rooms.Events
{
	public class DealRoom : BossRoom
	{
		public override bool ActivateOnStart => false;
		public override bool ActivateOnBossSpawn => false;
		public override bool GiveBossReward => false;
	}
}
