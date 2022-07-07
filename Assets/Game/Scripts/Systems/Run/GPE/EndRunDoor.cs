using Game.Managers;
using Nawlian.Lib.Systems.Interaction;

namespace Game.Systems.Run.GPE
{
	public class EndRunDoor : RoomDoor
	{
		public override void Activate() => OnActivate();

		public override void Interact(IInteractionActor actor)
		{
			actor.UnSuggestInteraction(this);
			RunManager.EndRun();
		}
	}
}
